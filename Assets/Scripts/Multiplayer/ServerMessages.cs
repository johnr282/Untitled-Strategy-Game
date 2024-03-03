using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains RPCs used by the server to send messages to clients
// ------------------------------------------------------------------

public class ServerMessages : NetworkBehaviour
{
    // Called by server to send a client their player ID
    [Rpc]
    public static void RPC_SendPlayerID(NetworkRunner runner,
        [RpcTarget] PlayerRef player, 
        int playerID)
    {
        Debug.Log("Calling RPC_SendPlayerID");
        EventBus.Publish(new PlayerIDReceivedEvent(playerID));
    }

    // Called by server to send map seed to all clients so each client can
    // generate the map
    [Rpc]
    public static void RPC_GenerateMap(NetworkRunner runner, 
        int mapSeed) 
    {
        Debug.Log("Calling RPC_GenerateMap");
        EventBus.Publish(new GenerateMapEvent(mapSeed));
    }

    // Called by server to notify a player that it's their turn
    [Rpc]
    public static void RPC_NotifyPlayerTurn(NetworkRunner runner,
        [RpcTarget] PlayerRef player)
    {
        Debug.Log("Calling RPC_NotifyPlayerTurn");
        EventBus.Publish(new PlayerTurnEvent());
    }
}