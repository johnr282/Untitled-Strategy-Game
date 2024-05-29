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

// Sent to clients once for each player to update the PlayerManager
public readonly struct PlayerAddedUpdate : INetworkStruct
{
    public PlayerRef PlayerRef { get; }

    public PlayerAddedUpdate(PlayerRef playerRefIn)
    {
        PlayerRef = playerRefIn;
    }
}

// Sent to clients when all players have joined and the game is starting
public readonly struct GameStartedUpdate : INetworkStruct
{
    public int MapSeed { get; }

    public GameStartedUpdate(int mapSeedIn)
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
    public CreateUnitAction UnitInfo { get; }

    public UnitCreatedUpdate(CreateUnitAction unitInfoIn)
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