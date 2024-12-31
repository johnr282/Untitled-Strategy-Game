using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Simple StateManager test
// ------------------------------------------------------------------

public class TestStateManager : SimulationBehaviour
{
    [SerializeField] int val = 0;

    void Start()
    {
        StateManager.RegisterStateUpdate<TestStateUpdate>(Validate,
            PerformUpdate,
            StateManagerRPCs.RPC_TestStateUpdateServer,
            StateManagerRPCs.RPC_TestStateUpdateClient);
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
}