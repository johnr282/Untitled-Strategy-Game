using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : NetworkBehaviour
{
    class StateUpdateRegistration
    {
        Delegate _validateUpdateFunc;
        Delegate _performUpdateFunc;
        Delegate _serverUpdateRPC;
        Delegate _clientUpdateRPC;
        Type _stateUpdateType;

        public StateUpdateRegistration(Delegate validateUpdateFuncIn,
            Delegate performUpdateFuncIn,
            Delegate serverUpdateRPCIn,
            Delegate clientUpdateRPCIn,
            Type stateUpdateTypeIn)
        {
            _validateUpdateFunc = validateUpdateFuncIn;
            _performUpdateFunc = performUpdateFuncIn;
            _serverUpdateRPC = serverUpdateRPCIn;
            _clientUpdateRPC = clientUpdateRPCIn;
            _stateUpdateType = stateUpdateTypeIn;
        }

        public bool CheckUpdateType<TStateUpdate>(TStateUpdate updateData)
        {
            return _stateUpdateType == updateData.GetType();
        }

        public bool ValidateUpdate(IStateUpdate updateData)
        {
            return (bool)_validateUpdateFunc.DynamicInvoke(updateData);
        }

        public void PerformUpdate(IStateUpdate updateData)
        {
            _performUpdateFunc.DynamicInvoke(updateData);
        }

        // Server update RPC is called on the client, so we need to go through
        // the ClientRPCManager
        public void CallServerUpdateRPC(NetworkRunner runner,
            string registrationString,
            IStateUpdate updateData)
        {
            ClientRPCManager.QueueClientRPC(_serverUpdateRPC,
                runner,
                PlayerRef.None,
                registrationString,
                updateData);
        }

        public void CallClientUpdateRPC(NetworkRunner runner,
            string registrationString,
            IStateUpdate updateData)
        {
            _clientUpdateRPC.DynamicInvoke(runner,
                registrationString,
                updateData);
        }
    }

    static Dictionary<string, StateUpdateRegistration> _registeredUpdates = new();
    static NetworkRunner StateManagerRunner
    {
        get
        {
            if (_stateManagerRunner == null)
            {
                NetworkObject networkObject = ProjectUtilities.FindComponent<NetworkObject>("StateManager");
                _stateManagerRunner = networkObject.Runner ??
                    throw new RuntimeException("StateManager network runner is null");
            }

            return _stateManagerRunner;
        }
    }

    static NetworkRunner _stateManagerRunner;    

    // Register a new state update with the given registration string, validate 
    // function, and perform function
    // Throws exception if registrationString is already found in _registeredUpdates
    public static void RegisterStateUpdate<TStateUpdate>(string registrationString, 
        Predicate<TStateUpdate> validateUpdate,
        Action<TStateUpdate> performUpdate,
        Action<NetworkRunner, PlayerRef, string, TStateUpdate> serverUpdateRPC,
        Action<NetworkRunner, string, TStateUpdate> clientUpdateRPC)
        where TStateUpdate : struct, IStateUpdate
    {
        if (_registeredUpdates.ContainsKey(registrationString))
            throw new RuntimeException(registrationString + " is already registered");

        StateUpdateRegistration newRegistration = new(validateUpdate, 
            performUpdate,
            serverUpdateRPC,
            clientUpdateRPC,
            typeof(TStateUpdate));
        _registeredUpdates[registrationString] = newRegistration;
    }

    // Initiates a state update with the given registration string and update
    // data; only called by clients
    // Throws exception if called on the server
    // Throws exception if registrationString was not previously registered
    // with RegisterStateUpdate 
    // Throws exception if updateData type doesn't match the registration corresponding
    // to registrationString
    public static void RequestStateUpdate<TStateUpdate>(string registrationString,
        TStateUpdate updateData)
        where TStateUpdate : struct, IStateUpdate
    {
        if (StateManagerRunner.GameMode == GameMode.Server)
            throw new RuntimeException("RequestStateUpdate called on server");

        StateUpdateRegistration registration = GetRegistration(registrationString);

        if (!registration.CheckUpdateType(updateData))
            throw new RuntimeException(
                "Type of updateData does not match type registered for " + registrationString);

        // Before sending request to server, client checks if update is valid
        if (!registration.ValidateUpdate(updateData))
        {
            Debug.Log("Requested state update of type " + registrationString + 
                " was not validated by the client");
            return;
        }

        registration.CallServerUpdateRPC(StateManagerRunner,
            registrationString,
            updateData);
    }

    // Should be called by the user-created server update RPC to initiate
    // state update on the server
    // Throws exception if registrationString was not previously registered
    // with RegisterStateUpdate; this should never happen as the registration
    // string is checked on the client before this, and the registrations 
    // should be identical on the server
    public static void UpdateServerState<TStateUpdate>(string registrationString,
        TStateUpdate updateData)
        where TStateUpdate : struct, IStateUpdate
    {
        StateUpdateRegistration registration = GetRegistration(registrationString);

        // Validate update again on the server, don't trust client
        if (!registration.ValidateUpdate(updateData))
        {
            Debug.Log("Requested state update of type " + registrationString +
                " was not validated by the server");
            return;
        }

        // Updates state on the server, only needed if this is a dedicated server
        // with no local player
        if (StateManagerRunner.GameMode == GameMode.Server)
        {
            Debug.Log("Performing state update on dedicated server");
            registration.PerformUpdate(updateData);
        }

        // Updates state on all clients
        Debug.Log("Sending state update to clients");
        registration.CallClientUpdateRPC(StateManagerRunner,
            registrationString,
            updateData);
    }

    // Should be called by the user-created client update RPC to initiate state
    // update on this client
    public static void UpdateClientState<TStateUpdate>(string registrationString,
        TStateUpdate updateData)
        where TStateUpdate : struct, IStateUpdate
    {
        StateUpdateRegistration registration = GetRegistration(registrationString);

        // Server must've already validated update, so no need to validate again
        Debug.Log("Performing state update on client");
        registration.PerformUpdate(updateData);
    }

    // Returns the registration corresponding to given registrationString
    // Throws exception if registrationString has not been registered
    static StateUpdateRegistration GetRegistration(string registrationString)
    {
        if (!_registeredUpdates.ContainsKey(registrationString))
            throw new RuntimeException(registrationString + " is not registered");

        return _registeredUpdates[registrationString];
    }
}