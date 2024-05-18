using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains event definitions used with EventBus system that are
// published on the server
// ------------------------------------------------------------------ 

// Published when a client finishes their turn
public class TurnFinishedEventServer
{
    public TurnEndData TurnEndInfo { get; }

    public TurnFinishedEventServer(TurnEndData turnEndInfoIn)
    {
        TurnEndInfo = turnEndInfoIn;
    }
}