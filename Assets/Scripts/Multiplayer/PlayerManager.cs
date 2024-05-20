using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Used exclusively by the server to manage players and turn order
// ------------------------------------------------------------------

public class PlayerManager : NetworkBehaviour
{
    // Maps player IDs to players
    Dictionary<int, Player> _players = new();

    int _numPlayers { get => _players.Count; }

    // Contains player IDs
    List<int> _turnOrder = new();

    // Contains an index to _turnOrder
    int _currTurnIndex = 0;

    Subscription<TurnFinishedEventServer> _turnFinishedSub;

    void Start()
    {
        _turnFinishedSub = EventBus.Subscribe<TurnFinishedEventServer>(
            TurnFinishedCallback);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe(_turnFinishedSub);
    }

    // Creates a new player and corresponding ID, and sends player ID to
    // client corresponding to given PlayerRef
    public void AddPlayer(PlayerRef player)
    {
        int newPlayerID = _numPlayers;
        _players.Add(newPlayerID, new Player(player, newPlayerID));
        _turnOrder.Add(newPlayerID);
    }

    // Spawns a unit and sends a game start message for each player
    public void NotifyGameStart(List<GameTile> startingTiles)
    {
        Debug.Log("All players joined");

        foreach (KeyValuePair<int, Player> pair in _players)
        {
            int playerID = pair.Key;
            Player player = pair.Value;
            GameTile startingTile = startingTiles[playerID];

            EventBus.Publish(new CreateUnitRequest(UnitType.land,
                startingTile.Hex,
                playerID));

            ServerMessages.RPC_GameStart(Runner,
                player.PlayerRef,
                new GameStartData(playerID,
                    startingTile));
        }

        //NotifyFirstPlayer();
    }



    // Notifies the first player in _turnOrder that it's their turn
    public void NotifyFirstPlayer()
    {
        NotifyNextPlayer(new TurnStartData(true));
    }

    // Returns the PlayerRef corresponding to the given playerID
    // Throws an ArgumentException if playerID is invalid
    public PlayerRef GetPlayerRef(int playerID)
    {
        if (!_players.TryGetValue(playerID, out Player player))
            throw new ArgumentException(
                "Attempted to get PlayerRef of invalid player ID");

        return player.PlayerRef;
    }

    // Updates _currTurnIndex and notifies next player that it's their turn
    void TurnFinishedCallback(TurnFinishedEventServer turnFinishedEvent)
    {
        UpdateCurrTurnIndex();
        NotifyNextPlayer(new TurnStartData(false,
            turnFinishedEvent.TurnEndInfo.SelectedHex));
    }

    // Increments _currTurnIndex or wraps it back around to 0
    void UpdateCurrTurnIndex()
    {
        if (_currTurnIndex >= (_turnOrder.Count - 1))
            _currTurnIndex = 0;
        else
            _currTurnIndex++;

        Debug.Log("Updated curr turn index to : " +  _currTurnIndex);
    }

    // Notifies the next player that it's their turn
    void NotifyNextPlayer(TurnStartData turnStartData)
    {
        int nextPlayerID = _turnOrder[_currTurnIndex];
        bool playerNotFound = !_players.TryGetValue(nextPlayerID, 
            out Player nextPlayer);

        if (playerNotFound)
        {
            Debug.LogError("Player " + nextPlayerID + " not found");
            return;
        }

        ServerMessages.RPC_NotifyPlayerTurn(Runner, 
            nextPlayer.PlayerRef,
            turnStartData);
    }
}