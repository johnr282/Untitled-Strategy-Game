using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component used to manage players and turn order
// ------------------------------------------------------------------

public class PlayerManager : NetworkBehaviour
{
    public int NumPlayers { get => _players.Count; }

    // Player ID is index into players list
    List<Player> _players = new();

    List<PlayerID> _turnOrder = new();

    // Contains an index to _turnOrder
    int _currTurnIndex = 0;

    // Stores the calling client's player ID; this value will be different for
    // each client
    public PlayerID ThisPlayerID
    {
        get
        {
            if (_thisPlayerID.ID == -1)
                throw new RuntimeException("ThisPlayerID not set");
            return _thisPlayerID;
        }
    }
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
        //ServerMessages.RPC_SendPlayerID(Runner,
        //    player,
        //    newPlayerID);
    }

    // Spawns a unit and sends a game start message for each player
    public void NotifyGameStart(List<GameTile> startingTiles)
    {
        //Debug.Log("All players joined");

        //foreach (Player player in _players)
        //{
        //    GameTile startingTile = startingTiles[player.PlayerID.ID];

        //    EventBus.Publish(new CreateUnitRequest(UnitType.land,
        //        startingTile.Hex,
        //        player.PlayerID));

        //    ServerMessages.RPC_StartGame(Runner,
        //        player.PlayerRef,
        //        new GameStarted(player.PlayerID,
        //            startingTile.Hex));
        //}

        //NotifyFirstPlayer();
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