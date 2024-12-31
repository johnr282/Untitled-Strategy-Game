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
    public static void RPC_TestStateUpdateServer(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        TestStateUpdate updateData)
    {
        StateManager.UpdateServerState(updateData);
    }

    [Rpc]
    public static void RPC_TestStateUpdateClient(NetworkRunner runner,
        TestStateUpdate updateData)
    {
        StateManager.UpdateClientState(updateData);
    }

    [Rpc]
    public static void RPC_PlayerAddedServer(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        PlayerAdded updateData)
    {
        StateManager.UpdateServerState(updateData);
    }

    [Rpc]
    public static void RPC_PlayerAddedClient(NetworkRunner runner,
        PlayerAdded updateData)
    {
        StateManager.UpdateClientState(updateData);
    }
}