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
    public static void RPC_GameStart(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        GameStartData gameStartData)
    {
        Debug.Log("RPC_GameStart called on player " + 
            gameStartData.PlayerID.ToString());

        EventBus.Publish(new PlayerIDReceivedEvent(gameStartData.PlayerID));
    }

    // Notifies a client that it's their turn
    [Rpc]
    public static void RPC_NotifyPlayerTurn(NetworkRunner runner,
        [RpcTarget] PlayerRef player, 
        TurnStartData turnStartData)
    {
        EventBus.Publish(new PlayerTurnEvent(turnStartData));
    }

    //// Notifies a client about the success of their create unit request; success
    //// is true if the unit was successfuly created, false if not
    //[Rpc]
    //public static void RPC_CreateUnitResponse(NetworkRunner runner,
    //    [RpcTarget] PlayerRef player, 
    //    CreateUnitRequest createUnitRequest,
    //    bool success)
    //{
    //    EventBus.Publish(new CreateUnitResponseEvent(createUnitRequest, success));
    //}
}