using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains RPCs used by clients to communicate with the server
// ------------------------------------------------------------------

public class ClientMessages : NetworkBehaviour  
{
    // Called by a client to notify server that the player's turn corresponding
    // to playerID is finished
    [Rpc]
    public static void RPC_EndTurn(NetworkRunner runner,
        [RpcTarget] PlayerRef player, 
        TurnEndData turnEndData)
    {
        EventBus.Publish(new TurnFinishedEventServer(turnEndData));
    }

    // Requests the server to create a new unit
    [Rpc]
    public static void RPC_CreateUnit(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        CreateUnitRequest request)
    {
        Debug.Log("Calling RPC_CreateUnit");
        EventBus.Publish(request);
    }

    // Requests the server to move a unit
    [Rpc]
    public static void RPC_MoveUnit(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        MoveUnitRequest request)
    {
        Debug.Log("Calling RPC_MoveUnit");
        EventBus.Publish(request);
    }
}
