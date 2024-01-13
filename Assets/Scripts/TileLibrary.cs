using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component for storing tile assets
// ------------------------------------------------------------------

public class TileLibrary : MonoBehaviour
{
    public TileBase defaultTile;
    public TileBase landTile;
    public TileBase seaTile;

    // Returns corresponding Tile object for given terrain
    public TileBase GetCorrespondingTile(Terrain terrain)
    {
        Terrain.TerrainType type = terrain.GetTerrainType();

        switch (type)
        {
            case Terrain.TerrainType.sea:
                return seaTile;

            case Terrain.TerrainType.land:
                return landTile;

            default:
                return defaultTile;
        }
    }

}
