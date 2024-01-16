using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains Event definitions used with EventBus system
// ------------------------------------------------------------------

public class NewTileSelectedEvent
{
    public Vector3Int coordinate { get; }

    public NewTileSelectedEvent(Vector3Int coordinateIn)
    {
        coordinate = coordinateIn;
    }
}
