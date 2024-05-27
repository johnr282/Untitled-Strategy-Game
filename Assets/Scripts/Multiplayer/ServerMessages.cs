using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ------------------------------------------------------------------
// Contains RPCs used by the server to communicate with clients
// ------------------------------------------------------------------

public class ServerMessages : NetworkBehaviour
{
    // Sent to a player when they join to give them their player ID
    [Rpc]
    public static void RPC_SendPlayerID(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        PlayerID playerID)
    {
        EventBus.Publish(playerID);
    }

    [Rpc]
    public static void RPC_AddPlayer(NetworkRunner runner,
        AddPlayer addPlayer)
    {
        EventBus.Publish(addPlayer);
    }

    // Called once all players join to notify a client that the game has started
    [Rpc]
    public static void RPC_StartGame(NetworkRunner runner,
        GameStarted gameStartData)
    {
        EventBus.Publish(gameStartData);
    }

    // Notifies a client that it's their turn
    [Rpc]
    public static void RPC_StartTurn(NetworkRunner runner,
        [RpcTarget] PlayerRef player, 
        TurnChanged turnStarted)
    {
        EventBus.Publish(turnStarted);
    }

    // Notifies clients that a unit was created
    [Rpc]
    public static void RPC_UnitCreated(NetworkRunner runner,
        UnitCreated unitCreated)
    {
        EventBus.Publish(unitCreated);
    }

    // Notifies clients that a unit was moved
    [Rpc]
    public static void RPC_UnitMoved(NetworkRunner runner,
        UnitMoved unitMoved)
    {
        EventBus.Publish(unitMoved);
    }
}