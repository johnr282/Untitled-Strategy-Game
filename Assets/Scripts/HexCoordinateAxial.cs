using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Class representing the location of a tile in a pointed-top hexagonal
// map using axial coordinates 
// ------------------------------------------------------------------

public class HexCoordinateAxial: HexCoordinate<HexCoordinateAxial>
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

    public override string ToString()
    {
        return "X: " + X.ToString() + ", Y: " + Y.ToString();
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) ||
            !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            HexCoordinateAxial hex = (HexCoordinateAxial)obj;
            return (X == hex.X) && (Y == hex.Y);
        }
    }

    // Returns an array of the 6 adjacent hex coordinates to this hex
    // The first coordinate in array is the hex directly to the east, 
    // and the rest continue counter-clockwise 
    public HexCoordinateAxial[] AdjacentHexes()
    {

    }

    // Returns the hex coordinate adjacent to this hex in the given direction
    public HexCoordinateAxial AdjacentHex(HexUtilities.Direction direction)
    {
        return new HexCoordinateAxial(1, 1);
    }

    // Returns all hexes exactly n steps away from this hex
    public List<HexCoordinateAxial> HexesExactlyNAway(int n)
    {
        // Hexes n steps away will satisfy max(abs(x), abs(y), abs(z)) = n, 
        // where x,y,z are the coordinates of this - hexAway
        List<HexCoordinateAxial> hexesNAway = new();

        return hexesNAway;
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

    
}