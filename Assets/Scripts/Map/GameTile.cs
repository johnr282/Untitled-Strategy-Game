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

    // Returns the cost for a land unit to travel into this tile from the 
    // given adjacent start tile
    public int CostByLand(GameTile start)
    {
        switch (TileTerrain)
        {
            case Terrain.sea:
                return int.MaxValue;

            case Terrain.land:
                return 1;

            default:
                throw new RuntimeException("TileTerrain of GameTile not valid");
        }
    }

    // Returns the cost for a sea unit to travel into this tile from the 
    // given adjacent start tile
    public int CostBySea(GameTile start)
    {
        switch (TileTerrain)
        {
            case Terrain.sea:
                return 1;

            case Terrain.land:
                return int.MaxValue;

            default:
                throw new RuntimeException("TileTerrain of GameTile not valid");
        }
    }
}