using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Used exclusively by the server to manage players and turn order
// ------------------------------------------------------------------

public class PlayerManager : NetworkBehaviour
{
    Dictionary<int, Player> _players;   // maps player IDs to players
    List<int> _turnOrder;               // contains player IDs
    int _currTurnIndex = 0;             // contains an index to _turnOrder

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
    public void AddPlayer(NetworkRunner runner, 
        PlayerRef player)
    {
        int newPlayerID = _players.Count + 1;
        _players.Add(newPlayerID, new Player(player, newPlayerID));
        _turnOrder.Add(newPlayerID);
        ServerMessages.RPC_SendPlayerID(runner,
            player,
            newPlayerID);
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

        
    }
}
