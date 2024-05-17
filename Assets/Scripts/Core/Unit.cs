using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int Strength {  get; }
    public int Capacity { get; }
    public UnitType Type { get; }
    public List<Terrain> TraversableTerrains { get; }
    public GameTile CurrentLocation { get; set; }
    public int UnitID { get; }

    public Unit(UnitType typeIn, 
        GameTile currentLocationIn,
        int unitIDIn)
    {
        Type = typeIn;
        CurrentLocation = currentLocationIn;
        UnitID = unitIDIn;
    }
}
