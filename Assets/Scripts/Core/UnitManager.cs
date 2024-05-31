using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Static class managing the status and movements of units within the
// game map; part of the global game state
// ------------------------------------------------------------------

public static class UnitManager
{
    static Dictionary<UnitID, Unit> _units = new();

    static int _nextUnitID = 0;

    // Returns the unit corresponding to the given unit ID
    // Throws an ArgumentException if given ID has no corresponding unit
    public static Unit GetUnit(UnitID unitID)
    {
        if (!_units.TryGetValue(unitID, out Unit unit))
            throw new ArgumentException("No unit exists with the given unit ID");

        return unit;
    }

    // Creates a new unit according to the given request and returns the unit ID
    // of the new unit
    // Throws an ArgumentException if no GameTile exists at the request location
    public static UnitID CreateUnit(CreateUnitRequest request)
    {
        GameTile initialTile = GameMap.GetTile(request.Location);

        Unit newUnit = new(request.Type,
            initialTile,
            GetNextUnitID());
        _units.Add(newUnit.UnitID, newUnit);
        initialTile.AddUnit(newUnit);

        return newUnit.UnitID;
    }

    // Moves the unit corresponding to the given unit ID to the given new tile
    public static void MoveUnit(UnitID unitID,
        GameTile newTile)
    {
        Unit unit = GetUnit(unitID);
        GameTile oldTile = unit.CurrentLocation;
        oldTile.RemoveUnit(unit);
        newTile.AddUnit(unit);
        unit.CurrentLocation = newTile;
    }

    // Returns whether the given CreateUnitRequest is valid
    // Throws an ArgumentException if no GameTile exists at the requested
    // location
    public static bool ValidateCreateUnitAction(CreateUnitRequest action)
    {
        GameTile initialTile = GameMap.GetTile(action.Location);

        if (!initialTile.Available(action.RequestingPlayerID))
            return false;

        return true;
    }

    // Returns whether the given MoveUnitRequest is valid
    // Throws an ArgumentException if no unit exists with the given ID or no
    // GameTile exists at the requested location
    public static bool ValidateMoveUnitAction(MoveUnitRequest action)
    {
        GameTile requestedTile = GameMap.GetTile(action.Location);
        Unit unit = GetUnit(action.UnitID);

        try
        {
            GameMap.FindPath(unit,
                requestedTile);
        }
        catch (RuntimeException)
        {
            return false;
        }

        return true;
    }

    static UnitID GetNextUnitID()
    {
        if (_nextUnitID >= ushort.MaxValue)
            throw new RuntimeException("Ran out of unit IDs");

        UnitID nextID = new(_nextUnitID);
        _nextUnitID++;
        return nextID;
    }
}