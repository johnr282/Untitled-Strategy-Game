using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using StaticRPC = System.Action<Fusion.NetworkRunner, 
//    Fusion.PlayerRef, 
//    Fusion.INetworkStruct>;

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

// Represents a client request to create a new unit
public struct CreateUnitRequest : INetworkStruct
{
    public UnitType Type { get; }
    public Vector3Int Location { get; }
    public int RequestingPlayerID { get; }

    public CreateUnitRequest(UnitType typeIn,
        Vector3Int locationIn,
        int requestingPlayerIDIn)
    {
        Type = typeIn;
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }
}