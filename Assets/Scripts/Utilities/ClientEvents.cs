using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains event definitions used with EventBus system that are
// published on a client
// ------------------------------------------------------------------   

// Published when a new tile is hovered over
public readonly struct NewTileHoveredOverEvent
{
    public HexCoordinateOffset Coordinate { get; }

    public NewTileHoveredOverEvent(HexCoordinateOffset coordinateIn)
    {
        Coordinate = coordinateIn;
    }
}

// Published when a tile is selected
public readonly struct TileSelectedEvent
{
    public HexCoordinateOffset Coordinate { get; }

    public TileSelectedEvent(HexCoordinateOffset coordinateIn)
    {
        Coordinate = coordinateIn;
    }
}

// Published when it's this client's turn
public readonly struct MyTurnEvent { }

// Published when the territory selection phase starts
public readonly struct TerritorySelectionPhaseStartedEvent 
{ 
    public int InfantryBudget { get; }

    public TerritorySelectionPhaseStartedEvent(int infantryBudgetIn)
    {
        InfantryBudget = infantryBudgetIn;
    }
}

public readonly struct TerritorySelectionUnitPlacedEvent
{
    public PlayerID PlayerID { get; }
    public int NewRemainingInfantryBudget { get; }

    public TerritorySelectionUnitPlacedEvent(PlayerID playerIDIn,
        int newRemainingInfantryBudgetIn)
    {
        PlayerID = playerIDIn;
        NewRemainingInfantryBudget = newRemainingInfantryBudgetIn;
    }
}

public readonly struct SelectingCapitalLocationsEvent { }

public readonly struct TerritorySelectionPhaseEndedEvent { }