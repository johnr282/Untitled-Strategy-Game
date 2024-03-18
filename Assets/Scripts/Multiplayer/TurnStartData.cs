using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Stores data sent to a client when it becomes their turn
// ------------------------------------------------------------------

public class TurnStartData 
{
    public Vector3Int PreviousPlayerHex { get; }

    public TurnStartData(Vector3Int previousPlayerHexIn)
    {
        PreviousPlayerHex = previousPlayerHexIn;
    }
}
