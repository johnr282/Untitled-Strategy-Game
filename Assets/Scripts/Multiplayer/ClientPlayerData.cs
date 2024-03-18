using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// ------------------------------------------------------------------
// Used by client to store player data
// ------------------------------------------------------------------

public class ClientPlayerData : NetworkBehaviour
{
    public int PlayerID { get; private set; }
    public bool MyTurn {  get; private set; }

    Subscription<PlayerIDReceivedEvent> _playerIDSub;
    Subscription<PlayerTurnEvent> _turnSub;

    void Start()
    {
        MyTurn = false;

        _playerIDSub = EventBus.Subscribe<PlayerIDReceivedEvent>(PlayerIDCallback);
        _turnSub = EventBus.Subscribe<PlayerTurnEvent>(PlayerTurnCallback);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe(_playerIDSub);
        EventBus.Unsubscribe(_turnSub);
    }

    public void EndTurn(Vector3Int selectedHex)
    {
        Debug.Log("Player " + PlayerID + " ending turn");
        MyTurn = false;

        ClientMessages.RPC_EndTurn(Runner,
            PlayerRef.None,
            PlayerID);
    }

    // Sets PlayerID to ID given in playerIDEvent
    void PlayerIDCallback(PlayerIDReceivedEvent playerIDEvent)
    {
        PlayerID = playerIDEvent.PlayerID;
    }

    // Sets MyTurn to true 
    void PlayerTurnCallback(PlayerTurnEvent turnEvent)
    {
        MyTurn = true;
        Debug.Log("Player " + PlayerID.ToString() + ": It's my turn!");
    }
}