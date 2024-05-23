using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Used exclusively by the server to manage players and turn order
// ------------------------------------------------------------------

public struct PlayerID : INetworkStruct
{
    public ushort ID { get; }

    public PlayerID (ushort idIn)
    {
        ID = idIn;
    }
}

public class PlayerManager : NetworkBehaviour
{
    // Player ID is index into players list
    List<Player> _players = new();

    // Maps player IDs to players
    //Dictionary<int, Player> _players = new();

    int _numPlayers { get => _players.Count; }

    List<PlayerID> _turnOrder = new();

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
        PlayerID newPlayerID = new PlayerID((ushort)_numPlayers);
        _players.Add(new Player(player, newPlayerID));
        _turnOrder.Add(newPlayerID);
    }

    // Spawns a unit and sends a game start message for each player
    public void NotifyGameStart(List<GameTile> startingTiles)
    {
        Debug.Log("All players joined");

        foreach (Player player in _players)
        {
            GameTile startingTile = startingTiles[player.PlayerID.ID];

            EventBus.Publish(new CreateUnitRequest(UnitType.land,
                startingTile.Hex,
                player.PlayerID));

            ServerMessages.RPC_GameStart(Runner,
                player.PlayerRef,
                new GameStartData(player.PlayerID,
                    startingTile));
        }

        //NotifyFirstPlayer();
    }



    // Notifies the first player in _turnOrder that it's their turn
    public void NotifyFirstPlayer()
    {
        NotifyNextPlayer(new TurnStartData(true));
    }

    // Returns the Player corresponding to the given playerID
    // Throws an ArgumentException if playerID is invalid
    public Player GetPlayer(PlayerID playerID)
    {
        if (playerID.ID >= _numPlayers)
            throw new ArgumentException(
                "Invalid player ID");

        return _players[playerID.ID];
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

    // Notifies the next player that it's their turns and sends them the given
    // turn start data
    void NotifyNextPlayer(TurnStartData turnStartData)
    {
        PlayerID nextPlayerID = _turnOrder[_currTurnIndex];
        Player nextPlayer = GetPlayer(nextPlayerID);

        ServerMessages.RPC_NotifyPlayerTurn(Runner, 
            nextPlayer.PlayerRef,
            turnStartData);
    }
}