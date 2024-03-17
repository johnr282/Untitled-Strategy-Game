using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// ------------------------------------------------------------------
// Used by client to store player data
// ------------------------------------------------------------------

public class ClientPlayerData : NetworkBehaviour
{
    int _playerID;
    bool _myTurn = false;

    Subscription<PlayerIDReceivedEvent> _playerIDSub;
    Subscription<PlayerTurnEvent> _turnSub;

    void Start()
    {
        _playerIDSub = EventBus.Subscribe<PlayerIDReceivedEvent>(PlayerIDCallback);
        _turnSub = EventBus.Subscribe<PlayerTurnEvent>(PlayerTurnCallback);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe(_playerIDSub);
        EventBus.Unsubscribe(_turnSub);
    }

    // Sets _playerID to ID given in playerIDEvent
    void PlayerIDCallback(PlayerIDReceivedEvent playerIDEvent)
    {
        _playerID = playerIDEvent.PlayerID;
    }

    // Sets _myTurn to true 
    void PlayerTurnCallback(PlayerTurnEvent turnEvent)
    {
        _myTurn = true;
        Debug.Log("Player " + _playerID.ToString() + ": It's my turn!");
        _myTurn = false;

        ClientMessages.RPC_EndTurn(Runner, 
            PlayerRef.None, 
            _playerID);
    }
}