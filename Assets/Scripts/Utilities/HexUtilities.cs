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
    public enum Direction
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
}