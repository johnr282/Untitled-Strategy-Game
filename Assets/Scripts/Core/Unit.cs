using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// ------------------------------------------------------------------
// Class representing a single unit
// ------------------------------------------------------------------

/*
 * Data files for each unit type: 
 * Unit Type Attributes
 *      strength
 *      capacity
 *      movement points
 *      attack points
 *      unit category
 * 
 * Data file for each unit category:
 * Unit Category Attributes
 *      Costs to traverse each type of terrain
 *          sea
 *          land
 *      
 */

public enum UnitCategory
{ 

}

// TODO: make these the categories instead
public enum UnitType
{
    land, 
    naval, 
    air
}

public struct Unit : INetworkStruct
{
    public int Strength {  get; }
    public int Capacity { get; }
    public UnitType Type { get; }
    public int UnitID { get; }
    public GameTile CurrentLocation { get; set; }

    [Capacity(GameTile.TerrainTypeCount)]
    public NetworkLinkedList<Terrain> TraversableTerrains { get; }

    public Unit(UnitType typeIn, 
        GameTile currentLocationIn,
        int unitIDIn)
    {
        Strength = -1;
        Capacity = -1;
        Type = typeIn;
        CurrentLocation = currentLocationIn;
        UnitID = unitIDIn;
        TraversableTerrains = new();
    }
}