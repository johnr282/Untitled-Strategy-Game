using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    public enum TerrainType
    {
        ocean,
        land
    }

    [SerializeField] TerrainType _terrainType;

    

    // Returns true if units are able to travel on this terrain
    public bool IsTraversable()
    {
        if (_terrainType == TerrainType.ocean)
            return false;
        else
            return true;
    }
}
