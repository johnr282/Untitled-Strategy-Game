using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains RPCs used by clients to communicate with the server
// ------------------------------------------------------------------

public class ClientMessages : NetworkBehaviour  
{
    //// Called by a client to notify server that this player is ending their turn
    //[Rpc]
    //public static void RPC_EndTurn(NetworkRunner runner,
    //    [RpcTarget] PlayerRef player,
    //    EndTurnRequest action)
    //{
    //    Debug.Log("Received EndTurnAction from player " + action.EndingPlayerID);

    //    if (PlayerManager.ValidateEndTurnUpdate(action))
    //    {
    //        Debug.Log("Action successful");
    //        GameStateManager.UpdateGameState(runner,
    //            new EndTurnUpdate(),
    //            ServerMessages.RPC_NextTurn);
    //    }
    //    else
    //    {
    //        Debug.Log("Action denied");
    //    }
    //}

    ////// Requests the server to create a new unit
    ////[Rpc]
    ////public static void RPC_CreateUnit(NetworkRunner runner,
    ////    [RpcTarget] PlayerRef player,
    ////    CreateUnitRequest action)
    ////{
    ////    Debug.Log("Received CreateUnitAction from player " +
    ////        action.RequestingPlayerID + " at " + action.Location);

    ////    if (UnitManager.ValidateCreateUnitUpdate(action))
    ////    {
    ////        Debug.Log("Action successful");
    ////        CreateUnitUpdate unitCreated = new(action);
    ////        GameStateManager.UpdateGameState(runner,
    ////            unitCreated,
    ////            ServerMessages.RPC_UnitCreated);
    ////    }
    ////    else
    ////    {
    ////        Debug.Log("Action denied");
    ////    }
    ////}

    //// Requests the server to move a unit; if the MoveUnitRequest is valid, 
    //// updates the game state accordingly
    //[Rpc]
    //public static void RPC_MoveUnit(NetworkRunner runner,
    //    [RpcTarget] PlayerRef player,
    //    MoveUnitRequest action)
    //{
    //    Debug.Log("Received MoveUnitRequest from player " +
    //        action.RequestingPlayerID);

    //    if (UnitManager.ValidateMoveUnitUpdate(action))
    //    {
    //        Debug.Log("Request is valid");

    //        MoveUnitUpdate unitMoved = new(action.UnitID,
    //            action.Location);
    //        GameStateManager.UpdateGameState(runner,
    //            unitMoved,
    //            ServerMessages.RPC_UnitMoved);
    //    }
    //    else
    //    {
    //        Debug.Log("Action denied");
    //    }
    //}
}
