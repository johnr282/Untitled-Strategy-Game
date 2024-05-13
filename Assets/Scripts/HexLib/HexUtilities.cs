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

    // Converts given Vector3Int to a HexCoordinateOffset of the form (x, y)
    public static HexCoordinateOffset ConvertToHexCoordinateOffset(Vector3Int coordinate)
    {
        return new HexCoordinateOffset(coordinate.x, coordinate.y);
    }

    // Converts given Vector2Int to a HexCoordinateOffset of the form (x, y)
    public static HexCoordinateOffset ConvertToHexCoordinateOffset(Vector2Int coordinate)
    {
        return new HexCoordinateOffset(coordinate.x, coordinate.y);
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

    // Returns whether the given hexes are adjacent
    public static bool AreAdjacent(HexCoordinateOffset a, 
        HexCoordinateOffset b)
    {
        return DistanceBetween(a, b) == 1;
    }

    // Returns whether the given hexes are adjacent
    public static bool AreAdjacent(HexCoordinateAxial a,
        HexCoordinateAxial b)
    {
        return DistanceBetween(a, b) == 1;
    }

    // Returns the shortest path between given start and goal as a list of hexes; 
    // traversableNeighborsFunc should be a function that returns the list of the
    // given node's traversable neighbors
    // costFunc should be a function that returns the cost to travel between two 
    // adjacent hexes
    // Throws a RuntimeException if no valid path is found
    public static List<HexCoordinateOffset> FindShortestPath(HexCoordinateOffset start,
        HexCoordinateOffset goal,
        Func<HexCoordinateOffset, List<HexCoordinateOffset>> traversableNeighborsFunc,
        Func<HexCoordinateOffset, HexCoordinateOffset, int> costFunc)
    {
        Func<HexCoordinateOffset, int> heuristicFunc = (hex)
            => DistanceBetween(hex, goal);

        AStarPlanner<HexCoordinateOffset> planner = new(traversableNeighborsFunc,
            costFunc,
            heuristicFunc);
        return planner.FindPath(start, goal);
    }
}