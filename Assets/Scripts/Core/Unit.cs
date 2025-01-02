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
    public int OffensiveStrength { get; }
    public int DefensiveStrength { get; }
    public int Size { get; }
    public int MovementActionPoints { get; }
    public int CombatActionPoints { get; }
    public int MovementActionPointsRemaining { get; set; }
    public int CombatActionPointsRemaining { get; set; }
    public UnitType Type { get; }
    public UnitID UnitID { get; }
    public GameTile CurrentLocation { get; set; }
    public PlayerID OwnerID { get; set; }
    public List<Terrain> TraversableTerrains { get; } = new();

    // UnitObject will only be set on the server, clients should not access it
    public UnitObject UnitObject { get; set; } = null;

    public Unit(UnitType typeIn, 
        GameTile currentLocationIn,
        UnitID unitIDIn,
        PlayerID ownerIDIn)
    {
        UnitID = unitIDIn;
        Type = typeIn;
        CurrentLocation = currentLocationIn;
        OwnerID = ownerIDIn;
    }
}