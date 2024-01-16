using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Class representing a single hexagonal tile in the game map; not
// used for visual rendering
// ------------------------------------------------------------------

public class GameTile
{
    // Location of tile in game map
    HexCoordinateOffset _coordinate;

    public Terrain TileTerrain { get; set; }

    public int ContinentID { get; set; }

    // Constructor
    public GameTile(HexCoordinateOffset coordinate, 
        Terrain terrain)
    {
        _coordinate = coordinate;
        TileTerrain = terrain;
    }
}