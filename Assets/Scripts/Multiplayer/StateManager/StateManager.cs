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
            Delegate clientUpdateRPCIn)
        {
            _validateUpdateFunc = validateUpdateFuncIn;
            _performUpdateFunc = performUpdateFuncIn;
            _serverUpdateRPC = serverUpdateRPCIn;
            _clientUpdateRPC = clientUpdateRPCIn;
        }

        public bool ValidateUpdate(IStateUpdate update, out string failureReason)
        {
            failureReason = "";

            // Can't just directly pass parameters, need to invoke like this in order
            // to extract updated out parameter failureReason
            object[] args = { update, failureReason };
            bool validated = (bool)_validateUpdateFunc.DynamicInvoke(args);
            failureReason = (string)args[1];
            return validated;
        }

        public void PerformUpdate(IStateUpdate update)
        {
            _performUpdateFunc.DynamicInvoke(update);
        }

        // Server update RPC is called on the client, so we need to go through
        // the ClientRPCManager
        public void CallServerUpdateRPC(NetworkRunner runner,
            IStateUpdate update)
        {
            ClientRPCManager.QueueClientRPC(_serverUpdateRPC,
                runner,
                PlayerRef.None,
                update);
        }

        public void CallClientUpdateRPC(NetworkRunner runner,
            IStateUpdate update)
        {
            _clientUpdateRPC.DynamicInvoke(runner,
                update);
        }
    }

    public delegate bool Validator<T>(T obj, out string failureReason);

    // Default validator that always returns true
    // Can be used for convenience for state updates that don't require validation
    public static bool DefaultValidator<T>(T obj, out string failureReason)
    {
        failureReason = "";
        return true;
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
    public static void RegisterStateUpdate<TStateUpdate>(Validator<TStateUpdate> validateUpdate,
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
            clientUpdateRPC);
        _registeredUpdates[updateType] = newRegistration;
    }

    // Initiates a state update with the given update data; only called by clients
    // Returns true if the update was successfully validated and sent to the server
    // Returns false if the update was not validated
    // Throws RuntimeException if called on the server
    // Throws RuntimeException if update type was not previously registered
    // with RegisterStateUpdate 
    public static bool RequestStateUpdate<TStateUpdate>(TStateUpdate update)
        where TStateUpdate : struct, IStateUpdate
    {
        Type updateType = typeof(TStateUpdate);
        if (StateManagerRunner.GameMode == GameMode.Server)
            throw new RuntimeException("RequestStateUpdate for " + updateType + " called on server");

        // Before sending request to server, client checks if update is valid
        if (!ValidateUpdate(update))
            return false;

        Debug.Log("Sending " + updateType + " request to server");
        StateUpdateRegistration registration = GetRegistration(updateType);
        registration.CallServerUpdateRPC(StateManagerRunner, update);
        return true;
    }

    // Should be called by the user-created server update RPC to initiate
    // state update on the server
    // Throws exception if TStateUpdate was not previously registered
    // with RegisterStateUpdate; this should never happen as the update type
    // is checked on the client before this, and the registrations 
    // should be identical on the server
    public static void UpdateServerState<TStateUpdate>(TStateUpdate update)
        where TStateUpdate : struct, IStateUpdate
    {
        // Validate update again on the server, don't trust client
        if (!ValidateUpdate(update))
            return;

        // Updates state on the server, only needed if this is a dedicated server
        // with no local player
        Type updateType = typeof(TStateUpdate);
        if (StateManagerRunner.GameMode == GameMode.Server)
        {
            Debug.Log("Performing " + updateType + " on dedicated server");
            PerformUpdate(update);
        }

        // Updates state on all clients
        Debug.Log("Sending " + updateType + " to clients");
        StateUpdateRegistration registration = GetRegistration(updateType);
        registration.CallClientUpdateRPC(StateManagerRunner, update);
    }

    // Should be called by the user-created client update RPC to initiate state
    // update on this client
    public static void UpdateClientState<TStateUpdate>(TStateUpdate update)
        where TStateUpdate : struct, IStateUpdate
    {
        // Server must've already validated update, so no need to validate again
        Type updateType = typeof(TStateUpdate);
        Debug.Log("Performing " + updateType + " on client");
        PerformUpdate(update);
    }

    // Returns the registration corresponding to given update type
    // Throws exception if updateType has not been registered
    static StateUpdateRegistration GetRegistration(Type updateType)
    {
        if (!_registeredUpdates.ContainsKey(updateType))
            throw new RuntimeException(updateType + " is not registered");

        return _registeredUpdates[updateType];
    }

    // Validates the given update and all of its child updates
    static bool ValidateUpdate<TStateUpdate>(TStateUpdate update)
        where TStateUpdate : struct, IStateUpdate
    {
        List<IStateUpdate> childUpdates = update.GetStateUpdatesInOrder();
        foreach (IStateUpdate childUpdate in childUpdates)
        {
            if (!ValidateUpdateHelper(childUpdate))
                return false;
        }

        return true;
    }

    // Validates only the given update
    static bool ValidateUpdateHelper(IStateUpdate update)
    {
        Type updateType = update.GetType();
        StateUpdateRegistration registration = GetRegistration(updateType);
        if (!registration.ValidateUpdate(update, out string failureReason))
        {
            Debug.Log(updateType + " request was not validated; reason: " + failureReason);
            return false;
        }

        return true;
    }

    // Performs the given update and all of its child updates
    static void PerformUpdate<TStateUpdate>(TStateUpdate update)
        where TStateUpdate : struct, IStateUpdate
    {
        Type updateType = typeof(TStateUpdate);
        List<IStateUpdate> childUpdates = update.GetStateUpdatesInOrder();
        foreach (IStateUpdate childUpdate in childUpdates)
        {
            Type childUpdateType = childUpdate.GetType();
            // Need to validate again in case previous update invalidated this one
            if (!ValidateUpdateHelper(childUpdate))
            {
                Debug.Log($"{childUpdateType} as part of {updateType} was invalidated by previous update");
                return;
            }
            Debug.Log($"Performing {childUpdateType} as part of {updateType}");
            PerformUpdateHelper(childUpdate);
        }
    }

    // Performs only the given update, not its children
    static void PerformUpdateHelper(IStateUpdate update)
    {
        Type updateType = update.GetType();
        StateUpdateRegistration registration = GetRegistration(updateType);
        registration.PerformUpdate(update);
    }
}