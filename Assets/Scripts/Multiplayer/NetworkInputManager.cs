using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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