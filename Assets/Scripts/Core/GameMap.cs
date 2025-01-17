using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// ------------------------------------------------------------------
// Static class representing the entire game map composed of a grid of 
// GameTiles; part of the global game state
// ------------------------------------------------------------------

public class Continent
{
    public List<GameTile> ContinentTiles = new();
    public int Size { get => ContinentTiles.Count; }

    // Adds the given tile to this continent
    public void AddTile(GameTile tile)
    {
        ContinentTiles.Add(tile);
    }
}

public static class GameMap
{
    static Dictionary<HexCoordinateOffset, GameTile> _gameMap = new();

    // List of continents in the map, continent ID is the index corresponding
    // to that continent
    static List<Continent> _continents = new();

    public static int NumCols { get; set; }
    public static int NumRows { get; set; }

    public static int NumContinents { get => _continents.Count; }

    // A value big enough to be larger than any possible A* g score, but not large 
    // enough that it could overflow and become negative
    public const int ImpassableCost = int.MaxValue / 2;

    // Returns whether a GameTile exists at the given hex
    public static bool TileExists(HexCoordinateOffset hex)
    {
        return _gameMap.ContainsKey(hex);
    }

    // Returns the GameTile at the given hex
    // Throws an ArgumentException if no GameTile exists at the given hex
    public static GameTile GetTile(HexCoordinateOffset hex)
    {
        if (!_gameMap.ContainsKey(hex))
            throw new ArgumentException(
                "No GameTile exists at the given hex");

        return _gameMap[hex];
    }

    // Adds given GameTile at given coordinate to map
    // Throws an ArgumentException if a GameTile with the given hex already exists
    public static void AddTile(HexCoordinateOffset hex, 
        GameTile newTile)
    {
        if (TileExists(hex))
            throw new ArgumentException("GameTile with given hex already exists");

        _gameMap.Add(hex, newTile);
    }

    // Sets GameTile at given coordinate to given GameTile;
    // Throws an ArgumentException if no GameTile exists at the given hex
    public static void SetTile(HexCoordinateOffset hex, 
        GameTile newTile)
    {
        if (!TileExists(hex))
            throw new ArgumentException("No GameTile exists at the given hex");

        _gameMap[hex] = newTile;
    }

    // Clears the game map
    public static void ClearMap()
    {
        _gameMap.Clear();
        _continents.Clear();
    }

    // Adds given continentID and continent to _continents; continents should
    // be added sequentially by ID
    // Throws an ArgumentException if continent ID doesn't equal the size of 
    // the current continents list or if the number of continents would exceed
    // the max
    public static void AddContinent(ContinentID continentID, 
        Continent continent)
    {
        if (NumContinents != continentID.ID)
            throw new ArgumentException("Continent ID is non-sequential");

        _continents.Add(continent);
    }

    // Returns list of valid tiles adjacent to given tile
    // Throws an ArgumentException if given tile is invalid
    public static List<GameTile> Neighbors(GameTile tile)
    {
        List<GameTile> neighborTiles = new();
        HexCoordinateOffset[] neighborHexes = tile.Hex.Neighbors();

        foreach (HexCoordinateOffset neighborHex in neighborHexes)
        {
            // For tiles on the edge of the map, some neighbor hexes will be
            // invalid; simply skip over these
            try
            {
                GameTile adjacentTile = GetTile(neighborHex);
                neighborTiles.Add(adjacentTile);
            }
            catch (ArgumentException)
            {
                continue;
            }
        }

        return neighborTiles;
    }

    // Returns whether the given tile is traversable by a unit of the given type
    // Throws an ArgumentException if given hex is invalid
    public static bool Traversable(Unit unit, 
        GameTile tile)
    {
        return tile.CostToTraverse(unit, unit.CurrentLocation) != ImpassableCost;
    }

    // Returns list of hexes adjacent to the given tile that can be traversed by
    // the given unit
    public static List<HexCoordinateOffset> TraversableNeighbors(Unit unit, 
        GameTile tile)
    {
        List<GameTile> neighbors = Neighbors(tile);
        List<HexCoordinateOffset> traversableNeighbors = new();

        foreach (GameTile neighbor in neighbors)
        {
            if (Traversable(unit, neighbor))
                traversableNeighbors.Add(neighbor.Hex);
        }

        return traversableNeighbors;
    }

    // Returns the cost for the given unit to traverse between the given start
    // hex and the adjacent goal hex
    // Throws an ArgumentException if goal doesn't exist in the map or the unit
    // isn't adjacent to the goal hex
    public static int CostToTraverse(Unit unit,
        HexCoordinateOffset start,
        HexCoordinateOffset goal)
    {
        if (!HexUtilities.AreAdjacent(start, goal))
            throw new ArgumentException(
                "Attempted to calculate cost between non-adjacent tiles");

        GameTile goalTile = GetTile(goal);
        GameTile startTile = GetTile(start);
        return goalTile.CostToTraverse(unit, startTile);
    }

    // Executes the given action on every tile in the map
    public static void ExecuteOnAllTiles(Action<GameTile> action)
    {
        foreach (KeyValuePair<HexCoordinateOffset, GameTile> pair in _gameMap)
        {
            action(pair.Value);
        }
    }

    // Returns the shortest path between the given unit's current location and
    // the given goal tile for the given unit
    // Includes the goal, but not the start, in the returned path
    // Throws a RuntimeException if no valid path was found
    public static List<HexCoordinateOffset> FindPath(Unit unit,
        GameTile goal)
    {
        Func<HexCoordinateOffset, HexCoordinateOffset, int> costFunc = (startHex, goalHex)
            => CostToTraverse(unit, startHex, goalHex);

        Func<HexCoordinateOffset, List<HexCoordinateOffset>> traversableNeighborsFunc = (hex) =>
        {
            GameTile tile = GetTile(hex);
            return TraversableNeighbors(unit, tile);
        };

        return HexUtilities.FindShortestPath(
            unit.CurrentLocation.Hex,
            goal.Hex,
            traversableNeighborsFunc,
            costFunc); 
    }

    // Returns a list of every valid continent ID
    public static List<int> ContinentIDList()
    {
        return UnityUtilities.SequentialList(0, NumContinents);
    }

    public static Continent GetContinent(int continentID)
    {
        if (continentID >= NumContinents)
            throw new ArgumentException("Invalid continent ID");

        return _continents[continentID];
    }
}