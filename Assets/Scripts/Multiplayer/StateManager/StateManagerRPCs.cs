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

    // EndTurnUpdate RPCs
    [Rpc]
    public static void RPC_EndTurnServer(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        EndActivePlayersTurnUpdate updateData)
    {
        StateManager.UpdateServerState(updateData);
    }

    [Rpc]
    public static void RPC_EndTurnClient(NetworkRunner runner,
        EndActivePlayersTurnUpdate updateData)
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

    // MoveUnitUpdate RPCs
    [Rpc]
    public static void RPC_MoveUnitServer(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        MoveUnitUpdate updateData)
    {
        StateManager.UpdateServerState(updateData);
    }

    [Rpc]
    public static void RPC_MoveUnitClient(NetworkRunner runner,
        MoveUnitUpdate updateData)
    {
        StateManager.UpdateClientState(updateData);
    }

    // PlaceTerritorySelectionUnitUpdate RPCs
    [Rpc]
    public static void RPC_PlaceTerritorySelectionUnitServer(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        PlaceTerritorySelectionUnitUpdate updateData)
    {
        StateManager.UpdateServerState(updateData);
    }

    [Rpc]
    public static void RPC_PlaceTerritorySelectionUnitClient(NetworkRunner runner,
        PlaceTerritorySelectionUnitUpdate updateData)
    {
        StateManager.UpdateClientState(updateData);
    }

    // CreateStructureUpdate RPCs
    [Rpc]
    public static void RPC_CreateStructureServer(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        CreateStructureUpdate updateData)
    {
        StateManager.UpdateServerState(updateData);
    }

    [Rpc]
    public static void RPC_CreateStructureClient(NetworkRunner runner,
        CreateStructureUpdate updateData)
    {
        StateManager.UpdateClientState(updateData);
    }

    // PlaceCapitalUpdate RPCs
    [Rpc]
    public static void RPC_PlaceCapitalServer(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        PlaceCapitalUpdate updateData)
    {
        StateManager.UpdateServerState(updateData);
    }

    [Rpc]
    public static void RPC_PlaceCapitalClient(NetworkRunner runner,
        PlaceCapitalUpdate updateData)
    {
        StateManager.UpdateClientState(updateData);
    }
}