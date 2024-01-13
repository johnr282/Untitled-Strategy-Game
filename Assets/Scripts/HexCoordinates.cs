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
    public readonly int col;
    public readonly int row;

    // Constructor
    public HexCoordinateOffset(int colIn, 
        int rowIn)
    {
        col = colIn;
        row = rowIn;
    }

    // Addition operator overload
    public static HexCoordinateOffset operator +(HexCoordinateOffset lhs,
        HexCoordinateOffset rhs)
    {
        return new HexCoordinateOffset(lhs.col + rhs.col, 
            lhs.row + rhs.row);
    }

    // Subtraction operator overload
    public static HexCoordinateOffset operator -(HexCoordinateOffset lhs,
        HexCoordinateOffset rhs)
    {
        return new HexCoordinateOffset(lhs.col - rhs.col, 
            lhs.row - rhs.row);
    }

    public override string ToString()
    {
        return "col: " + col.ToString() + ", row: " + row.ToString();
    }

    public override int GetHashCode()
    {
        return col.GetHashCode() ^ row.GetHashCode();
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
            return (col == hex.col) && (row == hex.row);
        }
    }

    // Converts this coordinate to a Vector2Int of the form (col, row)
    public Vector2Int ConvertToVector2Int()
    {
        return new Vector2Int(col, row);
    }

    // Converts this coordinate to a Vector3Int of the form (col, row, 0)
    public Vector3Int ConvertToVector3Int()
    {
        return new Vector3Int(col, row, 0);
    }

    // Returns an array of the 6 adjacent hex coordinates to this hex
    // The first coordinate in array is the hex directly to the right, 
    // and the rest continue counter-clockwise 
    public HexCoordinateOffset[] AdjacentHexes()
    {
        HexCoordinateOffset[] adjacentHexes = new HexCoordinateOffset[6];

        // Offsets are different for odd and even rows
        HexCoordinateOffset[] offsets = new HexCoordinateOffset[6];
        bool evenRow = (row % 2) == 0;

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
    public readonly int q;
    public readonly int r;

    // Constructor
    public HexCoordinateAxial(int qIn, 
        int rIn)
    {
        q = qIn;
        r = rIn;
    }

    // Addition operator overload
    public static HexCoordinateAxial operator +(HexCoordinateAxial lhs,
        HexCoordinateAxial rhs)
    {
        return new HexCoordinateAxial(lhs.q + rhs.q, 
            lhs.r + rhs.r);
    }

    // Subtraction operator overload
    public static HexCoordinateAxial operator -(HexCoordinateAxial lhs,
        HexCoordinateAxial rhs)
    {
        return new HexCoordinateAxial(lhs.q - rhs.q, 
            lhs.r - rhs.r);
    }

    public override string ToString()
    {
        return "q: " + q.ToString() + ", r: " + r.ToString();
    }
}