using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// ------------------------------------------------------------------
// Component that stores player data unique to each client
// ------------------------------------------------------------------

public class ClientPlayerData : NetworkBehaviour
{
    public PlayerID PlayerID { get; private set; }
    public bool MyTurn {  get; private set; }

    //Subscription<PlayerIDReceivedEvent> _playerIDSub;
    //Subscription<PlayerTurnEvent> _turnSub;

    //void Start()
    //{
    //    MyTurn = false;

    //    _playerIDSub = EventBus.Subscribe<PlayerIDReceivedEvent>(PlayerIDCallback);
    //    _turnSub = EventBus.Subscribe<PlayerTurnEvent>(PlayerTurnCallback);
    //}

    //void OnDestroy()
    //{
    //    EventBus.Unsubscribe(_playerIDSub);
    //    EventBus.Unsubscribe(_turnSub);
    //}

    //public void EndTurn(TurnEndData turnEndData)
    //{
    //    Debug.Log("Player " + PlayerID + " ending turn");
    //    MyTurn = false;

    //    ClientMessages.RPC_EndTurn(Runner,
    //        PlayerRef.None,
    //        turnEndData);
    //    EventBus.Publish(new TurnFinishedEventClient(turnEndData));
    //}

    //// Sets PlayerID to ID given in playerIDEvent
    //void PlayerIDCallback(PlayerIDReceivedEvent playerIDEvent)
    //{
    //    PlayerID = playerIDEvent.PlayerID;
    //}

    //// Sets MyTurn to true 
    //void PlayerTurnCallback(PlayerTurnEvent turnEvent)
    //{
    //    MyTurn = true;
    //    Debug.Log("Player " + PlayerID.ToString() + ": It's my turn!");
    //}
}