using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct TestStateUpdate : IStateUpdate { }

public class TestStateManager : NetworkBehaviour
{
    [SerializeField] int val = 0;
    string testStateUpdateRegistration = "TestStateUpdate";

    void Start()
    {
        StateManager.RegisterStateUpdate<TestStateUpdate>(testStateUpdateRegistration,
            Validate,
            PerformUpdate,
            RPC_ServerTestStateUpdate,
            RPC_ClientTestStateUpdate);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StateManager.RequestStateUpdate(testStateUpdateRegistration,
                new TestStateUpdate());
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

    [Rpc]
    static void RPC_ServerTestStateUpdate(NetworkRunner runner,
        [RpcTarget] PlayerRef player,
        string registrationString,
        TestStateUpdate updateData)
    {
        StateManager.UpdateServerState(registrationString, updateData);
    }

    [Rpc]
    static void RPC_ClientTestStateUpdate(NetworkRunner runner,
        string registrationString,
        TestStateUpdate updateData)
    {
        StateManager.UpdateClientState(registrationString, updateData);
    }
}