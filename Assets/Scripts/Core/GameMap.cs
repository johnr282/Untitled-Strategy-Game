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

    public struct Continent
    {
        public List<GameTile> ContinentTiles { get; }

        public Continent(List<GameTile> continentTilesIn)
        {
            ContinentTiles = continentTilesIn;
        }
    }

    // Map from continent IDs to continents
    Dictionary<int,  Continent> _continents = new();

    public int NumContinents { get => _continents.Count; }

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

    // Adds given continentID and continent to _continents
    public void AddContinent(int continentID, 
        Continent continent)
    {
        _continents[continentID] = continent;
    }

    // Returns list of tiles adjacent to given tile
    // Throws an ArgumentException if given tile is invalid
    public List<GameTile> Neighbors(GameTile tile)
    {
        List<GameTile> neighborTiles = new();
        HexCoordinateOffset[] neighborHexes = tile.Hex.Neighbors();

        foreach (HexCoordinateOffset neighborHex in neighborHexes)
        {
            if (FindTile(neighborHex, out GameTile adjacentTile))
                neighborTiles.Add(adjacentTile);
        }

        return neighborTiles;
    }

    // Returns whether the given hex is traversable by a unit of the given type
    public bool Traversable(Unit unit, 
        HexCoordinateOffset hex)
    {
        if (!FindTile(hex, out GameTile tile))
            return false;

        return tile.CostToTraverse(unit) != ImpassableCost;
    }

    // Returns list of tiles adjacent to the given tile that can be traversed by
    // the given unit type
    public List<GameTile> TraversableNeighbors(Unit unit, 
        GameTile tile)
    {
        List<GameTile> neighbors = Neighbors(tile);
        List<GameTile> traversableNeighbors = new();

        foreach (GameTile neighbor in neighbors)
        {
            if (Traversable(unit, neighbor.Hex))
                traversableNeighbors.Add(neighbor);
        }

        return traversableNeighbors;
    }

    // Returns the cost for a unit of the given UnitType to traverse from start 
    // to goal
    // Throws an ArgumentException if unitType is invalid, start or goal don't 
    // exist in the map, or start and goal aren't adjacent
    public int CostToTraverse(Unit unit, 
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

        return goalTile.CostToTraverse(unit, startTile);
    }

    // Executes the given action on every tile in the map
    public void ExecuteOnAllTiles(Action<GameTile> action)
    {
        foreach (KeyValuePair<HexCoordinateOffset, GameTile> pair in _gameMap)
        {
            action(pair.Value);
        }
    }

    // Returns the shortest path between the given start and goal tiles for the 
    // given unit 
    // Throws a RuntimeException if no valid path was found between start and goal
    // or if an invalid hex is found in the returned path, which should never happen
    public List<GameTile> FindPath(Unit unit,
        GameTile start, 
        GameTile goal)
    {
        Func<HexCoordinateOffset, HexCoordinateOffset, int> costFunc = (startHex, goalHex)
            => CostToTraverse(unit, startHex, goalHex);

        Func<HexCoordinateOffset, List<HexCoordinateOffset>> traversableNeighborsFunc = (hex) =>
        {
            if (!FindTile(hex, out GameTile tile))
                throw new ArgumentException(
                    "Attempted to calculate neighbors of invalid tile");

            List<GameTile> traversableNeighborTiles = TraversableNeighbors(unit, tile);
            List<HexCoordinateOffset> traversableNeighborHexes = new();

            foreach (GameTile neighborTile in traversableNeighborTiles)
            {
                traversableNeighborHexes.Add(neighborTile.Hex);
            }
            return traversableNeighborHexes;
        };

        List<HexCoordinateOffset> hexPath = HexUtilities.FindShortestPath(start.Hex,
            goal.Hex,
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

    // Returns a list of n unique starting tiles; each tile is guaranteed to be
    // on land, and if possible, each tile will be on a different continent
    public List<GameTile> GenerateStartingTiles(int n)
    {
        List<GameTile> startingTiles = new();
        List<int> availableContinents = ContinentIDList();

        for (int i = 0; i < n; i++)
        {
            if (availableContinents.Count == 0)
                availableContinents = ContinentIDList();

            int continentIndex = UnityUtilities.RandomIndex(availableContinents);
            Continent continent = _continents[availableContinents[continentIndex]];

            int tileIndex = UnityUtilities.RandomIndex(continent.ContinentTiles);
            GameTile startingTile = continent.ContinentTiles[tileIndex];
            startingTiles.Add(startingTile);
        }

        return startingTiles;
    }

    // Returns a list of every valid continent ID
    List<int> ContinentIDList()
    {
        return UnityUtilities.SequentialList(0, NumContinents);
    }
}