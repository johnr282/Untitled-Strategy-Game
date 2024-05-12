using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains an A-Star search algorithm implementation used to find the
// shortest path between two given hexes
// ------------------------------------------------------------------

public class AStarPlanner
{
    // Contains hexes that still need to be explored; priority of each hex
    // is its f score, which is the sum of its g score and heuristic
    PriorityQueue<HexCoordinateOffset, int> _frontier = new();

    // Stores current cost from the start to each hex
    Dictionary<HexCoordinateOffset, int> _gScores = new();

    // Stores each hex's preceding hex in the path
    Dictionary<HexCoordinateOffset, HexCoordinateOffset> _cameFrom = new();

    HexCoordinateOffset _start;
    HexCoordinateOffset _goal;

    // Function that returns the cost to travel between two given adjacent hexes
    Func<HexCoordinateOffset, HexCoordinateOffset, int> _costFunc;

    public AStarPlanner(Func<HexCoordinateOffset, HexCoordinateOffset, int> costFuncIn)
    {
        _costFunc = costFuncIn;
    }

    public List<HexCoordinateOffset> FindPath(HexCoordinateOffset startIn, 
        HexCoordinateOffset goalIn)
    {
        _start = startIn; 
        _goal = goalIn;

        AddStartVertex();

        while (!_frontier.Empty())
        {
            HexCoordinateOffset minHex = _frontier.Dequeue();
            if (minHex == _goal)
            {
                return ReconstructPath();
            }   

            UpdateNeighbors(minHex);
        }

        throw new RuntimeException("AStarPlanner failed to find path from " + 
            _start.ToString() + " to " + _goal.ToString());
    }

    // Initializes and adds the start vertex to _frontier
    void AddStartVertex()
    {
        _gScores[_start] = 0;
        _frontier.Enqueue(_start, CalculateFScore(_start));
    }

    // For each neighbor of given hex, updates g scores and adds to _frontier
    // if necessary
    void UpdateNeighbors(HexCoordinateOffset hex)
    {
        HexCoordinateOffset[] neighbors = hex.AdjacentHexes();
        foreach (HexCoordinateOffset neighbor in neighbors)
        {
            int newGScore = _gScores[hex] + _costFunc(hex, neighbor);
            bool neighborUnexplored = !_gScores.ContainsKey(neighbor);
            int currentGScore = neighborUnexplored ? int.MaxValue : _gScores[neighbor];

            if (newGScore < currentGScore)
            {
                _gScores[neighbor] = newGScore;
                _cameFrom[neighbor] = hex;
                try
                {
                    _frontier.Enqueue(neighbor, CalculateFScore(neighbor));
                }
                catch (ArgumentException)
                {
                    // Simply continue if an element with the same hex and f score
                    // already exists in _frontier
                }
            }
        }
    }

    // Reconstructs and returns the path from _start to _goal found by the algorithm 
    List<HexCoordinateOffset> ReconstructPath()
    {
        List<HexCoordinateOffset> path = new();
        HexCoordinateOffset current = _goal;

        while (current != _start)
        {
            path.Add(current);
            current = _cameFrom[current];
        }

        path.Reverse();
        return path;
    }

    // Calculates and returns the given hex's f score
    int CalculateFScore(HexCoordinateOffset hex)
    {
        return _gScores[hex] + Heuristic(hex);
    }

    // Returns the length of the cheapest possible path from the given hex to
    // the goal
    int Heuristic(HexCoordinateOffset hex)
    {
        return HexUtilities.DistanceBetween(hex, _goal);
    }
}