using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains structs representing game state updates that the server
// sends to clients; each one is an argument to its corresponding RPC,
// so they all need to implement the INetworkStruct interface
// ------------------------------------------------------------------

// Base interface for all game state updates
public interface IStateUpdate : INetworkStruct 
{
    // Returns a list of updates composing this update; must include this
    // or this update won't be applied
    // Allows state updates to be composed of multiple updates, each
    // of which are validated and applied by the StateManager, and defines the
    // order in which each update is applied
    public List<IStateUpdate> GetStateUpdatesInOrder();
}

// Adds players to the player manager
public readonly struct AddPlayerUpdate : IStateUpdate
{
    public PlayerRef PlayerRef { get; }
    public PlayerID ID { get; }

    public AddPlayerUpdate(PlayerRef playerRefIn, PlayerID idIn)
    {
        PlayerRef = playerRefIn;
        ID = idIn;
    }

    public List<IStateUpdate> GetStateUpdatesInOrder() => new List<IStateUpdate>{ this };
}

// Starts the game after all players have joined
public readonly struct StartGameUpdate : IStateUpdate
{
    public int MapSeed { get; }

    public StartGameUpdate(int mapSeedIn)
    {
        MapSeed = mapSeedIn;
    }

    public List<IStateUpdate> GetStateUpdatesInOrder() => new List<IStateUpdate> { this };
}

// Ends the active player's turn
public readonly struct EndActivePlayersTurnUpdate : IStateUpdate 
{ 
    public PlayerID RequestingPlayerID { get; }

    public EndActivePlayersTurnUpdate(PlayerID requestingPlayerIDIn)
    {
        RequestingPlayerID = requestingPlayerIDIn;
    }

    public List<IStateUpdate> GetStateUpdatesInOrder() => new List<IStateUpdate>{ this };
}


// Creates a unit
public readonly struct CreateUnitUpdate : IStateUpdate
{
    public UnitType Type { get; }
    public HexCoordinateOffset Location { get; }
    public PlayerID RequestingPlayerID { get; }

    public CreateUnitUpdate(UnitType typeIn,
        HexCoordinateOffset locationIn,
        PlayerID requestingPlayerIDIn)
    {
        Type = typeIn;
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }

    public List<IStateUpdate> GetStateUpdatesInOrder() => new List<IStateUpdate>{ this };
}

// Moves a unit
public readonly struct MoveUnitUpdate : IStateUpdate
{
    public UnitID UnitID { get; }
    public HexCoordinateOffset NewLocation { get; }
    public PlayerID RequestingPlayerID { get; }

    public MoveUnitUpdate(UnitID unitIDIn,
        HexCoordinateOffset newLocationIn,
        PlayerID requestingPlayerIDIn)
    {
        UnitID = unitIDIn;
        NewLocation = newLocationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }

    public List<IStateUpdate> GetStateUpdatesInOrder() => new List<IStateUpdate>{ this };
}

// Places a territory selection unit to either claim a new tile or
// reinforce an already claimed tile
public readonly struct PlaceTerritorySelectionUnitUpdate : IStateUpdate
{
    public HexCoordinateOffset Location { get; }
    public PlayerID RequestingPlayerID { get; }

    public PlaceTerritorySelectionUnitUpdate(HexCoordinateOffset locationIn,
        PlayerID requestingPlayerIDIn)
    {
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }

    public List<IStateUpdate> GetStateUpdatesInOrder()
    {
        return new List<IStateUpdate>
        {
            new CreateUnitUpdate(UnitType.Infantry, Location, RequestingPlayerID),
            this,
            new EndActivePlayersTurnUpdate(RequestingPlayerID)
        };
    }
}

// Creates a structure
public readonly struct CreateStructureUpdate : IStateUpdate
{
    public StructureType Type { get; }
    public HexCoordinateOffset Location { get; }
    public PlayerID RequestingPlayerID { get; }

    public CreateStructureUpdate(StructureType typeIn,
        HexCoordinateOffset locationIn,
        PlayerID requestingPlayerIDIn)
    {
        Type = typeIn;
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }

    public List<IStateUpdate> GetStateUpdatesInOrder() => new List<IStateUpdate>{ this };
}

// Places a player's capital, the final step in the territory selection phase
public readonly struct PlaceCapitalUpdate : IStateUpdate
{
    public HexCoordinateOffset Location { get; }
    public PlayerID RequestingPlayerID { get; }

    public PlaceCapitalUpdate(HexCoordinateOffset locationIn,
        PlayerID requestingPlayerIDIn)
    {
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }

    public List<IStateUpdate> GetStateUpdatesInOrder()
    {
        return new List<IStateUpdate>
        {
            new CreateStructureUpdate(StructureType.Capital, Location, RequestingPlayerID),
            this,
            new EndActivePlayersTurnUpdate(RequestingPlayerID)
        };
    }
}