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

public enum UnitType
{ 
    land, 
    naval,
    air
}


public class GameTile
{
    // Location of tile in game map
    public HexCoordinateOffset Coordinate { get; }

    public Terrain TileTerrain { get; set; }

    public int ContinentID { get; set; }

    // Constructor
    public GameTile(HexCoordinateOffset coordinateIn, 
        Terrain terrainIn, 
        int contintentIDIn = -1)
    {
        Coordinate = coordinateIn;
        TileTerrain = terrainIn;
        ContinentID = contintentIDIn;
    }

    // Returns true if this tile is in a continent, false otherwise
    public bool InContinent()
    {
        return ContinentID != -1;
    }

    // Returns the cost for a unit of the given UnitType to traverse into this
    // tile from the given adjacent start tile
    // Start is null and ignored by default; if start is given, assumes it is
    // adjacent to this tile
    // Throws an ArgumentException if unitType is invalid or this tile's terrain
    // is invalid
    public int CostToTraverse(UnitType unitType, 
        GameTile start = null)
    {
        string invalidTerrainMsg = "TileTerrain of GameTile not valid";

        switch (unitType)
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