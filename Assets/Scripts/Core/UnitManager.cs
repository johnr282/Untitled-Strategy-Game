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

    void Start()
    {
        _gameMap = ProjectUtilities.FindGameMap();
        _playerManager = ProjectUtilities.FindPlayerManager();
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

    // If given request is valid, creates a new unit and puts it into the newUnit
    // parameter; returns false if given request is invalid
    public bool TryCreateUnit(CreateUnitRequest request,
        out Unit newUnit)
    {
        HexCoordinateOffset initialHex =
            HexUtilities.ConvertToHexCoordinateOffset(request.Location);

        if (!_gameMap.FindTile(initialHex, out GameTile initialTile) ||
            !initialTile.Available(request.RequestingPlayerID))
        {
            newUnit = null;
            return false;
        }

        newUnit = new(request.Type, initialTile);
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