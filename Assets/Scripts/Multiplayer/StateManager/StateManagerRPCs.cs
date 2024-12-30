using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains all RPCs used with the StateManager
// Must inherit from NetworkBehaviour or RPCs won't work
// ------------------------------------------------------------------

public class StateManagerRPCs: NetworkBehaviour
{
    [Rpc]
    public static void RPC_ServerTestStateUpdate(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        string registrationString,
        TestStateUpdate updateData)
    {
        StateManager.UpdateServerState(registrationString, updateData);
    }

    [Rpc]
    public static void RPC_ClientTestStateUpdate(NetworkRunner runner,
        string registrationString,
        TestStateUpdate updateData)
    {
        StateManager.UpdateClientState(registrationString, updateData);
    }
}
