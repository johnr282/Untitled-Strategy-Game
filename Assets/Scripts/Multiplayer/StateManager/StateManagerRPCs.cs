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
    // AddPlayerUpdate RPCs
    [Rpc]
    public static void RPC_AddPlayerServer(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        AddPlayerUpdate updateData)
    {
        StateManager.UpdateServerState(updateData);
    }

    [Rpc]
    public static void RPC_AddPlayerClient(NetworkRunner runner,
        AddPlayerUpdate updateData)
    {
        StateManager.UpdateClientState(updateData);
    }

    // StartGameUpdate RPCs
    [Rpc]
    public static void RPC_StartGameServer(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        StartGameUpdate updateData)
    {
        StateManager.UpdateServerState(updateData);
    }

    [Rpc]
    public static void RPC_StartGameClient(NetworkRunner runner,
        StartGameUpdate updateData)
    {
        StateManager.UpdateClientState(updateData);
    }

    // CreateUnitUpdate RPCs
    [Rpc]
    public static void RPC_CreateUnitServer(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        CreateUnitUpdate updateData)
    {
        StateManager.UpdateServerState(updateData);
    }

    [Rpc]
    public static void RPC_CreateUnitClient(NetworkRunner runner,
        CreateUnitUpdate updateData)
    {
        StateManager.UpdateClientState(updateData);
    }
}