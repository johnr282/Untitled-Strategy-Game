using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Base class for all other structures
// ------------------------------------------------------------------

public enum StructureType
{
    Capital
}

public struct StructureAttributes
{
    public int Size { get; }

    public StructureAttributes(int sizeIn)
    {
        Size = sizeIn;
    }
}

public class Structure
{
    public StructureType Type { get; }
    public StructureAttributes Attributes { get; }
    public StructureID StructureID { get; }
    public GameTile Location { get; }
    public PlayerID OwnerID { get; set; }
    public StructureObject StructureObject { get; set; } = null;

    public Structure(StructureType typeIn, 
        StructureAttributes attributesIn, 
        StructureID structureIDIn,
        GameTile currentLocationIn,
        PlayerID ownerIDIn)
    {
        Type = typeIn;
        Attributes = attributesIn;
        StructureID = structureIDIn;
        Location = currentLocationIn;
        OwnerID = ownerIDIn;
    }
}
