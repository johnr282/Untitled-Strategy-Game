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

    // If it's this player's turn, allow them to select the hex given
    // in tileSelectedEvent
    void SelectHexOnTurn(TileSelectedEvent tileSelectedEvent)
    {
        if (!_thisPlayer.MyTurn)
            return;

        Vector3Int selectedHex = tileSelectedEvent.Coordinate;
        EventBus.Publish(new HexSelectedEvent(selectedHex));
        _thisPlayer.EndTurn(selectedHex);
    }
}
