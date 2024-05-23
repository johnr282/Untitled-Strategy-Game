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

public readonly struct UnitID : INetworkStruct
{
    public readonly ushort ID { get; }

    public UnitID(ushort idIn)
    {
        ID = idIn;
    }
}

public struct Unit : INetworkStruct
{
    public int Strength {  get; }
    public int Capacity { get; }
    public UnitType Type { get; }
    public UnitID UnitID { get; }
    public GameTile CurrentLocation { get; set; }

    [Networked, Capacity(GameTile.TerrainTypeCount)]
    public NetworkArray<Terrain> TraversableTerrains => default;

    public Unit(UnitType typeIn, 
        GameTile currentLocationIn,
        UnitID unitIDIn)
    {
        Strength = -1;
        Capacity = -1;
        Type = typeIn;
        CurrentLocation = currentLocationIn;
        UnitID = unitIDIn;
    }
}