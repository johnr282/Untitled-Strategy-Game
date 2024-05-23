using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains the NetworkInputEvent class, which is used to represent all
// possible actions a client can take and their corresponding RPCs;
// also contains definitions for all INetworkStruct objects used to 
// represent each action
// ------------------------------------------------------------------

public class NetworkInputEvent
{
    public INetworkStruct InputData { get; }
    public Delegate RPC { get; } 
    public NetworkRunner Runner { get; }

    public NetworkInputEvent(INetworkStruct inputDataIn,
        Delegate rpcIn,
        NetworkRunner runnerIn)
    {
        InputData = inputDataIn;
        RPC = rpcIn;
        Runner = runnerIn;
    }

    public void CallRPC()
    {
        // Need to dynamically invoke because each RPC will have a different 
        // InputData type, and using an Action<> results in the compiler error
        // "can't convert method group to Action<>
        RPC.DynamicInvoke(Runner,
            PlayerRef.None,
            InputData);
    }
}

// A client request to create a new unit
public struct CreateUnitRequest : INetworkStruct
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

// A client request to move a unit 
public struct MoveUnitRequest : INetworkStruct
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