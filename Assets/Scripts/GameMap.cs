using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    // Keys are coordinates of tiles in the map, value is the tile itself
    // Unity indexes pointed-top hexagons (col, row), so coordinates are (col, row)
    Dictionary<Vector3Int, GameTile> _hexMap = new Dictionary<Vector3Int, GameTile>();

    // Returns true if HexTile exists at given coordinate and gets HexTile 
    // at that location; returns false otherwise
    public bool FindTile(Vector3Int coordinate, out GameTile tile)
    {
        return _hexMap.TryGetValue(coordinate, out tile);
    }

    // Adds given HexTile at given coordinate to map
    public void AddTile(Vector3Int coordinate, GameTile tile)
    {
        _hexMap.Add(coordinate, tile);
    }
}
