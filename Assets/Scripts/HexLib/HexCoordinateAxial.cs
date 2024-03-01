using System;
using System.Collections.Generic;

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
        HexCoordinateAxial[] offsets = new HexCoordinateAxial[6]
            {
                new HexCoordinateAxial(1, 0),
                new HexCoordinateAxial(1, -1),
                new HexCoordinateAxial(0, -1),
                new HexCoordinateAxial(-1, 0),
                new HexCoordinateAxial(-1, 1),
                new HexCoordinateAxial(0, 1),
            };

        HexCoordinateAxial[] adjacentHexes = new HexCoordinateAxial[6];
        for(int i = 0; i < 6; i++)
        {
            adjacentHexes[i] = this + offsets[i];
        }

        return adjacentHexes;
    }

    // Returns the hex coordinate adjacent to this hex in the given direction
    public HexCoordinateAxial AdjacentHex(HexUtilities.HexDirection direction)
    {
        return AdjacentHexes()[(int)direction];
    }

    // Returns all hexes exactly n steps away from this hex
    public List<HexCoordinateAxial> HexesExactlyNAway(int n)
    {
        List<HexCoordinateAxial> hexesNAway = new();

        // Traces hexagonal ring of hexes n steps away from this hex
        // Draw lines of n - 1 hexes in each possible direction
        HexCoordinateAxial offset = new HexCoordinateAxial(-n, n);
        Array directions = Enum.GetValues(typeof(HexUtilities.HexDirection));

        foreach (HexUtilities.HexDirection direction in directions)
        {
            for(int i = 0; i < n; i++)
            {
                hexesNAway.Add(this + offset);
                offset = offset.AdjacentHex(direction);
            }
        }

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