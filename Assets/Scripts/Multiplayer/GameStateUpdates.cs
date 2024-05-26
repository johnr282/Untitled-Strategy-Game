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

// Sent to clients when all players have joined and the game is starting
public readonly struct GameStarted : INetworkStruct
{
    public int MapSeed { get; }

    public GameStarted(int mapSeedIn)
    {
        MapSeed = mapSeedIn;
    }
}

// Sent to clients when it's a new player's turn
public readonly struct TurnChanged : INetworkStruct
{

}

// Sent to clients when a new unit is created
public readonly struct UnitCreated : INetworkStruct
{
    public CreateUnitRequest UnitInfo { get; }

    public UnitCreated(CreateUnitRequest unitInfoIn)
    {
        UnitInfo = unitInfoIn;
    }
}

// Sent to clients when a unit is moved
public readonly struct UnitMoved : INetworkStruct
{
    public UnitID UnitID { get; }
    public HexCoordinateOffset NewLocation { get; }

    public UnitMoved(UnitID unitIDIn,
        HexCoordinateOffset newLocationIn)
    {
        UnitID = unitIDIn;
        NewLocation = newLocationIn;
    }
}