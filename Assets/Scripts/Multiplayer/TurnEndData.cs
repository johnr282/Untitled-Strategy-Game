using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Stores data sent to server when a player finishes their turn
// ------------------------------------------------------------------

public struct TurnEndData : INetworkStruct
{
    public int PlayerID { get; }
    public Vector3Int SelectedHex {  get; }

    public TurnEndData(int playerIDIn, 
        Vector3Int selectedHexIn)
    {
        PlayerID = playerIDIn;
        SelectedHex = selectedHexIn;
    }
}
