using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

// ------------------------------------------------------------------
// Classes representing the location of a tile in a hexagonal map; 
// implements classes for offset and axial coordinate systems
// ------------------------------------------------------------------

public class HexCoordinateOffset
{
    public int Col { get; }
    public int Row { get; }

    // Constructor
    public HexCoordinateOffset(int colIn, 
        int rowIn)
    {
        Col = colIn;
        Row = rowIn;
    }

    // Addition operator overload
    public static HexCoordinateOffset operator +(HexCoordinateOffset lhs,
        HexCoordinateOffset rhs)
    {
        return new HexCoordinateOffset(lhs.Col + rhs.Col, 
            lhs.Row + rhs.Row);
    }

    // Subtraction operator overload
    public static HexCoordinateOffset operator -(HexCoordinateOffset lhs,
        HexCoordinateOffset rhs)
    {
        return new HexCoordinateOffset(lhs.Col - rhs.Col, 
            lhs.Row - rhs.Row);
    }

    public override string ToString()
    {
        return "Col: " + Col.ToString() + ", Row: " + Row.ToString();
    }

    public override int GetHashCode()
    {
        return Col.GetHashCode() ^ Row.GetHashCode();
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
            HexCoordinateOffset hex = (HexCoordinateOffset)obj;
            return (Col == hex.Col) && (Row == hex.Row);
        }
    }

    // Converts this coordinate to a Vector2Int of the form (Col, Row)
    public Vector2Int ConvertToVector2Int()
    {
        return new Vector2Int(Col, Row);
    }

    // Converts this coordinate to a Vector3Int of the form (Col, Row, 0)
    public Vector3Int ConvertToVector3Int()
    {
        return new Vector3Int(Col, Row, 0);
    }

    // Returns an array of the 6 adjacent hex coordinates to this hex
    // The first coordinate in array is the hex directly to the right, 
    // and the rest continue counter-clockwise 
    public HexCoordinateOffset[] AdjacentHexes()
    {
        HexCoordinateOffset[] adjacentHexes = new HexCoordinateOffset[6];

        // Offsets are different for odd and even rows
        HexCoordinateOffset[] offsets = new HexCoordinateOffset[6];
        bool evenRow = (Row % 2) == 0;

        if (evenRow)
        {
            offsets = new HexCoordinateOffset[6]
                {
                    new HexCoordinateOffset(1, 0),
                    new HexCoordinateOffset(1, -1),
                    new HexCoordinateOffset(0, -1),
                    new HexCoordinateOffset(-1, 0),
                    new HexCoordinateOffset(0, 1),
                    new HexCoordinateOffset(1, 1)
                };
        }
        else
        {
            offsets = new HexCoordinateOffset[6]
                {
                    new HexCoordinateOffset(1, 0),
                    new HexCoordinateOffset(0, -1),
                    new HexCoordinateOffset(-1, -1),
                    new HexCoordinateOffset(-1, 0),
                    new HexCoordinateOffset(-1, 1),
                    new HexCoordinateOffset(0, 1)
                };
        }

        // Add each offset to this hex to get the adjacent hexes
        for(int i = 0; i < 6; i++)
        {
            adjacentHexes[i] = this + offsets[i];
        }

        return adjacentHexes;
    }
}

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
}