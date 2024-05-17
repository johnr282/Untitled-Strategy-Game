using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Class representing a single hexagonal tile in the game map; not
// used for visual rendering
// ------------------------------------------------------------------

public enum Terrain
{
    sea,
    land
}

public class GameTile
{
    // Location of tile in game map
    public HexCoordinateOffset Hex { get; }

    public Terrain TileTerrain { get; set; }

    // ID of this tile's continent; -1 if not part of any continent
    public int ContinentID { get; set; }

    // Player ID of this tile's current owner; -1 if not owned by any player
    public int OwnerID { get; set; }

    // Map from unit IDs to units of all the units currently on this tile
    Dictionary<int, Unit> _unitsOnTile = new();

    public int Capacity { get; private set; }

    // Constructor
    public GameTile(HexCoordinateOffset coordinateIn, 
        Terrain terrainIn, 
        int contintentIDIn = -1, 
        int ownerIDIn = -1)
    {
        Hex = coordinateIn;
        TileTerrain = terrainIn;
        ContinentID = contintentIDIn;
        OwnerID = ownerIDIn;
    }

    // Returns true if this tile is in a continent, false otherwise
    public bool InContinent()
    {
        return ContinentID != -1;
    }

    // Returns whether the given player ID either has or can claim ownership over
    // this tile
    public bool Available(int playerID)
    {
        bool unclaimedTile = OwnerID == -1;
        return unclaimedTile ||
            (OwnerID == playerID);
    }

    // Sets the owner of this tile to the given player ID
    public void Claim(int playerID)
    {
        OwnerID = playerID;
    }

    // Adds the given unit to this tile
    // Throws an ArgumentException if given a unit that already exists in this tile
    public void AddUnit(Unit unit)
    {
        _unitsOnTile.Add(unit.UnitID, unit);
    }

    // Removes the given unit from this tile
    // Throws an ArgumentException if given unit doesn't exist in this tile
    public void RemoveUnit(Unit unit)
    {
        if (!_unitsOnTile.Remove(unit.UnitID))
            throw new ArgumentException("Unit does not exist in this tile");
    }

    // Returns the cost for a unit of the given UnitType to traverse into this
    // tile from the given adjacent start tile
    // Start is null and ignored by default; if start is given, assumes it is
    // adjacent to this tile
    // Throws an ArgumentException if unitType is invalid or this tile's terrain
    // is invalid
    public int CostToTraverse(Unit unit, 
        GameTile start = null)
    {
        string invalidTerrainMsg = "TileTerrain of GameTile not valid";

        switch (unit.Type)
        {
            case UnitType.land:
                switch (TileTerrain)
                {
                    case Terrain.sea:
                        return GameMap.ImpassableCost;

                    case Terrain.land:
                        return 1;

                    default:
                        throw new RuntimeException(invalidTerrainMsg);
                }

            case UnitType.naval:
                switch (TileTerrain)
                {
                    case Terrain.sea:
                        return 1;

                    case Terrain.land:
                        return GameMap.ImpassableCost;

                    default:
                        throw new RuntimeException(invalidTerrainMsg);
                }

            case UnitType.air:
                switch (TileTerrain)
                {
                    case Terrain.sea:
                        return 1;

                    case Terrain.land:
                        return 1;

                    default:
                        throw new RuntimeException(invalidTerrainMsg);
                }

            default:
                throw new ArgumentException("Invalid unit type");
        }
    }
}