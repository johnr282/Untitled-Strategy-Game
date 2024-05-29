using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains the ClientAction class, which is used to represent all
// possible actions a client can take and their corresponding RPCs.
// ClientActions are sent to the server by the ClientActionManager.
// If the action is valid, the server sends out a corresponding 
// game state update, which is handled in the GameStateManager.
// Also contains definitions for all INetworkStruct objects used to 
// represent each action.
// ------------------------------------------------------------------

public class ClientAction
{
    public INetworkStruct ActionData { get; }
    public Delegate RPC { get; } 

    public ClientAction(INetworkStruct inputDataIn,
        Delegate rpcIn)
    {
        ActionData = inputDataIn;
        RPC = rpcIn;
    }

    public void CallRPC(NetworkRunner runner)
    {
        // Need to dynamically invoke because each RPC will have a different 
        // ActionData type, and using an Action<> results in the compiler error
        // "can't convert method group to Action<>
        RPC.DynamicInvoke(runner,
            PlayerRef.None,
            ActionData);
    }
}

public readonly struct EndTurnAction : INetworkStruct
{
    public PlayerID EndingPlayerID { get; }

    public EndTurnAction(PlayerID playerIDIn)
    {
        EndingPlayerID = playerIDIn;
    }
}

public readonly struct CreateUnitAction : INetworkStruct
{
    public UnitType Type { get; }
    public HexCoordinateOffset Location { get; }
    public PlayerID RequestingPlayerID { get; }

    public CreateUnitAction(UnitType typeIn,
        HexCoordinateOffset locationIn,
        PlayerID requestingPlayerIDIn)
    {
        Type = typeIn;
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }
}

public readonly struct MoveUnitAction : INetworkStruct
{
    public UnitID UnitID { get; }
    public HexCoordinateOffset Location { get; }
    public PlayerID RequestingPlayerID { get; }

    public MoveUnitAction(UnitID unitIDIn,
        HexCoordinateOffset locationIn,
        PlayerID requestingPlayerIDIn)
    {
        UnitID = unitIDIn;
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }
}