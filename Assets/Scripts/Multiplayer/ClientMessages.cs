using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains RPCs used by clients to send messages to the server
// ------------------------------------------------------------------

public class ClientMessages : NetworkBehaviour  
{
    // Called by a client to notify server that the player's turn
    // corresponding to playerID is finished
    [Rpc]
    public static void RPC_EndTurn(NetworkRunner runner,
        [RpcTarget] PlayerRef player, 
        int playerID)
    {
        Debug.Log("Calling RPC_EndTurn");
        EventBus.Publish(new TurnFinishedEvent(playerID));
    }

    // Called by a client to send its selected hex to the server
    [Rpc]
    public static void RPC_SendSelectedHex(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        Vector3Int selectedHex)
    {
        Debug.Log("Calling RPC_SendSelectedHex");

    }
}
