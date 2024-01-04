using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    Dictionary<Vector3Int, HexTile> _hexMap = new Dictionary<Vector3Int, HexTile>();

    // Returns true if HexTile exists at given coordinate and gets HexTile 
    // at that location; returns false otherwise
    public bool FindTile(Vector3Int coordinate, out HexTile tile)
    {
        return _hexMap.TryGetValue(coordinate, out tile);
    }

    // Adds given HexTile at given coordinate to map
    public void AddTile(Vector3Int coordinate, HexTile tile)
    {
        _hexMap.Add(coordinate, tile);
    }
}
