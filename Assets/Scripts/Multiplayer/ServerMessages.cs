using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains RPCs used by the server to communicate with clients
// ------------------------------------------------------------------

public class ServerMessages : NetworkBehaviour
{
    // Called once all players join to notify a client that the game has started
    [Rpc]
    public static void RPC_StartGame(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        GameStarted gameStartData)
    {
        Debug.Log("RPC_GameStart called on player " + 
            gameStartData.PlayerID.ToString());

        EventBus.Publish(gameStartData.PlayerID);
    }

    // Notifies a client that it's their turn
    [Rpc]
    public static void RPC_StartTurn(NetworkRunner runner,
        [RpcTarget] PlayerRef player, 
        TurnStarted turnStarted)
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