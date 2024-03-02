using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains RPCs used by the server to send messages to clients
// ------------------------------------------------------------------

public class ServerMessages : NetworkBehaviour
{

    // Called by server to generate map with given seed for specified player
    [Rpc(sources: RpcSources.StateAuthority, 
        targets: RpcTargets.All, 
        HostMode = RpcHostMode.SourceIsServer)]
    public static void RPC_GenerateMap(NetworkRunner runner, 
        [RpcTarget] PlayerRef player, 
        int mapSeed) 
    {
        Debug.Log("Calling RPC_GenerateMap");
        EventBus.Publish(new GenerateMapEvent(mapSeed));
    }
}