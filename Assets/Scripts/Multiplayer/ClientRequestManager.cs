using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component that calls the corresponding RPC for each published
// ClientRequest in the Unity Update() function.
// This is necessary because RPCs will fail if they are called in
// the client's resimulation phase, which should never be the case
// in Update(). This isn't needed for RPCs called by the server because
// the server doesn't have a resimulation phase.
// ------------------------------------------------------------------

public class ClientRequestManager : NetworkBehaviour
{
    Queue<ClientRequest> _clientRequestQueue = new();

    void Start()
    {    
        EventBus.Subscribe<ClientRequest>(OnClientRequest);
    }

    // Constructs and publishes a ClientRequest with the given input data
    // and RPC
    public static void QueueClientRequest<TRequestData>(TRequestData actionData,
        Action<NetworkRunner, PlayerRef, TRequestData> RPC_SendClientRequest)
        where TRequestData : struct, INetworkStruct
    {
        EventBus.Publish(new ClientRequest(actionData,
            RPC_SendClientRequest));
    }

    void OnClientRequest(ClientRequest clientRequest)
    {
        Debug.Log("Adding new client request to queue");
        _clientRequestQueue.Enqueue(clientRequest);
    }

    void Update()
    {
        if (_clientRequestQueue.TryDequeue(out ClientRequest request))
        {
            Debug.Log("New event in queue, calling RPC");
            request.CallRPC(Runner);
        }    
    }
}