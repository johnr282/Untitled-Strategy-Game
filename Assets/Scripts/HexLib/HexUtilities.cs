using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Utility functions for hexagonal grid systems; uses the offset
// coordinate system
// ------------------------------------------------------------------

public static class HexUtilities 
{
    // Represents the 6 possible directions from a hex
    public enum HexDirection
    {
        east, 
        northeast, 
        northwest, 
        west, 
        southwest, 
        southeast
    }

    // Given two offset hex coordinates, returns the distance in hexes 
    // between them
    public static int DistanceBetween(HexCoordinateOffset a, 
        HexCoordinateOffset b)
    {
        HexCoordinateAxial axialA = a.OffsetToAxial();
        HexCoordinateAxial axialB = b.OffsetToAxial();
        return DistanceBetween(axialA, axialB);
    }

    // Given two axial hex coordinates, returns the distance in hexes
    // between them
    public static int DistanceBetween(HexCoordinateAxial a, 
        HexCoordinateAxial b)
    {
        HexCoordinateAxial diff = a - b;
        // Calculate implicit z coordinate used in cube coordinates
        int diffZ = diff.CalculateZ();
        return Mathf.Max(Mathf.Abs(diff.X), 
            Mathf.Abs(diff.Y), 
            Mathf.Abs(diffZ));
    }

    // Returns the shortest path between given start and goal as a list of hexes; 
    // costFunc should be a function that returns the cost to travel between two 
    // adjacent hexes
    public static List<HexCoordinateOffset> FindShortestPath(HexCoordinateOffset start,
        HexCoordinateOffset goal,
        Func<HexCoordinateOffset, HexCoordinateOffset, int> costFunc)
    {
        AStarPlanner planner = new AStarPlanner(costFunc);
        return planner.FindPath(start, goal);
    }
}