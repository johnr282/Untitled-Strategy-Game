using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Static class managing the status and movements of units within the
// game map; part of the global game state
// ------------------------------------------------------------------

public class UnitManager : SimulationBehaviour
{
    static Dictionary<UnitID, Unit> _units = new();

    static UnitObjectSpawner _unitObjectSpawner = null;

    static int _nextUnitID = 0;

    void Start()
    {
        _unitObjectSpawner = ProjectUtilities.FindUnitObjectSpawner();

        StateManager.RegisterStateUpdate<CreateUnitUpdate>(ValidateCreateUnitUpdate,
            CreateUnit,
            StateManagerRPCs.RPC_CreateUnitServer,
            StateManagerRPCs.RPC_CreateUnitClient);
    }

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
    static void CreateUnit(CreateUnitUpdate request)
    {
        GameTile initialTile = GameMap.GetTile(request.Location);

        Unit newUnit = new(request.Type,
            initialTile,
            GetNextUnitID());
        _units.Add(newUnit.UnitID, newUnit);
        initialTile.AddUnit(newUnit);

        Debug.Log("Created unit " + newUnit.UnitID);

        UnitObject newUnitObject = _unitObjectSpawner.SpawnUnitObject(newUnit.UnitID,
            request.RequestingPlayerID,
            request.Location);
        newUnit.UnitObject = newUnitObject;
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
    static bool ValidateCreateUnitUpdate(CreateUnitUpdate update)
    {
        GameTile initialTile = GameMap.GetTile(update.Location);

        if (!initialTile.Available(update.RequestingPlayerID))
            return false;

        return true;
    }

    // Returns whether the given MoveUnitRequest is valid
    // Throws an ArgumentException if no unit exists with the given ID or no
    // GameTile exists at the requested location
    public static bool ValidateMoveUnitRequest(MoveUnitRequest action)
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