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

public class Unit
{
    public int Strength { get; }
    public int Capacity { get; }
    public UnitType Type { get; }
    public UnitID UnitID { get; }
    public GameTile CurrentLocation { get; set; }
    public List<Terrain> TraversableTerrains { get; } = new();

    // UnitObject will only be set on the server, clients should not access it
    public UnitObject UnitObject { get; set; } = null;

    public Unit(UnitType typeIn, 
        GameTile currentLocationIn,
        UnitID unitIDIn)
    {
        UnitID = unitIDIn;
        Strength = -1;
        Capacity = -1;
        Type = typeIn;
        CurrentLocation = currentLocationIn;
    }
}