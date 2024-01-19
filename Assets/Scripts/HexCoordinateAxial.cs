using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Class representing the location of a tile in a pointed-top hexagonal
// map using axial coordinates 
// ------------------------------------------------------------------

public class HexCoordinateAxial
{
    public int X { get; }
    public int Y { get; }

    // Constructor
    public HexCoordinateAxial(int xIn,
        int yIn)
    {
        X = xIn;
        Y = yIn;
    }

    // Addition operator overload
    public static HexCoordinateAxial operator +(HexCoordinateAxial lhs,
        HexCoordinateAxial rhs)
    {
        return new HexCoordinateAxial(lhs.X + rhs.X,
            lhs.Y + rhs.Y);
    }

    // Subtraction operator overload
    public static HexCoordinateAxial operator -(HexCoordinateAxial lhs,
        HexCoordinateAxial rhs)
    {
        return new HexCoordinateAxial(lhs.X - rhs.X,
            lhs.Y - rhs.Y);
    }

    public override string ToString()
    {
        return "X: " + X.ToString() + ", Y: " + Y.ToString();
    }

    // Converts this axial coordinate to the offset coordinate system
    public HexCoordinateOffset AxialToOffset()
    {
        int col = X + (Y - (Y % 2)) / 2;
        int row = Y;
        return new HexCoordinateOffset(col, row);
    }

    // Calculates implicit Z coordinate used in cube coordinate system
    public int CalculateZ()
    {
        return (-X) - Y;
    }

    // Returns all hexes exactly n steps away from this hex
    public List<HexCoordinateAxial> HexesExactlyNAway(int n)
    {
        // Hexes n steps away will satisfy max(abs(x), abs(y), abs(z)) = n, 
        // where x,y,z are the coordinates of this - hexAway
        List<HexCoordinateAxial> hexesNAway = new();

        return hexesNAway;
    }
}