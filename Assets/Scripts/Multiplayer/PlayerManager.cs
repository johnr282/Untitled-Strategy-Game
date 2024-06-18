using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Static class used to manage players and turn order; part of the
// global game state
// ------------------------------------------------------------------

public static class PlayerManager
{
    // Returns the calling client's player ID; this value will be different for
    // each client
    public static PlayerID MyPlayerID
    {
        get
        {
            if (_myPlayerID.ID == -1)
                throw new RuntimeException("MyPlayerID not set");
            return _myPlayerID;
        }
        set
        {
            _myPlayerID = value;
        }
    }

    public static int NumPlayers { get => _players.Count; }

    // The player whose turn it currently is
    public static PlayerID ActivePlayer { get => _turnOrder[_currTurnIndex]; }

    // Returns whether it's the calling client's turn
    public static bool MyTurn { get => MyPlayerID.ID == ActivePlayer.ID; }

    // Player ID is index into players list
    static List<Player> _players = new();

    static List<PlayerID> _turnOrder = new();

    // Contains an index to _turnOrder
    static int _currTurnIndex = 0;

    static PlayerID _myPlayerID = new(-1);

    // Creates a new player, and returns the PlayerID of the new player
    // Modifies game state
    public static PlayerID AddPlayer(PlayerRef player)
    {
        PlayerID newPlayerID = new(NumPlayers);
        _players.Add(new Player(player, newPlayerID));
        _turnOrder.Add(newPlayerID);
        return newPlayerID;
    }

    // Returns the Player corresponding to the given playerID
    // Throws an ArgumentException if playerID is invalid
    public static Player GetPlayer(PlayerID playerID)
    {
        if (playerID.ID >= NumPlayers)
            throw new ArgumentException("Invalid player ID");

        return _players[playerID.ID];
    }

    // If called on the ActivePlayer, notifies them that it's their turn
    public static void NotifyActivePlayer()
    {
        if (MyPlayerID.ID == ActivePlayer.ID)
            EventBus.Publish(new MyTurnEvent());
    }

    // Requests the server to end this player's turn
    public static void EndMyTurn()
    {
        ClientRequestManager.QueueClientRequest(new EndTurnRequest(MyPlayerID),
            ClientMessages.RPC_EndTurn);
    }

    // Updates the ActivePlayer by incrementing _currTurnIndex or wrapping it
    // back around to 0
    // Modifies game state
    public static void UpdateCurrTurnIndex()
    {
        if (_currTurnIndex >= (_turnOrder.Count - 1))
            _currTurnIndex = 0;
        else
            _currTurnIndex++;
    }

    // Returns whether the given EndTurnRequest is valid
    [RequestValidator(typeof(EndTurnRequest))]
    public static bool ValidateEndTurnAction(EndTurnRequest action)
    {
        return action.EndingPlayerID.ID == ActivePlayer.ID;
    }
}