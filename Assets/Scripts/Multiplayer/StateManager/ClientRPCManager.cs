using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// ------------------------------------------------------------------
// Component that calls queued RPCs in the Unity Update() function.
// This is necessary because RPCs will fail if they are called in
// the client's resimulation phase, which should never be the case
// in Update(). This isn't needed for RPCs called by the server because
// the server doesn't have a resimulation phase.
// ------------------------------------------------------------------

public class ClientRPCManager : NetworkBehaviour
{
    class ClientRPC
    {
        Delegate _rpc;
        object[] _parameters;

        public ClientRPC(Delegate rpcIn,
            params object[] parametersIn)
        {
            _rpc = rpcIn;
            _parameters = parametersIn;
        }

        public void CallRPC()
        {
            _rpc.DynamicInvoke(_parameters);
        }
    }

    static Queue<ClientRPC> _clientRPCQueue = new();

    // Creates a ClientRPC object with the given RPC and parameters and
    // adds it to the queue
    // Throws exception if rpcParameters doesn't match the number of
    // parameters or parameter types of clientRPC
    public static void QueueClientRPC(Delegate clientRPC,
        params object[] rpcParameters)
    {
        // Check that types in rpcParameters match the parameters of clientRPC for safety
        string exceptionMsg = "Given parameters do not match parameters of RPC";
        ParameterInfo[] rpcParameterInfo = clientRPC.GetMethodInfo().GetParameters();

        if (rpcParameterInfo.Length != rpcParameters.Length)
            throw new RuntimeException(exceptionMsg);

        for (int i = 0; i < rpcParameters.Length; i++)
        {
            Type requiredType = rpcParameterInfo[i].ParameterType;
            Type givenType = rpcParameters[i].GetType();
            if (requiredType != givenType)
                throw new RuntimeException(exceptionMsg);
        }

        _clientRPCQueue.Enqueue(new ClientRPC(clientRPC, rpcParameters));
    }

    void Update()
    {
        if (_clientRPCQueue.TryDequeue(out ClientRPC clientRPC))
        {
            Debug.Log("Calling client RPC in queue");
            clientRPC.CallRPC();
        }    
    }
}