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
    // Sends each player their PlayerID as they join
    [Rpc]
    public static void RPC_SendPlayerID(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        PlayerID playerID)
    {
        PlayerManager.ThisPlayerID = playerID;
    }

    [Rpc]
    public static void RPC_AddPlayer(NetworkRunner runner,
        AddPlayer addPlayer)
    {
        EventBus.Publish(addPlayer);
    }

    [Rpc]
    public static void RPC_StartGame(NetworkRunner runner,
        GameStarted gameStartData)
    {
        EventBus.Publish(gameStartData);
    }

    [Rpc]
    public static void RPC_NextTurn(NetworkRunner runner,
        NextTurn nextTurn)
    {
        EventBus.Publish(nextTurn);
    }

    [Rpc]
    public static void RPC_UnitCreated(NetworkRunner runner,
        UnitCreated unitCreated)
    {
        EventBus.Publish(unitCreated);
    }

    [Rpc]
    public static void RPC_UnitMoved(NetworkRunner runner,
        UnitMoved unitMoved)
    {
        EventBus.Publish(unitMoved);
    }
}