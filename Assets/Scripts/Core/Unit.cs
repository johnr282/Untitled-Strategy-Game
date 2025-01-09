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
    Land, 
    Naval, 
    Air
}

public enum UnitType
{
    Infantry
}

public struct UnitAttributes
{
    public int OffensiveStrength { get; }
    public int DefensiveStrength { get; }
    public int Size { get; }
    public int MovementActionPoints { get; }
    public int CombatActionPoints { get; }
    public UnitCategory Category { get; }

    public UnitAttributes(int offensiveStrengthIn,
        int defensiveStrengthIn,
        int sizeIn,
        int movementActionPointsIn,
        int combatActionPointsIn,
        UnitCategory categoryIn)
    {
        OffensiveStrength = offensiveStrengthIn;
        DefensiveStrength = defensiveStrengthIn;
        Size = sizeIn;
        MovementActionPoints = movementActionPointsIn;
        CombatActionPoints = combatActionPointsIn;
        Category = categoryIn;
    }
}

public class Unit
{
    public UnitType Type { get; }
    public UnitAttributes Attributes { get; }
    public UnitID UnitID { get; }
    public GameTile CurrentLocation { get; set; }
    public PlayerID OwnerID { get; set; }
    public int MovementActionPointsRemaining { get; set; }
    public int CombatActionPointsRemaining { get; set; }

    // UnitObject will only be set on the server, clients should not access it
    public UnitObject UnitObject { get; set; } = null;

    public Unit(UnitType typeIn, 
        UnitID unitIDIn,
        GameTile currentLocationIn,
        PlayerID ownerIDIn)
    {
        Type = typeIn;
        Attributes = UnitManager.GetUnitAttributes(Type);
        UnitID = unitIDIn;
        CurrentLocation = currentLocationIn;
        OwnerID = ownerIDIn;
        MovementActionPointsRemaining = Attributes.MovementActionPoints;
        CombatActionPointsRemaining = Attributes.CombatActionPoints;
    }
}