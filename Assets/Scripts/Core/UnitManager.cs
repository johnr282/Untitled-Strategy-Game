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
        
        StateManager.RegisterStateUpdate<MoveUnitUpdate>(ValidateMoveUnitUpdate,
            MoveUnit,
            StateManagerRPCs.RPC_MoveUnitServer,
            StateManagerRPCs.RPC_MoveUnitClient);
    }

    // Returns the unit corresponding to the given unit ID
    // Throws an ArgumentException if given ID has no corresponding unit
    public static Unit GetUnit(UnitID unitID)
    {
        if (!_units.TryGetValue(unitID, out Unit unit))
            throw new ArgumentException("No unit exists with the given unit ID");

        return unit;
    }

    // Creates a new unit according to the given update and returns the unit ID
    // of the new unit
    // Throws an ArgumentException if no GameTile exists at the update location
    static void CreateUnit(CreateUnitUpdate update)
    {
        GameTile initialTile = GameMap.GetTile(update.Location);

        Unit newUnit = new(update.Type,
            initialTile,
            GetNextUnitID());
        _units.Add(newUnit.UnitID, newUnit);
        initialTile.AddUnit(newUnit);

        Debug.Log("Created unit " + newUnit.UnitID);

        UnitObject newUnitObject = _unitObjectSpawner.SpawnUnitObject(newUnit.UnitID,
            update.RequestingPlayerID,
            update.Location);
        newUnit.UnitObject = newUnitObject;
    }

    // Moves the unit corresponding to the given unit ID to the given new tile
    // Moves the corresponding UnitObject as well
    static void MoveUnit(MoveUnitUpdate update)
    {
        Debug.Log("Moving unit " + update.UnitID);

        // Need to move UnitObject before updating state
        Unit unitToMove = GetUnit(update.UnitID);
        unitToMove.UnitObject.MoveTo(update.NewLocation);

        GameTile oldTile = unitToMove.CurrentLocation;
        oldTile.RemoveUnit(unitToMove);
        GameTile destTile = GameMap.GetTile(update.NewLocation);
        destTile.AddUnit(unitToMove);
        unitToMove.CurrentLocation = destTile;
    }

    // Returns whether the given CreateUnitUpdate is valid
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
    static bool ValidateMoveUnitUpdate(MoveUnitUpdate update)
    {
        GameTile requestedTile = GameMap.GetTile(update.NewLocation);
        Unit unit = GetUnit(update.UnitID);

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