using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile
{
    // Location of tile in tilemap
    Vector3Int _coordinate;

    Terrain _terrain;

    public Terrain GetTerrain()
    {
        return _terrain;
    }

    public void SetTerrain(Terrain terrain)
    {
        _terrain = terrain;
    }
}
