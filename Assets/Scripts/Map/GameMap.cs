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
        Terrain newTerrain)
    {
        if (!FindTile(coordinate, out GameTile tile))
            return false;

        tile.TileTerrain = newTerrain;
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

    // Returns the cost for a land unit to travel from the given start tile to
    // the given goal tile
    // Throws ArgumentException if start and goal are not adjacent or if start 
    // or goal do not exist on the map
    public int CostByLand(HexCoordinateOffset start, 
        HexCoordinateOffset goal)
    {
        ValidateAdjacentTiles(start, 
            goal, 
            out GameTile startTile);

        if (!FindTile(goal, out GameTile goalTile))
            return int.MaxValue;

        return goalTile.CostByLand(startTile);
    }

    // Returns the cost for a land unit to travel from the given start tile to
    // the given goal tile
    // Throws ArgumentException if start and goal are not adjacent or if start 
    // or goal do not exist on the map
    public int CostBySea(HexCoordinateOffset start,
        HexCoordinateOffset goal)
    {
        ValidateAdjacentTiles(start,
            goal,
            out GameTile startTile);

        if (!FindTile(goal, out GameTile goalTile))
            return int.MaxValue;

        return goalTile.CostBySea(startTile);
    }

    // Ensures that the given hexes are adjacent and the start tile exists, and
    // puts corresponding tiles into startTile output parameter
    // Throws ArgumentException if hexes aren't adjacent or if start does not exist
    // on the map
    void ValidateAdjacentTiles(HexCoordinateOffset start,
        HexCoordinateOffset goal, 
        out GameTile startTile)
    {
        if (!HexUtilities.AreAdjacent(start, goal))
            throw new ArgumentException("Attempted to calculate cost between non-adjacent tiles");

        if (!FindTile(start, out startTile))
        {
            throw new ArgumentException("Attempted to calculate cost between invalid tiles");
        }
    }
}