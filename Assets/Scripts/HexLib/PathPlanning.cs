using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathPlanning 
{
    // Returns the shortest path between given start and goal as a list of hexes
    public static List<HexCoordinateOffset> FindShortestPath(HexCoordinateOffset start,
        HexCoordinateOffset goal, 
        Predicate<HexCoordinateOffset> traversable)
    {
        List<HexCoordinateOffset> path = new();
        Debug.Log("Start traversable: " + traversable(start).ToString());

        return path;
    }
}
