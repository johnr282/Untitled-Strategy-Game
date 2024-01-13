using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Utility functions for hexagonal grid systems; uses the offset
// coordinate system
// ------------------------------------------------------------------

public static class HexUtilities 
{
    // Given a hex coordinate (col, row), returns a list of coordinates
    // of the 6 adjacent hex tiles
    public static Vector2Int[] AdjacentHexes(Vector2Int coordinate)
    {
        Vector2Int[] adjacentCoordinates = new Vector2Int[6];

        int col = coordinate.x;
        int row = coordinate.y;

        // Offsets are different for odd and even rows
        bool evenRow = (row % 2) == 0;

        if(evenRow)
        {

        }

        return adjacentCoordinates;
    }
}
