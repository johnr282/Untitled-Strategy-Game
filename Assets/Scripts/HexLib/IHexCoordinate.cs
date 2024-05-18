using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

// ------------------------------------------------------------------
// Interface representing the location of a tile in a hexagonal map
// ------------------------------------------------------------------

public interface IHexCoordinate<HexCoordinateType> 
{
    // Returns an array of the 6 adjacent hex coordinates to this hex
    // The first coordinate in array is the hex directly to the east, 
    // and the rest continue counter-clockwise 
    public HexCoordinateType[] Neighbors();

    // Returns the hex coordinate adjacent to this hex in the given direction
    public HexCoordinateType Neighbor(HexUtilities.HexDirection direction);

    // Returns all hexes exactly n steps away from this hex
    public List<HexCoordinateType> HexesExactlyNAway(int n);
}