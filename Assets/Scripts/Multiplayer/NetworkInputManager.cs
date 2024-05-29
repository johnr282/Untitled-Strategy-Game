using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component that calls the corresponding RPC for each published
// ClientAction in the Unity Update() function.
// This is necessary because RPCs will fail if they are called in
// the client's resimulation phase, which should never be the case
// in Update(). This isn't needed for RPCs called by the server because
// the server doesn't have a resimulation phase.
// ------------------------------------------------------------------

public class ClientActionManager : NetworkBehaviour
{
    Queue<ClientAction> _clientActionQueue = new();

    void Start()
    {    
        EventBus.Subscribe<ClientAction>(OnClientAction);
    }

    // Constructs and publishes a ClientAction with the given input data
    // and RPC
    public static void QueueClientAction<TActionData>(TActionData actionData,
        Action<NetworkRunner, PlayerRef, TActionData> RPC_SendClientAction)
        where TActionData : struct, INetworkStruct
    {
        EventBus.Publish(new ClientAction(actionData,
            RPC_SendClientAction));
    }

    void OnClientAction(ClientAction clientAction)
    {
        Debug.Log("Adding new client action to queue");
        _clientActionQueue.Enqueue(clientAction);
    }

    void Update()
    {
        if (_clientActionQueue.TryDequeue(out ClientAction inputEvent))
        {
            Debug.Log("New event in queue, calling RPC");
            inputEvent.CallRPC(Runner);
        }    
    }
}