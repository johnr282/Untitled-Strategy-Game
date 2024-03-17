using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Used exclusively by the server to manage players and turn order
// ------------------------------------------------------------------

public class PlayerManager : NetworkBehaviour
{
    // Maps player IDs to players
    Dictionary<int, Player> _players = new Dictionary<int, Player>();

    // Contains player IDs
    List<int> _turnOrder = new List<int>();

    // Contains an index to _turnOrder
    int _currTurnIndex = 0;             

    Subscription<TurnFinishedEvent> _turnFinishedSub;

    void Start()
    {
        _turnFinishedSub = EventBus.Subscribe<TurnFinishedEvent>(
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
        int newPlayerID = _players.Count + 1;
        _players.Add(newPlayerID, new Player(player, newPlayerID));
        _turnOrder.Add(newPlayerID);
        ServerMessages.RPC_SendPlayerID(Runner,
            player,
            newPlayerID);
    }

    // Notifies the first player in _turnOrder that it's their turn
    public void NotifyFirstPlayer()
    {
        NotifyNextPlayer();
    }

    // Updates _currTurnIndex and notifies next player that it's their turn
    void TurnFinishedCallback(TurnFinishedEvent turnFinishedEvent)
    {
        UpdateCurrTurnIndex();
        NotifyNextPlayer();
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
    void NotifyNextPlayer()
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
            nextPlayer.ClientRef);
    }
}
