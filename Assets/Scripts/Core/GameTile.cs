using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

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
    // Update this constant to match the number of Terrain types
    public const int TerrainTypeCount = 2;
    public const int MaxTileUnitCapacity = 12;

    public HexCoordinateOffset Hex { get; }
    public Terrain TileTerrain { get; set; }
    public ContinentID ContinentID { get; set; }
    public PlayerID OwnerID { get; private set; }
    public int Capacity { get; private set; }
    List<UnitID> _unitsOnTile = new();

    // Pass in -1 for continent ID if tile is not in a continent
    public GameTile(HexCoordinateOffset coordinateIn, 
        Terrain terrainIn, 
        ContinentID contintentIDIn)
    {
        Hex = coordinateIn;
        TileTerrain = terrainIn;
        ContinentID = contintentIDIn;
        OwnerID = new(-1);
        Capacity = 0;
    }

    // Returns true if this tile is in a continent, false otherwise
    public bool InContinent()
    {
        return ContinentID.ID != -1;
    }

    // Returns whether the given player ID either has or can claim ownership over
    // this tile
    public bool Available(PlayerID playerID)
    {
        return !Claimed() ||
            (OwnerID.ID == playerID.ID);
    }

    // Returns whether this tile has been claimed by a player
    public bool Claimed()
    {
        return OwnerID.ID != -1;
    }

    // Sets the owner of this tile to the given player ID
    public void Claim(PlayerID playerID)
    {
        OwnerID = playerID;
    }

    // Adds the given unit to this tile
    // Throws an ArgumentException if given a unit that already exists in this tile
    public void AddUnit(Unit unit)
    {
        _unitsOnTile.Add(unit.UnitID);
    }

    // Removes the given unit from this tile
    // Throws an ArgumentException if given unit doesn't exist in this tile
    public void RemoveUnit(Unit unit)
    {
        if (!_unitsOnTile.Remove(unit.UnitID))
            throw new ArgumentException("Unit does not exist in this tile");
    }

    // Returns the cost for the given unit to traverse into this tile from the
    // given start tile
    // Assumes the start tile is adjacent to this tile
    // Throws an ArgumentException if unitType is invalid or this tile's terrain
    // is invalid
    public int CostToTraverse(Unit unit, 
        GameTile startTile)
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