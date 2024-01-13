using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Class representing a single hexagonal tile in the game map; not
// used for visual rendering
// ------------------------------------------------------------------

public class GameTile
{
    // Location of tile in tilemap
    // Unity indexes pointed-top hexagons (col, row), so coordinates are (col, row, 0)
    Vector2Int _coordinate;

    Terrain _terrain;

    int _continentID;

    // Constructor
    public GameTile(Vector2Int coordinate, 
        Terrain terrain)
    {
        _coordinate = coordinate;
        _terrain = terrain;
    }

    public Terrain GetTerrain()
    {
        return _terrain;
    }

    public void SetTerrain(Terrain terrain)
    {
        _terrain = terrain;
    }

    public int GetContinent()
    {
        return _continentID;
    }

    public void SetContinent(int continentID)
    {
        _continentID = continentID;
    }
}
