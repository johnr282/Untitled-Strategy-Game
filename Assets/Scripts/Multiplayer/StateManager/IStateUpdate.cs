using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains IStateUpdate, the base interface for all game state updates
// ------------------------------------------------------------------

public interface IStateUpdate : INetworkStruct 
{
    // Returns a list of updates composing this update; must include this
    // or this update won't be applied
    // Allows state updates to be composed of multiple updates, each
    // of which are validated and applied by the StateManager, and defines the
    // order in which each update is applied
    public List<IStateUpdate> GetStateUpdatesInOrder();
}