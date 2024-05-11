using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains an A-Star search algorithm implementation used to find the
// shortest path between two given hexes
// ------------------------------------------------------------------

public static class PathPlanning
{
    // Stores data about hexes needed by the A-Star algorithm
    struct Vertex
    {
        HexCoordinateOffset hex;
        int fScore;
        int gScore;
        bool inOpenList;
    }




    // Returns the shortest path between given start and goal as a list of hexes; 
    // costFunc should be a function that returns the cost to travel between two 
    // adjacent hexes
    public static List<HexCoordinateOffset> FindShortestPath(HexCoordinateOffset start,
        HexCoordinateOffset goal, 
        Func<HexCoordinateOffset, HexCoordinateOffset, int> costFunc)
    {
        List<HexCoordinateOffset> path = new();
        int cost = costFunc(start, goal);
        Debug.Log("Cost from start to goal: " + cost.ToString());

        return path;
    }
}
