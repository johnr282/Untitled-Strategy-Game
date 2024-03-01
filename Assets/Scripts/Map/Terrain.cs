using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Class for handling different terrain types of game tiles
// ------------------------------------------------------------------

public class Terrain
{
    public enum TerrainType
    {
        sea,
        land
    }

    public TerrainType Type { get; }

    // Constructor
    public Terrain(TerrainType terrainType)
    {
        Type = terrainType;
    }

    // Returns true if units are able to travel on this terrain
    public bool IsTraversable()
    {
        if (Type == TerrainType.sea)
            return false;
        else
            return true;
    }
}
