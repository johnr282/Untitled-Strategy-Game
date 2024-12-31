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

public readonly struct TestStateUpdate : IStateUpdate { }

// Sent to clients once for each player to update the PlayerManager
public readonly struct PlayerAdded : IStateUpdate
{
    public PlayerRef PlayerRef { get; }
    public PlayerID ID { get; }

    public PlayerAdded(PlayerRef playerRefIn, PlayerID idIn)
    {
        PlayerRef = playerRefIn;
        ID = idIn;
    }
}

// Sent to clients when all players have joined and the game is starting
public readonly struct GameStarted : IStateUpdate
{
    public int MapSeed { get; }

    public GameStarted(int mapSeedIn)
    {
        MapSeed = mapSeedIn;
    }
}

// Sent to clients when it's the next player's turn
public readonly struct NextTurnUpdate : INetworkStruct
{
}

// Sent to clients when a new unit is created
public readonly struct UnitCreatedUpdate : INetworkStruct
{
    public CreateUnitRequest UnitInfo { get; }

    public UnitCreatedUpdate(CreateUnitRequest unitInfoIn)
    {
        UnitInfo = unitInfoIn;
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