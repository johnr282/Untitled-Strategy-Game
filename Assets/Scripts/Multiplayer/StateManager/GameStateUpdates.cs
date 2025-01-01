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
public interface IStateUpdate : INetworkStruct { }

// Sent to clients once for each player to update the PlayerManager
public readonly struct AddPlayerUpdate : IStateUpdate
{
    public PlayerRef PlayerRef { get; }
    public PlayerID ID { get; }

    public AddPlayerUpdate(PlayerRef playerRefIn, PlayerID idIn)
    {
        PlayerRef = playerRefIn;
        ID = idIn;
    }
}

// Sent to clients when all players have joined and the game is starting
public readonly struct StartGameUpdate : IStateUpdate
{
    public int MapSeed { get; }

    public StartGameUpdate(int mapSeedIn)
    {
        MapSeed = mapSeedIn;
    }
}

// Sent to clients when it's the next player's turn
public readonly struct NextTurnUpdate : IStateUpdate { }

// Sent to clients when a new unit is created
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
}

// Sent to clients when a unit is moved
public readonly struct UnitMovedUpdate : INetworkStruct
{
    public UnitID UnitID { get; }
    public HexCoordinateOffset NewLocation { get; }

    public UnitMovedUpdate(UnitID unitIDIn,
        HexCoordinateOffset newLocationIn)
    {
        UnitID = unitIDIn;
        NewLocation = newLocationIn;
    }
}