using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// ------------------------------------------------------------------
// Allows a client to select a hex on their turn
// ------------------------------------------------------------------

public class HexPicker : MonoBehaviour
{
    [SerializeField] ClientPlayerData _thisPlayer;
    Subscription<TileSelectedEvent> _tileSelectedSub;

    void Start()
    {
        _tileSelectedSub = EventBus.Subscribe<TileSelectedEvent>(SelectHexOnTurn);    
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe(_tileSelectedSub);
    }

    // If it's this player's turn, end their turn with the selected hex
    // given in tileSelectedEvent
    void SelectHexOnTurn(TileSelectedEvent tileSelectedEvent)
    {
        if (!_thisPlayer.MyTurn)
            return;

        TurnEndData turnEndData = new TurnEndData(_thisPlayer.PlayerID,
            tileSelectedEvent.Coordinate);
        _thisPlayer.EndTurn(turnEndData);
    }
}
