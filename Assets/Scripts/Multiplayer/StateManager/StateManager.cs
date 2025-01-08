using DG.Tweening;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : NetworkBehaviour
{
    class StateUpdateRegistration
    {
        readonly Delegate _validateUpdateFunc;
        readonly Delegate _performUpdateFunc;
        readonly Delegate _serverUpdateRPC;
        readonly Delegate _clientUpdateRPC;

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
            IStateUpdate updateData)
        {
            ClientRPCManager.QueueClientRPC(_serverUpdateRPC,
                runner,
                PlayerRef.None,
                updateData);
        }

        public void CallClientUpdateRPC(NetworkRunner runner,
            IStateUpdate updateData)
        {
            _clientUpdateRPC.DynamicInvoke(runner,
                updateData);
        }
    }

    static Dictionary<Type, StateUpdateRegistration> _registeredUpdates = new();

    static NetworkRunner StateManagerRunner
    {
        get
        {
            if (_stateManagerRunner == null)
            {
                _stateManagerRunner = _stateManagerNetworkObject.Runner ??
                    throw new RuntimeException("StateManager network runner is null");
            }

            return _stateManagerRunner;
        }
    }

    static NetworkRunner _stateManagerRunner;
    static NetworkObject _stateManagerNetworkObject;

    void Start()
    {
        // Finding network object first prevents strange bug where StateManager object 
        // couldn't be found on a random client
        _stateManagerNetworkObject = ProjectUtilities.FindManagersNetworkObject();
    }

    // Register a new state update with the given validate function, perform function,
    // and RPCs
    // Throws exception if update type is already found in _registeredUpdates
    public static void RegisterStateUpdate<TStateUpdate>(Predicate<TStateUpdate> validateUpdate,
        Action<TStateUpdate> performUpdate,
        Action<NetworkRunner, PlayerRef, TStateUpdate> serverUpdateRPC,
        Action<NetworkRunner, TStateUpdate> clientUpdateRPC)
        where TStateUpdate : struct, IStateUpdate
    {
        Type updateType = typeof(TStateUpdate);
        if (_registeredUpdates.ContainsKey(updateType))
            throw new RuntimeException(updateType + " is already registered");

        Debug.Log("Registering " + updateType);
        StateUpdateRegistration newRegistration = new(validateUpdate, 
            performUpdate,
            serverUpdateRPC,
            clientUpdateRPC,
            updateType);
        _registeredUpdates[updateType] = newRegistration;
    }

    // Initiates a state update with the given update data; only called by clients
    // If the localUpdateOnly flag is set to true, the update will not be sent to
    // the server and will only be executed on the local client; set this flag when
    // you are requesting a state update as part of another state update function
    // 
    // Returns true if the update was successfully validated and sent to the server
    // Returns false if the update was not validated
    // Throws exception if called on the server
    // Throws exception if update type was not previously registered
    // with RegisterStateUpdate 
    public static bool RequestStateUpdate<TStateUpdate>(TStateUpdate updateData, 
        bool localUpdateOnly = false)
        where TStateUpdate : struct, IStateUpdate
    {
        Type updateType = typeof(TStateUpdate);
        if (StateManagerRunner.GameMode == GameMode.Server)
            throw new RuntimeException("RequestStateUpdate for " + updateType + " called on server");

        StateUpdateRegistration registration = GetRegistration(updateType);

        // Before sending request to server, client checks if update is valid
        if (!registration.ValidateUpdate(updateData))
        {
            Debug.Log(updateType + " request was not validated by the client");
            return false;
        }

        if (localUpdateOnly)
        {
            Debug.Log("Performing " + updateType + " only on local client");
            registration.PerformUpdate(updateData);
            return true;
        }

        Debug.Log("Sending " + updateType + " request to server");
        registration.CallServerUpdateRPC(StateManagerRunner, updateData);
        return true;
    }

    // Should be called by the user-created server update RPC to initiate
    // state update on the server
    // Throws exception if TStateUpdate was not previously registered
    // with RegisterStateUpdate; this should never happen as the update type
    // is checked on the client before this, and the registrations 
    // should be identical on the server
    public static void UpdateServerState<TStateUpdate>(TStateUpdate updateData)
        where TStateUpdate : struct, IStateUpdate
    {
        Type updateType = typeof(TStateUpdate);
        StateUpdateRegistration registration = GetRegistration(updateType);

        // Validate update again on the server, don't trust client
        if (!registration.ValidateUpdate(updateData))
        {
            Debug.Log(updateType + " request was not validated by the server");
            return;
        }

        // Updates state on the server, only needed if this is a dedicated server
        // with no local player
        if (StateManagerRunner.GameMode == GameMode.Server)
        {
            Debug.Log("Performing " + updateType + " on dedicated server");
            registration.PerformUpdate(updateData);
        }

        // Updates state on all clients
        Debug.Log("Sending " + updateType + " to clients");
        registration.CallClientUpdateRPC(StateManagerRunner, updateData);
    }

    // Should be called by the user-created client update RPC to initiate state
    // update on this client
    public static void UpdateClientState<TStateUpdate>(TStateUpdate updateData)
        where TStateUpdate : struct, IStateUpdate
    {
        Type updateType = typeof(TStateUpdate);
        StateUpdateRegistration registration = GetRegistration(updateType);

        // Server must've already validated update, so no need to validate again
        Debug.Log("Performing " + updateType + " on client");
        registration.PerformUpdate(updateData);
    }

    // Returns the registration corresponding to given update type
    // Throws exception if updateType has not been registered
    static StateUpdateRegistration GetRegistration(Type updateType)
    {
        if (!_registeredUpdates.ContainsKey(updateType))
            throw new RuntimeException(updateType + " is not registered");

        return _registeredUpdates[updateType];
    }
}