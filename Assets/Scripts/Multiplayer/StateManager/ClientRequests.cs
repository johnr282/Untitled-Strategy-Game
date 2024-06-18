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
    public IClientRequestData RequestData { get; }
    Predicate<IClientRequestData> RequestValid { get; }
    Action<IClientRequestData> UpdateState { get; }

    public Delegate RPC { get; } 

    public ClientRequest(IClientRequestData requestDataIn,
        Predicate<IClientRequestData> requestValidIn,
        Action<IClientRequestData> updateStateIn)
    {
        RequestData = requestDataIn;
        RequestValid = requestValidIn;
        UpdateState = updateStateIn;
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

