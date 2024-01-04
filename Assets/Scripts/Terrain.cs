using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Terrain : MonoBehaviour
{
    public enum TerrainType
    {
        sea,
        land
    }

    [SerializeField] TerrainType _terrainType;

    public TerrainType GetTerrainType()
    {
        return _terrainType;
    }

    // Returns true if units are able to travel on this terrain
    public bool IsTraversable()
    {
        if (_terrainType == TerrainType.sea)
            return false;
        else
            return true;
    }
}
