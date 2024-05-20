using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component managing the status and movements of units within the game
// map
// ------------------------------------------------------------------

[RequireComponent(typeof(GameMap))]
public class UnitManager : NetworkBehaviour
{
    const int MaxUnits = GameTile.MaxTileUnitCapacity * 
        GameMap.MaxWidth * GameMap.MaxHeight;

    [Networked, Capacity(MaxUnits)]
    NetworkDictionary<int, Unit> Units { get; }

    GameMap _gameMap;
    int _nextUnitID = -1;

    void Start()
    {
        _gameMap = GetComponent<GameMap>();
    }

    // If given request is valid, creates a new unit and puts it into the newUnit
    // parameter; returns false if given request is invalid
    // Throws a RuntimeException if there are MaxUnits units
    public bool TryCreateUnit(CreateUnitRequest request,
        out Unit newUnit)
    {
        if (Units.Count >= MaxUnits)
            throw new RuntimeException("Exceeding maximum number of units");

        GameTile initialTile = _gameMap.GetTile(request.Location);

        if (!initialTile.Available(request.RequestingPlayerID))
        {
            newUnit = default;
            return false;
        }

        newUnit = new(request.Type, 
            initialTile, 
            GetNextUnitID());
        Units.Add(newUnit.UnitID, newUnit);
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
    public Unit GetUnit(int unitID)
    {
        if (!Units.TryGet(unitID, out Unit unit))
            throw new ArgumentException("No unit exists with the given unit ID");

        return Units.Get(unitID);
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
        GameTile requestedTile = _gameMap.GetTile(requestedHex);
        Unit unit = GetUnit(unitID);

        List<GameTile> path;
        try
        {
            path = _gameMap.FindPath(unit,
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