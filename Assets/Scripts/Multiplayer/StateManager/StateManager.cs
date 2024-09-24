using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    // Need to define interface to allow registeredUpdates dictionary to 
    // contain StateUpdateRegistration structs with different generic types
    interface IStateUpdateRegistration { }

    struct StateUpdateRegistration<TStateUpdate> : IStateUpdateRegistration
        where TStateUpdate : IGameStateUpdate
    {
        public Predicate<TStateUpdate> ValidateUpdate { get; }
        public Action<TStateUpdate> PerformUpdate { get; }

        public StateUpdateRegistration(Predicate<TStateUpdate> validateUpdateIn,
            Action<TStateUpdate> performUpdateIn)
        {
            ValidateUpdate = validateUpdateIn;
            PerformUpdate = performUpdateIn;
        }

    }

    struct StateUpdateEvent
    {
        public string UpdateRegistrationString { get; }

        public StateUpdateEvent(string updateRegistrationStringIn)
        {
            UpdateRegistrationString = updateRegistrationStringIn;
        }
    }

    Dictionary<string, IStateUpdateRegistration> registeredUpdates;

    void Start()
    {
        RegisterUpdates();
    }

    void RegisterUpdates()
    {

    }

    void RegisterStateUpdate<TStateUpdate>(string registrationString, 
        Predicate<TStateUpdate> validateUpdate,
        Action<TStateUpdate> performUpdate)
        where TStateUpdate : IGameStateUpdate
    {
        StateUpdateRegistration<TStateUpdate> newRegistration = new(validateUpdate, performUpdate);
        registeredUpdates[registrationString] = newRegistration;
    }

}
