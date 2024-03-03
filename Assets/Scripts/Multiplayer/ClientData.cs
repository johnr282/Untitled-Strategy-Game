using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Used by client to store relevant data
// ------------------------------------------------------------------

public class ClientData : MonoBehaviour
{
    int _playerID;

    Subscription<PlayerIDReceivedEvent> _playerIDSub;

    void Start()
    {
        _playerIDSub = EventBus.Subscribe<PlayerIDReceivedEvent>(PlayerIDCallback);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe(_playerIDSub);    
    }

    // Sets _playerID to ID given in playerIDEvent
    void PlayerIDCallback(PlayerIDReceivedEvent playerIDEvent)
    {
        _playerID = playerIDEvent.PlayerID;
    }
}
