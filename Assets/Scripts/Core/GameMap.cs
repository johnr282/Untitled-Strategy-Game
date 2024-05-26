using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// ------------------------------------------------------------------
// Component representing the entire game map composed of a grid of 
// GameTiles
// ------------------------------------------------------------------

public class Continent
{
    public List<GameTile> ContinentTiles = new();
    public int Size { get => ContinentTiles.Count; }

    // Adds the given tile to this continent
    // Throws an ArgumentException if this continent is at max capacity
    public void AddTile(GameTile tile)
    {
        ContinentTiles.Add(tile);
    }
}

public class GameMap : NetworkBehaviour
{
    Dictionary<HexCoordinateOffset, GameTile> _gameMap = new();

    public int NumCols { get; set; }
    public int NumRows { get; set; }

    // Keys are coordinates of tiles in the map, value is the tile itself
    //Dictionary<HexCoordinateOffset, GameTile> _gameMap = new();


    // List of continents in the map, continent ID is the index corresponding
    // to that continent
    List<Continent> _continents = new();

    public int NumContinents { get => _continents.Count; }

    // A value big enough to be larger than any possible A* g score, but not large 
    // enough that it could overflow and become negative
    public const int ImpassableCost = int.MaxValue / 2;

    // Returns whether a GameTile exists at the given hex
    public bool TileExists(HexCoordinateOffset hex)
    {
        return _gameMap.ContainsKey(hex);
    }

    // Returns the GameTile at the given hex
    // Throws an ArgumentException if no GameTile exists at the given hex
    public GameTile GetTile(HexCoordinateOffset hex)
    {
        if (!_gameMap.ContainsKey(hex))
            throw new ArgumentException(
                "No GameTile exists at the given hex");

        return _gameMap[hex];
    }

    // Adds given GameTile at given coordinate to map
    // Throws an ArgumentException if a GameTile with the given hex already exists
    public void AddTile(HexCoordinateOffset hex, 
        GameTile newTile)
    {
        if (TileExists(hex))
            throw new ArgumentException("GameTile with given hex already exists");

        _gameMap.Add(hex, newTile);
    }

    // Sets GameTile at given coordinate to given GameTile;
    // Throws an ArgumentException if no GameTile exists at the given hex
    public void SetTile(HexCoordinateOffset hex, 
        GameTile newTile)
    {
        if (!TileExists(hex))
            throw new ArgumentException("No GameTile exists at the given hex");

        _gameMap[hex] = newTile;
    }

    // Adds given continentID and continent to _continents; continents should
    // be added sequentially by ID
    // Throws an ArgumentException if continent ID doesn't equal the size of 
    // the current continents list or if the number of continents would exceed
    // the max
    public void AddContinent(ContinentID continentID, 
        Continent continent)
    {
        if (NumContinents != continentID.ID)
            throw new ArgumentException("Continent ID is non-sequential");

        _continents.Add(continent);
    }

    // Returns list of valid tiles adjacent to given tile
    // Throws an ArgumentException if given tile is invalid
    public List<GameTile> Neighbors(GameTile tile)
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

    // Returns whether the given hex is traversable by a unit of the given type
    // Throws an ArgumentException if given hex is invalid
    public bool Traversable(Unit unit, 
        HexCoordinateOffset hex)
    {
        GameTile tile = GetTile(hex);
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

    // Returns the cost for the given unit to traverse between its current hex
    // and the adjacent goal hex
    // Throws an ArgumentException if goal doesn't exist in the map or the unit
    // isn't adjacent to the goal hex
    public int CostToTraverse(Unit unit,
        HexCoordinateOffset goal)
    {
        if (!HexUtilities.AreAdjacent(unit.CurrentLocation.Hex, goal))
            throw new ArgumentException(
                "Attempted to calculate cost between non-adjacent tiles");

        GameTile goalTile = GetTile(goal);
        return goalTile.CostToTraverse(unit);
    }

    // Executes the given action on every tile in the map
    public void ExecuteOnAllTiles(Action<GameTile> action)
    {
        foreach (KeyValuePair<HexCoordinateOffset, GameTile> pair in _gameMap)
        {
            action(pair.Value);
        }
    }

    // Returns the shortest path between the given unit's current location and
    // the given goal tile for the given unit
    // Throws a RuntimeException if no valid path was found 
    // Throws an ArgumentException if an invalid hex is found in the returned
    // path, which should never happen
    public List<GameTile> FindPath(Unit unit,
        GameTile goal)
    {
        Func<HexCoordinateOffset, HexCoordinateOffset, int> costFunc = (startHex, goalHex)
            => CostToTraverse(unit, goalHex);

        Func<HexCoordinateOffset, List<HexCoordinateOffset>> traversableNeighborsFunc = (hex) =>
        {
            GameTile tile = GetTile(hex);
            List<GameTile> traversableNeighborTiles = TraversableNeighbors(unit, tile);
            List<HexCoordinateOffset> traversableNeighborHexes = new();

            foreach (GameTile neighborTile in traversableNeighborTiles)
            {
                traversableNeighborHexes.Add(neighborTile.Hex);
            }
            return traversableNeighborHexes;
        };

        List<HexCoordinateOffset> hexPath = HexUtilities.FindShortestPath(
            unit.CurrentLocation.Hex,
            goal.Hex,
            traversableNeighborsFunc,
            costFunc); 

        List<GameTile> tilePath = new();
        foreach (HexCoordinateOffset hex in hexPath)
        {
            tilePath.Add(GetTile(hex));
        }

        return tilePath;
    }

    // Returns a list of every valid continent ID
    public List<int> ContinentIDList()
    {
        return UnityUtilities.SequentialList(0, NumContinents);
    }

    public Continent GetContinent(int continentID)
    {
        if (continentID >= NumContinents)
            throw new ArgumentException("Invalid continent ID");

        return _continents[continentID];
    }
}