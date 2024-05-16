using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component managing the status and movements of units within the game
// map
// ------------------------------------------------------------------

public class UnitManager : NetworkBehaviour
{
    GameMap _gameMap;
    PlayerManager _playerManager;

    Subscription<CreateUnitRequestEvent> _createUnitRequestSub;

    void Start()
    {
        _gameMap = ProjectUtilities.FindGameMap();
        _playerManager = ProjectUtilities.FindPlayerManager();

        _createUnitRequestSub =
            EventBus.Subscribe<CreateUnitRequestEvent>(OnCreateUnitRequest);
    }

    // Creates and returns a unit with the given type and initial location
    // Throws an ArgumentException if unit could not be created
    public Unit CreateUnit(UnitType unitType,
        HexCoordinateOffset initialLocation)
    {
        if (!_gameMap.FindTile(initialLocation, out GameTile initialTile))
            throw new ArgumentException("Attempted to create unit at invalid tile");

        return new Unit(unitType, initialTile);
    }

    void OnCreateUnitRequest(CreateUnitRequestEvent createUnitRequestEvent)
    {
        Debug.Log("Received create unit request for player " +
            createUnitRequestEvent.Request.RequestingPlayerID.ToString() +
            " at " + createUnitRequestEvent.Request.Location.ToString());
        bool success = ValidCreateUnitRequest(createUnitRequestEvent.Request, 
            out PlayerRef playerRef);
        Debug.Log("Request success: " + success.ToString());
        ServerMessages.RPC_CreateUnitResponse(Runner,
            playerRef,
            createUnitRequestEvent.Request,
            success);
    }

    // Returns whether the given create unit request is able to be completed;
    // if the request is valid, puts the corresponding PlayerRef into playerRef
    bool ValidCreateUnitRequest(CreateUnitRequest request, 
        out PlayerRef playerRef)
    {
        playerRef = PlayerRef.None;
        HexCoordinateOffset initialHex = 
            HexUtilities.ConvertToHexCoordinateOffset(request.Location);

        if (!_gameMap.FindTile(initialHex, out GameTile initialTile) ||
            !initialTile.Available(request.RequestingPlayerID))
            return false;

        try
        {
            playerRef = _playerManager.GetPlayerRef(request.RequestingPlayerID);
        }
        catch (ArgumentException e)
        {
            Debug.LogError(e.Message);
            return false;
        }

        return true;
    }


    // Attempts to move the given unit from its current position to the GameTile
    // corresponding to requestedPos
    // If the unit was successfully moved, returns true and populates the pathTaken
    // parameter; returns false otherwise
    // Throws an ArgumentException if no GameTile corresponding to requestedHex 
    // exists
    public bool TryMoveUnit(Unit unit, 
        HexCoordinateOffset requestedHex, 
        out List<HexCoordinateOffset> pathTaken)
    {
        if (!_gameMap.FindTile(requestedHex, out GameTile requestedTile))
            throw new ArgumentException("Requested to move unit to invalid tile");

        List<GameTile> path;
        try
        {
            path = _gameMap.FindPath(unit,
                unit.CurrentLocation,
                requestedTile);
        }
        catch (RuntimeException)
        {
            pathTaken = null;
            return false;
        }

        pathTaken = new();
        foreach (GameTile tile in path)
        {
            pathTaken.Add(tile.Hex);
        }

        unit.Move(requestedTile);
        return true;
    }
}