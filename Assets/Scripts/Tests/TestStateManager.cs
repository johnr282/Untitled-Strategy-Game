using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Simple StateManager test
// ------------------------------------------------------------------

public readonly struct TestStateUpdate : IStateUpdate { }

public class TestStateManager : SimulationBehaviour
{
    [SerializeField] int val = 0;

    void Start()
    {
        StateManager.RegisterStateUpdate<TestStateUpdate>(Validate,
            PerformUpdate,
            RPC_TestStateUpdateServer,
            RPC_TestStateUpdateClient);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StateManager.RequestStateUpdate(new TestStateUpdate());
        }
    }

    bool Validate(TestStateUpdate update)
    {
        Debug.Log("Validating update");
        return true;
    }

    void PerformUpdate(TestStateUpdate update)
    {
        val++;
        Debug.Log("Performed update, current value is " + val.ToString());
    }

    // TestStateUpdate RPCs
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
}