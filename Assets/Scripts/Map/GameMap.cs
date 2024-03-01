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
    Dictionary<HexCoordinateOffset, GameTile> _gameMap = new Dictionary<HexCoordinateOffset, GameTile>();

    // Returns true if HexTile exists at given coordinate and gets HexTile 
    // at that location; returns false otherwise
    public bool FindTile(HexCoordinateOffset coordinate, 
        out GameTile tile)
    {
        return _gameMap.TryGetValue(coordinate, out tile);
    }

    // Adds given HexTile at given coordinate to map
    public void AddTile(HexCoordinateOffset coordinate, 
        GameTile newTile)
    {
        _gameMap.Add(coordinate, newTile);
    }

    // Sets GameTile at given coordinate to given GameTile; returns false if no
    // tile exists at given coordinate, returns true otherwise
    public bool SetTile(HexCoordinateOffset coordinate, 
        GameTile newTile)
    {
        if (!FindTile(coordinate, out GameTile tile))
            return false;

        _gameMap[coordinate] = newTile;
        return true;
    }

    // Changes terrain of GameTile at given coordinate to given TerrainType; 
    // returns false if no tile exists at given coordinate, returns true otherwise
    public bool ChangeTerrain(HexCoordinateOffset coordinate, 
        Terrain.TerrainType newTerrainType)
    {
        if (!FindTile(coordinate, out GameTile tile))
            return false;

        tile.TileTerrain = new Terrain(newTerrainType);
        _gameMap[coordinate] = tile;
        return true;
    }

    // Sets continent ID of GameTile at given coordinate to given ID; returns false
    // if no tile exists at given coordinate, returns true otherwise
    public bool SetContinentID(HexCoordinateOffset coordinate, 
        int newContinentID)
    {
        if (!FindTile(coordinate, out GameTile tile))
            return false;

        tile.ContinentID = newContinentID;
        _gameMap[coordinate] = tile;
        return true;
    }
}