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

    // Returns whether a GameTile exists at the given hex
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
    // Throws an ArgumentException if given tile is invalid
    public List<GameTile> Neighbors(GameTile tile)
    {
        List<GameTile> neighborTiles = new();
        HexCoordinateOffset[] neighborHexes = tile.Coordinate.Neighbors();

        foreach (HexCoordinateOffset neighborHex in neighborHexes)
        {
            if (FindTile(neighborHex, out GameTile adjacentTile))
                neighborTiles.Add(adjacentTile);
        }

        return neighborTiles;
    }

    // Returns whether the given hex is traversable by a unit of the given type
    public bool Traversable(UnitType unitType, 
        HexCoordinateOffset hex)
    {
        if (!FindTile(hex, out GameTile tile))
            return false;

        return tile.CostToTraverse(unitType) != ImpassableCost;
    }

    // Returns list of tiles adjacent to the given tile that can be traversed by
    // the given unit type
    public List<GameTile> TraversableNeighbors(UnitType unitType, 
        GameTile tile)
    {
        List<GameTile> neighbors = Neighbors(tile);
        List<GameTile> traversableNeighbors = new();

        foreach (GameTile neighbor in neighbors)
        {
            if (Traversable(unitType, neighbor.Coordinate))
                traversableNeighbors.Add(neighbor);
        }

        return traversableNeighbors;
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

    // Returns the shortest path between the given start and goal hexes for the 
    // given unit type
    // Throws an ArgumentException if start or goal don't exist in the map
    // Throws a RuntimeException if no valid path was found between start and goal
    // or if an invalid hex is found in the returned path, which should never happen
    public List<GameTile> FindShortestPath(UnitType unitType,
        HexCoordinateOffset start, 
        HexCoordinateOffset goal)
    {
        if (!TileExists(start) ||
            !TileExists(goal))
            throw new ArgumentException(
                "Attempted to find path between invalid tiles");

        Func<HexCoordinateOffset, HexCoordinateOffset, int> costFunc = (startHex, goalHex)
            => CostToTraverse(unitType, startHex, goalHex);
        Predicate<HexCoordinateOffset> traversableFunc = (hex)
            => Traversable(unitType, hex);

        Func<HexCoordinateOffset, List<HexCoordinateOffset>> traversableNeighborsFunc = (hex) =>
        {
            if (!FindTile(hex, out GameTile tile))
                throw new ArgumentException(
                    "Attempted to calculate neighbors of invalid tile");

            List<GameTile> traversableNeighborTiles = TraversableNeighbors(unitType, tile);
            List<HexCoordinateOffset> traversableNeighborHexes = new();

            foreach (GameTile neighborTile in traversableNeighborTiles)
            {
                traversableNeighborHexes.Add(neighborTile.Coordinate);
            }
            return traversableNeighborHexes;
        };

        List<HexCoordinateOffset> hexPath = HexUtilities.FindShortestPath(start,
            goal,
            traversableNeighborsFunc,
            costFunc); 

        List<GameTile> tilePath = new();
        foreach (HexCoordinateOffset hex in hexPath)
        {
            if (!FindTile(hex, out GameTile tile))
                throw new RuntimeException("Invalid hex found in path");

            tilePath.Add(tile);
        }

        return tilePath;
    }
}