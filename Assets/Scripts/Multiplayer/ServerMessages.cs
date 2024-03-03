using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains RPCs used by the server to send messages to clients
// ------------------------------------------------------------------

public class ServerMessages : NetworkBehaviour
{

    // Called by server to send map seed to all clients so each client can
    // generate the map
    [Rpc]
    public static void RPC_GenerateMap(NetworkRunner runner, 
        int mapSeed) 
    {
        Debug.Log("Calling RPC_GenerateMap");
        EventBus.Publish(new GenerateMapEvent(mapSeed));
    }
}