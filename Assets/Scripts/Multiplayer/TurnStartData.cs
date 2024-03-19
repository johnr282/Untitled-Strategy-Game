using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Stores data sent to a client when it becomes their turn
// ------------------------------------------------------------------

public struct TurnStartData : INetworkStruct
{
    // True if this if the first turn of the game
    public NetworkBool FirstTurn { get; }      
    public Vector3Int PreviousPlayerHex { get; }

    public TurnStartData(NetworkBool firstTurnIn, 
        Vector3Int previousPlayerHexIn = new Vector3Int())
    {
        FirstTurn = firstTurnIn;
        PreviousPlayerHex = previousPlayerHexIn;
    }
}
