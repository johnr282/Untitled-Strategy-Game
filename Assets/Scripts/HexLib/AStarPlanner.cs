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

    // Function that returns whether the given hex is traversable
    Predicate<HexCoordinateOffset> _traversableFunc;

    public AStarPlanner(Func<HexCoordinateOffset, HexCoordinateOffset, int> costFuncIn,
        Predicate<HexCoordinateOffset> traversableFuncIn)
    {
        _costFunc = costFuncIn;
        _traversableFunc = traversableFuncIn;
    }

    // Returns the shortest path between the given start and goal
    // Throws a RuntimeException if no valid path is found
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
        HexCoordinateOffset[] neighbors = hex.Neighbors();
        foreach (HexCoordinateOffset neighbor in neighbors)
        {
            if (!_traversableFunc(neighbor))
                continue;

            int newGScore = _gScores[hex] + _costFunc(hex, neighbor);
            bool neighborUnexplored = !_gScores.ContainsKey(neighbor);
            int currentGScore = neighborUnexplored ? int.MaxValue : _gScores[neighbor];

            if (newGScore < currentGScore)
            {
                _gScores[neighbor] = newGScore;
                _cameFrom[neighbor] = hex;

                // Ensure that no dupliate hexes and priorities exist in the frontier
                int newFScore = CalculateFScore(neighbor);
                _frontier.TryRemove(neighbor, newFScore);
                _frontier.Enqueue(neighbor, newFScore);
            }
        }
    }

    // Reconstructs and returns the path from _start to _goal found by the algorithm 
    List<HexCoordinateOffset> ReconstructPath()
    {
        List<HexCoordinateOffset> path = new();
        HexCoordinateOffset current = _goal;
        int maxPathSize = 100;

        while (_cameFrom.ContainsKey(current) &&
            path.Count < maxPathSize)
        {
            path.Add(current);
            current = _cameFrom[current];
        }

        if (current != _start)
            throw new RuntimeException("Failed to reconstruct valid path");

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