using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Static class used to manage players and turn order; part of the
// global game state
// ------------------------------------------------------------------

public class PlayerManager : NetworkBehaviour
{
    // Stores the calling client's player ID; this value will be different for
    // each client
    public static PlayerID ThisPlayerID
    {
        get
        {
            if (_thisPlayerID.ID == -1)
                throw new RuntimeException("ThisPlayerID not set");
            return _thisPlayerID;
        }
    }

    public int NumPlayers { get => _players.Count; }

    // The player whose turn it currently is
    public PlayerID ActivePlayer { get => _turnOrder[_currTurnIndex]; }

    // Player ID is index into players list
    List<Player> _players = new();

    List<PlayerID> _turnOrder = new();

    // Contains an index to _turnOrder
    int _currTurnIndex = 0;

    static PlayerID _thisPlayerID = new(-1);


    void Start()
    {
        EventBus.Subscribe<PlayerID>(SetPlayerID);
    }

    void SetPlayerID(PlayerID playerID)
    {
        _thisPlayerID = playerID;
    }

    // Creates a new player, and returns the PlayerID of the new player
    public PlayerID AddPlayer(PlayerRef player)
    {
        PlayerID newPlayerID = new(NumPlayers);
        _players.Add(new Player(player, newPlayerID));
        _turnOrder.Add(newPlayerID);
        return newPlayerID;
    }

    // Notifies the first player in _turnOrder that it's their turn
    //public void NotifyFirstPlayer()
    //{
    //    NotifyNextPlayer(new TurnStartData(true));
    //}

    // Returns the Player corresponding to the given playerID
    // Throws an ArgumentException if playerID is invalid
    public Player GetPlayer(PlayerID playerID)
    {
        if (playerID.ID >= NumPlayers)
            throw new ArgumentException("Invalid player ID");

        return _players[playerID.ID];
    }

    // Updates _currTurnIndex and notifies next player that it's their turn
    //void TurnFinishedCallback(TurnFinishedEventServer turnFinishedEvent)
    //{
    //    UpdateCurrTurnIndex();
    //    NotifyNextPlayer(new TurnStartData(false,
    //        turnFinishedEvent.TurnEndInfo.SelectedHex));
    //}

    // Increments _currTurnIndex or wraps it back around to 0
    void UpdateCurrTurnIndex()
    {
        if (_currTurnIndex >= (_turnOrder.Count - 1))
            _currTurnIndex = 0;
        else
            _currTurnIndex++;

        Debug.Log("Updated curr turn index to : " +  _currTurnIndex);
    }

    // Notifies the next player that it's their turns and sends them the given
    // turn start data
    //void NotifyNextPlayer(TurnStartData turnStartData)
    //{
    //    PlayerID nextPlayerID = _turnOrder[_currTurnIndex];
    //    Player nextPlayer = GetPlayer(nextPlayerID);

    //    ServerMessages.RPC_NotifyPlayerTurn(Runner, 
    //        nextPlayer.PlayerRef,
    //        turnStartData);
    //}
}