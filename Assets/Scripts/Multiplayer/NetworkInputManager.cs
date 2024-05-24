using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component that calls the corresponding RPC for each published
// NetworkInputEvent for clients in the Unity Update() function.
// This is necessary because RPCs will fail if they are called in
// the client's resimulation phase, which should never be the case
// in Update()
// This isn't needed for RPCs called by the server because the server
// doesn't have a resimulation phase
// ------------------------------------------------------------------

public class NetworkInputManager : NetworkBehaviour
{
    Queue<NetworkInputEvent> _inputEventQueue = new();

    void Start()
    {    
        EventBus.Subscribe<NetworkInputEvent>(OnInputEvent);
    }

    void OnInputEvent(NetworkInputEvent inputEvent)
    {
        Debug.Log("Adding new input event to queue");
        _inputEventQueue.Enqueue(inputEvent);
    }

    void Update()
    {
        if (_inputEventQueue.TryDequeue(out NetworkInputEvent inputEvent))
        {
            Debug.Log("New event in queue, calling RPC");
            inputEvent.CallRPC();
        }    
    }
}