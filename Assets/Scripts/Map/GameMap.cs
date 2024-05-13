using System;
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
    Dictionary<HexCoordinateOffset, GameTile> _gameMap = new();

    // A value big enough to be larger than any possible A* g score, but not large 
    // enough that it could overflow and become negative
    public const int ImpassableCost = int.MaxValue / 2;

    // Returns whether a GameTile exists with the given hex
    public bool TileExists(HexCoordinateOffset hex)
    {
        return _gameMap.ContainsKey(hex);
    }

    // Returns true if GameTile exists at given coordinate and gets GameTile 
    // at that location; returns false otherwise
    public bool FindTile(HexCoordinateOffset hex, 
        out GameTile tile)
    {
        return _gameMap.TryGetValue(hex, out tile);
    }

    // Adds given GameTile at given coordinate to map
    public void AddTile(HexCoordinateOffset hex, 
        GameTile newTile)
    {
        _gameMap.Add(hex, newTile);
    }

    // Sets GameTile at given coordinate to given GameTile; returns false if no
    // tile exists at given coordinate, returns true otherwise
    public bool SetTile(HexCoordinateOffset hex, 
        GameTile newTile)
    {
        if (!FindTile(hex, out GameTile tile))
            return false;

        _gameMap[hex] = newTile;
        return true;
    }

    // Changes terrain of GameTile at given coordinate to given TerrainType; 
    // returns false if no tile exists at given coordinate, returns true otherwise
    public bool ChangeTerrain(HexCoordinateOffset hex, 
        Terrain newTerrain)
    {
        if (!FindTile(hex, out GameTile tile))
            return false;

        tile.TileTerrain = newTerrain;
        _gameMap[hex] = tile;
        return true;
    }

    // Sets continent ID of GameTile at given coordinate to given ID; returns false
    // if no tile exists at given coordinate, returns true otherwise
    public bool SetContinentID(HexCoordinateOffset hex, 
        int newContinentID)
    {
        if (!FindTile(hex, out GameTile tile))
            return false;

        tile.ContinentID = newContinentID;
        _gameMap[hex] = tile;
        return true;
    }

    // Returns list of tiles adjacent to given tile
    public List<GameTile> AdjacentTiles(GameTile tile)
    {
        List<GameTile> adjacentTiles = new();
        HexCoordinateOffset[] adjacentHexes = tile.Coordinate.Neighbors();

        foreach (HexCoordinateOffset adjacentHex in adjacentHexes)
        {
            if (FindTile(adjacentHex, out GameTile adjacentTile))
                adjacentTiles.Add(adjacentTile);
        }

        return adjacentTiles;
    }

    // Returns whether the given hex is traversable by a unit of the given type
    public bool Traversable(UnitType unitType, 
        HexCoordinateOffset hex)
    {
        if (!FindTile(hex, out GameTile tile))
            return false;

        return tile.CostToTraverse(unitType) != ImpassableCost;
    }

    // Returns the cost for a unit of the given UnitType to traverse from start 
    // to goal
    // Throws an ArgumentException if unitType is invalid, start or goal don't 
    // exist in the map, or start and goal aren't adjacent
    public int CostToTraverse(UnitType unitType, 
        HexCoordinateOffset start,
        HexCoordinateOffset goal)
    {
        if (!HexUtilities.AreAdjacent(start, goal))
            throw new ArgumentException(
                "Attempted to calculate cost between non-adjacent tiles");

        if (!FindTile(start, out GameTile startTile) ||
            !FindTile(goal, out GameTile goalTile))
            throw new ArgumentException(
                "Attempted to calculate cost between nonexistent tiles");

        return goalTile.CostToTraverse(unitType, startTile);
    }

    // Executes the given action on every tile in the map
    public void ExecuteOnAllTiles(Action<GameTile> action)
    {
        foreach (KeyValuePair<HexCoordinateOffset, GameTile> pair in _gameMap)
        {
            action(pair.Value);
        }
    }
}