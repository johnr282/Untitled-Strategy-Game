using System;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;

// ------------------------------------------------------------------
// Contains an A-Star search algorithm implementation used to find the
// shortest path between two given nodes
// ------------------------------------------------------------------

public class AStarPlanner<TNode>
{
    // Contains nodes that still need to be explored; priority of each node
    // is its f score, which is the sum of its g score and heuristic
    SimplePriorityQueue<TNode, int> _frontier = new();

    // Stores current cost from the start to each node
    Dictionary<TNode, int> _gScores = new();

    // Stores each node's preceding node in the path
    Dictionary<TNode, TNode> _cameFrom = new();

    TNode _start;
    TNode _goal;

    // Function that returns a list of the given node's traversable neighbors
    Func<TNode, List<TNode>> _traversableNeighborsFunc;

    // Function that returns the cost to travel between two given adjacent nodes
    Func<TNode, TNode, int> _costFunc;

    // Function that returns an estimate of the cost of the cheapest possible path 
    // from the given node to _goal; in order for the algorithm to always return
    // the optimal path, this heuristic must never overestimate the cost of this path
    Func<TNode, int> _heuristicFunc;

    public AStarPlanner(Func<TNode, List<TNode>> traversableNeighborsFuncIn,
        Func<TNode, TNode, int> costFuncIn,
        Func<TNode, int> heuristicFuncIn)
    {
        _traversableNeighborsFunc = traversableNeighborsFuncIn;
        _costFunc = costFuncIn;
        _heuristicFunc = heuristicFuncIn;
    }

    // Returns the shortest path between the given start and goal
    // Includes goal in returned path, but not start
    // Throws a RuntimeException if no valid path is found
    public List<TNode> FindPath(TNode startIn, 
        TNode goalIn)
    {
        _start = startIn; 
        _goal = goalIn;

        AddStartNode();

        while (_frontier.Count != 0)
        {
            TNode minNode = _frontier.Dequeue();
            if (minNode.Equals(_goal))
                return ReconstructPath();

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
        List<TNode> traversableNeighbors = _traversableNeighborsFunc(node);
        foreach (TNode neighbor in traversableNeighbors)
        {
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