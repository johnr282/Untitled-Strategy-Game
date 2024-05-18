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
    Dictionary<int, Unit> _units = new();
    int _nextUnitID = -1;

    void Start()
    {
        _gameMap = ProjectUtilities.FindGameMap();
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
            newUnit = default;
            return false;
        }

        newUnit = new(request.Type, 
            initialTile, 
            GetNextUnitID());
        _units[newUnit.UnitID] = newUnit;
        initialTile.AddUnit(newUnit);

        return true;
    }

    // Returns a unique ID for each new unit
    int GetNextUnitID()
    {
        _nextUnitID++;
        return _nextUnitID;
    }

    // Returns the unit corresponding to the given unit ID
    // Throws an ArgumentException if given ID has no corresponding unit
    public Unit FindUnit(int unitID)
    {
        if (!_units.TryGetValue(unitID, out Unit unit))
            throw new ArgumentException("No unit exists with the given unit ID");

        return unit;
    }

    // Attempts to move the given unit from its current position to the GameTile
    // corresponding to requestedPos
    // If the unit was successfully moved, returns true and populates the pathTaken
    // parameter; returns false otherwise
    // Throws an ArgumentException if no unit exists with the given ID or no GameTile
    // corresponding to requestedHex exists
    public bool TryMoveUnit(int unitID, 
        HexCoordinateOffset requestedHex, 
        out List<HexCoordinateOffset> pathTaken)
    {
        if (!_gameMap.FindTile(requestedHex, out GameTile requestedTile))
            throw new ArgumentException("Requested to move unit to invalid tile");

        Unit unit = FindUnit(unitID);

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

        MoveUnit(unit, requestedTile);
        return true;
    }

    // Moves the given unit to the given new tile
    public void MoveUnit(Unit unit,
        GameTile newTile)
    {
        GameTile oldTile = unit.CurrentLocation;
        oldTile.RemoveUnit(unit);
        newTile.AddUnit(unit);
        unit.CurrentLocation = newTile;
    }
}