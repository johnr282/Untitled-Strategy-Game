using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ------------------------------------------------------------------
// Component that displays turn info to the player
// ------------------------------------------------------------------

public class DisplayTurnInfo : MonoBehaviour
{
    TextMeshProUGUI _turnInfoDisplay;
    Subscription<TurnFinishedEvent> _hexSelectedSub;
    Subscription<PlayerTurnEvent> _playerTurnSub;

    void Start()
    {
        _turnInfoDisplay = GetComponent<TextMeshProUGUI>();
        _hexSelectedSub = EventBus.Subscribe<TurnFinishedEvent>(DisplayTurnEndInfo);
        _playerTurnSub = EventBus.Subscribe<PlayerTurnEvent>(DisplayTurnStartInfo);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe(_hexSelectedSub);    
    }

    void DisplayTurnStartInfo(PlayerTurnEvent turnEvent)
    {
        SetTurnInfoText("It's your turn! Select a hex on the map.\n" +
            "Previous player selected " + 
            turnEvent.TurnStartInfo.PreviousPlayerHex.ToString());
    }

    void DisplayTurnEndInfo(TurnFinishedEvent turnFinishedEvent)
    {
        SetTurnInfoText("You selected " + 
            turnFinishedEvent.TurnEndInfo.SelectedHex.ToString() +
            "\nWaiting for other players...");
    }

    void SetTurnInfoText(string text)
    {
        _turnInfoDisplay.text = text;
    }
}
