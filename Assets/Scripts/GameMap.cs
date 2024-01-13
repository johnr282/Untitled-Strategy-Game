using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component representing the entire game map composed of a grid of 
// GameTiles; not used for visual rendering
// ------------------------------------------------------------------

public class GameMap : MonoBehaviour
{
    // Keys are coordinates of tiles in the map, value is the tile itself
    // Unity indexes pointed-top hexagons (col, row), so coordinates are (col, row)
    Dictionary<Vector3Int, GameTile> _gameMap = new Dictionary<Vector3Int, GameTile>();

    // Returns true if HexTile exists at given coordinate and gets HexTile 
    // at that location; returns false otherwise
    public bool FindTile(Vector3Int coordinate, 
        out GameTile tile)
    {
        return _gameMap.TryGetValue(coordinate, out tile);
    }

    // Adds given HexTile at given coordinate to map
    public void AddTile(Vector3Int coordinate, 
        GameTile tile)
    {
        _gameMap.Add(coordinate, tile);
    }

    // Changes terrain of GameTile at given coordinate to given TerrainType; 
    // returns false if no tile exists at given coordinate, returns true otherwise
    public bool ChangeTerrain(Vector3Int coordinate, 
        Terrain.TerrainType newTerrainType)
    {
        if (!FindTile(coordinate, out GameTile tile))
            return false;

        tile.SetTerrain(new Terrain(newTerrainType));
        _gameMap[coordinate] = tile;
        return true;
    }
}
