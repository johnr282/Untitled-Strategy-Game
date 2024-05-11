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
    // Stores data about hexes needed by the A-Star algorithm
    struct Vertex
    {
        HexCoordinateOffset hex;
        int fScore;
        int gScore;
        bool inOpenList;
    }

    // Contains vertices that still need to be explored; priority of each vertex
    // is its f score
    PriorityQueue<Vertex, int> openList = new();

    // Contains vertices that have been explored already
    HashSet<Vertex> closedList = new();

    HexCoordinateOffset start;
    HexCoordinateOffset goal;

    // Function that returns the cost to travel between two given adjacent hexes
    Func<HexCoordinateOffset, HexCoordinateOffset, int> costFunc;

    public AStarPlanner(Func<HexCoordinateOffset, HexCoordinateOffset, int> costFuncIn)
    {
        costFunc = costFuncIn;
    }

    public List<HexCoordinateOffset> FindPath(HexCoordinateOffset startIn, 
        HexCoordinateOffset goalIn)
    {
        start = startIn; 
        goal = goalIn;

        return new();
    }
    
}
