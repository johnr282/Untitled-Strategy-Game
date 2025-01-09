using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component managing the status and movements of units within the
// game map; part of the global game state
// ------------------------------------------------------------------

public class UnitManager : SimulationBehaviour
{
    // Will eventually be loaded in from data files
    static Dictionary<UnitType, UnitAttributes> _unitTypeAttributes = new()
    {
        { UnitType.Infantry, new UnitAttributes(1, 1, 1, 1, 1, UnitCategory.Land) }
    };

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

    // Returns the unit attributes corresponding to the given unit type
    // Throws an ArgumentException if given type has no corresponding attributes
    public static UnitAttributes GetUnitAttributes(UnitType type)
    {
        if (!_unitTypeAttributes.TryGetValue(type, out UnitAttributes attributes))
            throw new ArgumentException("No attributes exist for the given unit type");

        return attributes;
    }

    // Creates a new unit according to the given update
    // Throws an ArgumentException if no GameTile exists at the update location
    static void CreateUnit(CreateUnitUpdate update)
    {
        GameTile initialTile = GameMap.GetTile(update.Location);

        Unit newUnit = new(update.Type,
            GetNextUnitID(),
            initialTile,
            update.RequestingPlayerID);
        _units.Add(newUnit.UnitID, newUnit);
        
        Debug.Log("Created unit " + newUnit.UnitID);

        UnitObject newUnitObject = _unitObjectSpawner.SpawnUnitObject(newUnit.UnitID,
            update.RequestingPlayerID,
            update.Location);
        newUnit.UnitObject = newUnitObject;
        initialTile.AddUnit(newUnit); // This needs to be after spawning unit object for stacking
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
    static bool ValidateCreateUnitUpdate(CreateUnitUpdate update, 
        out string failureReason)
    {
        GameTile initialTile = GameMap.GetTile(update.Location);

        if (!initialTile.Available(update.RequestingPlayerID))
        {
            failureReason = "Tile not available for requesting player";
            return false;
        }

        if (initialTile.ExceedsUnitCapacity(update.Type))
        {
            failureReason = "Unit would exceed tile unit capacity";
            return false;
        }

        failureReason = "";
        return true;
    }

    // Returns whether the given MoveUnitRequest is valid
    // Throws an ArgumentException if no unit exists with the given ID, no
    // GameTile exists at the requested location, or the RequestingPlayerID
    // is different from the unit's owner
    static bool ValidateMoveUnitUpdate(MoveUnitUpdate update, 
        out string failureReason)
    {
        GameTile requestedTile = GameMap.GetTile(update.NewLocation);
        Unit unit = GetUnit(update.UnitID);

        if (unit.OwnerID.ID != update.RequestingPlayerID.ID)
            throw new RuntimeException("Player " + update.RequestingPlayerID + 
                " requested to move unit owned by Player " + unit.OwnerID);

        try
        {
            GameMap.FindPath(unit,
                requestedTile);
        }
        catch (RuntimeException)
        {
            failureReason = "Couldn't find a valid path";
            return false;
        }

        failureReason = "";
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

// Creates a unit
public readonly struct CreateUnitUpdate : IStateUpdate
{
    public UnitType Type { get; }
    public HexCoordinateOffset Location { get; }
    public PlayerID RequestingPlayerID { get; }

    public CreateUnitUpdate(UnitType typeIn,
        HexCoordinateOffset locationIn,
        PlayerID requestingPlayerIDIn)
    {
        Type = typeIn;
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }

    public List<IStateUpdate> GetStateUpdatesInOrder() => new List<IStateUpdate> { this };
}

// Moves a unit
public readonly struct MoveUnitUpdate : IStateUpdate
{
    public UnitID UnitID { get; }
    public HexCoordinateOffset NewLocation { get; }
    public PlayerID RequestingPlayerID { get; }

    public MoveUnitUpdate(UnitID unitIDIn,
        HexCoordinateOffset newLocationIn,
        PlayerID requestingPlayerIDIn)
    {
        UnitID = unitIDIn;
        NewLocation = newLocationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }

    public List<IStateUpdate> GetStateUpdatesInOrder() => new List<IStateUpdate> { this };
}