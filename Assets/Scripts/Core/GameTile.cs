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

    public HexCoordinateOffset Hex { get; }
    public Terrain TileTerrain { get; set; }
    public ContinentID ContinentID { get; set; }
    public PlayerID OwnerID { get; private set; }
    public int TileUnitCapacity { get; private set; }
    public int TileStructureCapacity { get; private set; }
    public int TotalUnitSize { get; private set; } // Sum of all unit sizes on this tile

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

        GameParameters gameParameters = ProjectUtilities.FindGameParameters();
        TileUnitCapacity = gameParameters.TileUnitCapacity;
        TileStructureCapacity = gameParameters.TileStructureCapacity;
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
            IsOwnedBy(playerID);
    }

    public bool IsOwnedBy(PlayerID playerID)
    {
        return OwnerID.ID == playerID.ID;
    }

    // Returns whether this tile has been claimed by a player
    public bool Claimed()
    {
        return OwnerID.ID != -1;
    }

    // Returns whether a unit of the given type would exceed capacity if added to this tile
    public bool ExceedsUnitCapacity(UnitType type)
    {
        UnitAttributes attributes = UnitManager.GetUnitAttributes(type);
        return TotalUnitSize + attributes.Size > TileUnitCapacity;
    }

    // Adds the given unit to this tile
    // Throws an ArgumentException if given a unit that already exists in this tile
    // or if this tile's owner is different from the given unit's owner
    // Throws a RuntimeException if the given unit's size exceeds this tile's capacity
    public void AddUnit(Unit unit)
    {
        if (_unitsOnTile.Contains(unit.UnitID))
            throw new ArgumentException("Unit " + unit.UnitID + " already exists in this tile");

        if (!Available(unit.OwnerID))
            throw new ArgumentException("Unit's owner, " + unit.OwnerID + 
                ", is different from tile's owner, " + OwnerID);

        if (ExceedsUnitCapacity(unit.Type))
            throw new RuntimeException("Unit " + unit.UnitID + " exceeds tile unit capacity");

        _unitsOnTile.Add(unit.UnitID);
        TotalUnitSize += unit.Attributes.Size;
        Claim(unit.OwnerID);
    }

    // Removes the given unit from this tile
    // Throws an ArgumentException if given unit doesn't exist in this tile
    public void RemoveUnit(Unit unit)
    {
        if (!_unitsOnTile.Remove(unit.UnitID))
            throw new ArgumentException("Unit " + unit.UnitID + " does not exist in this tile");

        TotalUnitSize -= unit.Attributes.Size;
        if (_unitsOnTile.Count == 0)
            Unclaim();
    }

    // Returns the number of units on this tile
    public int GetNumUnits() 
    { 
        return _unitsOnTile.Count;
    }

    // Returns the cost for the given unit to traverse into this tile from the
    // given start tile
    // Assumes the start tile is adjacent to this tile
    // Throws an ArgumentException if unitType is invalid or this tile's terrain
    // is invalid
    public int CostToTraverse(Unit unit, 
        GameTile startTile)
    {
        if (!Available(unit.OwnerID))
            return GameMap.ImpassableCost;

        string invalidTerrainMsg = "TileTerrain of GameTile not valid";

        switch (unit.Attributes.Category)
        {
            case UnitCategory.Land:
                switch (TileTerrain)
                {
                    case Terrain.sea:
                        return GameMap.ImpassableCost;

                    case Terrain.land:
                        return 1;

                    default:
                        throw new RuntimeException(invalidTerrainMsg);
                }

            case UnitCategory.Naval:
                switch (TileTerrain)
                {
                    case Terrain.sea:
                        return 1;

                    case Terrain.land:
                        return GameMap.ImpassableCost;

                    default:
                        throw new RuntimeException(invalidTerrainMsg);
                }

            case UnitCategory.Air:
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

    // Sets the owner of this tile to the given player ID
    void Claim(PlayerID playerID)
    {
        OwnerID = playerID;
    }

    // Sets this tile to be unclaimed
    void Unclaim()
    {
        OwnerID = new(-1);
    }
}