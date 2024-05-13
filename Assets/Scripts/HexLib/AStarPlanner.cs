using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains an A-Star search algorithm implementation used to find the
// shortest path between two given nodes
// ------------------------------------------------------------------

public class AStarPlanner<TNode>
{
    // Contains nodes that still need to be explored; priority of each node
    // is its f score, which is the sum of its g score and heuristic
    PriorityQueue<TNode, int> _frontier = new();

    // Stores current cost from the start to each node
    Dictionary<TNode, int> _gScores = new();

    // Stores each node's preceding node in the path
    Dictionary<TNode, TNode> _cameFrom = new();

    TNode _start;
    TNode _goal;

    // Function that returns an array containing all of the given node's neighbors
    Func<TNode, TNode[]> _neighborsFunc;

    // Function that returns the cost to travel between two given adjacent nodes
    Func<TNode, TNode, int> _costFunc;

    // Function that returns an estimate of the cost of the cheapest possible path 
    // from the given node to _goal; in order for the algorithm to always return
    // the optimal path, this heuristic must never overestimate the cost of this path
    Func<TNode, int> _heuristicFunc;

    // Function that returns whether the given node is traversable
    Predicate<TNode> _traversableFunc;

    public AStarPlanner(Func<TNode, TNode[]> neighborsFuncIn,
        Func<TNode, TNode, int> costFuncIn,
        Func<TNode, int> heuristicFuncIn,
        Predicate<TNode> traversableFuncIn)
    {
        _neighborsFunc = neighborsFuncIn;
        _costFunc = costFuncIn;
        _heuristicFunc = heuristicFuncIn;
        _traversableFunc = traversableFuncIn;
    }

    // Returns the shortest path between the given start and goal
    // Throws a RuntimeException if no valid path is found
    public List<TNode> FindPath(TNode startIn, 
        TNode goalIn)
    {
        _start = startIn; 
        _goal = goalIn;

        AddStartNode();

        while (!_frontier.Empty())
        {
            TNode minNode = _frontier.Dequeue();
            if (minNode.Equals(_goal))
            {
                Debug.Log("Path found with a cost of " + _gScores[minNode].ToString());
                return ReconstructPath();
            }   

            UpdateNeighbors(minNode);
        }

        throw new RuntimeException("AStarPlanner failed to find path from " + 
            _start.ToString() + " to " + _goal.ToString());
    }

    // Initializes and adds the start node to _frontier
    void AddStartNode()
    {
        _gScores[_start] = 0;
        _frontier.Enqueue(_start, CalculateFScore(_start));
    }

    // For each neighbor of given node, updates g scores and adds to _frontier
    // if necessary
    void UpdateNeighbors(TNode node)
    {
        TNode[] neighbors = _neighborsFunc(node);
        foreach (TNode neighbor in neighbors)
        {
            if (!_traversableFunc(neighbor))
                continue;

            int newGScore = _gScores[node] + _costFunc(node, neighbor);
            bool neighborUnexplored = !_gScores.ContainsKey(neighbor);

            if (neighborUnexplored ||
                newGScore < _gScores[neighbor])
            {
                _gScores[neighbor] = newGScore;
                _cameFrom[neighbor] = node;
                _frontier.Enqueue(neighbor, CalculateFScore(neighbor));
            }
        }
    }

    // Reconstructs and returns the path from _start to _goal found by the algorithm 
    List<TNode> ReconstructPath()
    {
        List<TNode> path = new();
        TNode current = _goal;
        int maxPathSize = 100;

        while (_cameFrom.ContainsKey(current) &&
            path.Count < maxPathSize)
        {
            path.Add(current);
            current = _cameFrom[current];
        }

        if (!current.Equals(_start))
            throw new RuntimeException("Failed to reconstruct valid path");

        path.Reverse();
        return path;
    }

    // Calculates and returns the given node's f score
    int CalculateFScore(TNode node)
    {
        return _gScores[node] + _heuristicFunc(node);
    }
}