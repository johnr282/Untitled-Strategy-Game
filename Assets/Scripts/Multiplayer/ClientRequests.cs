using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains the ClientRequest class, which is used to represent all
// possible requests a client can make to the server and their
// corresponding RPCs.
// ClientRequests are sent to the server by the ClientRequestManager.
// If the request is valid, the server sends out a corresponding 
// game state update, which is handled in the GameStateManager.
// Also contains definitions for all INetworkStruct objects used to 
// represent each request.
// ------------------------------------------------------------------

public class ClientRequest
{
    public INetworkStruct RequestData { get; }
    public Delegate RPC { get; } 

    public ClientRequest(INetworkStruct requestDataIn,
        Delegate rpcIn)
    {
        RequestData = requestDataIn;
        RPC = rpcIn;
    }

    public void CallRPC(NetworkRunner runner)
    {
        // Need to dynamically invoke because each RPC will have a different 
        // RequestData type, and using an Action<> results in the compiler error
        // "can't convert method group to Action<>
        RPC.DynamicInvoke(runner,
            PlayerRef.None,
            RequestData);
    }
}

public readonly struct EndTurnRequest : INetworkStruct
{
    public PlayerID EndingPlayerID { get; }

    public EndTurnRequest(PlayerID playerIDIn)
    {
        EndingPlayerID = playerIDIn;
    }
}

public readonly struct CreateUnitRequest : INetworkStruct
{
    public UnitType Type { get; }
    public HexCoordinateOffset Location { get; }
    public PlayerID RequestingPlayerID { get; }

    public CreateUnitRequest(UnitType typeIn,
        HexCoordinateOffset locationIn,
        PlayerID requestingPlayerIDIn)
    {
        Type = typeIn;
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }
}

public readonly struct MoveUnitRequest : INetworkStruct
{
    public UnitID UnitID { get; }
    public HexCoordinateOffset Location { get; }
    public PlayerID RequestingPlayerID { get; }

    public MoveUnitRequest(UnitID unitIDIn,
        HexCoordinateOffset locationIn,
        PlayerID requestingPlayerIDIn)
    {
        UnitID = unitIDIn;
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }
}