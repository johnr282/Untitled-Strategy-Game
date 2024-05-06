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

    // Returns true if this tile is traversable by a land unit
    public bool TraversableByLand()
    {
        switch (TileTerrain)
        {
            case Terrain.sea:
                return false;

            case Terrain.land:
                return true;

            default:
                throw new RuntimeException("");
        }
    }

    // Returns true if this tile is traversable by a naval unit
    public bool TraversableBySea()
    {
        switch (TileTerrain)
        {
            case Terrain.sea:
                return true;

            case Terrain.land:
                return false;

            default:
                return true;
        }
    }
}