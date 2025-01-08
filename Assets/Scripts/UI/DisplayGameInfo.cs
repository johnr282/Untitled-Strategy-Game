using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component that displays game info to the player
// ------------------------------------------------------------------

[RequireComponent(typeof(TextMeshProUGUI))]
public class DisplayGameInfo : MonoBehaviour
{
    TextMeshProUGUI _gameInfoDisplay;

    Subscription<EndTurnUpdate> _nextTurnSub;

    void Start()
    {
        _gameInfoDisplay = GetComponent<TextMeshProUGUI>();
        _gameInfoDisplay.text = "Waiting to host or join a session...";

        EventBus.Subscribe<MyTurnEvent>(OnMyTurn);
        EventBus.Subscribe<StartGameUpdate>(OnGameStarted);
    }

    void OnGameStarted(StartGameUpdate update)
    {
        if (!PlayerManager.MyTurn)
            SetTurnInfoText("Waiting for other players...");
    }

    void OnMyTurn(MyTurnEvent myTurn)
    {
        string onMyTurnMessage = "";
        if (GamePhaseManager.CurrentPhase == GamePhase.TerritorySelection)
        {
            onMyTurnMessage = "It's your turn; place 1 unit on an unclaimed tile adjacent to at " +
                "least one of your current tiles, or reinforce an already claimed tile";
        }

        SetTurnInfoText(onMyTurnMessage);
        _nextTurnSub = EventBus.Subscribe<EndTurnUpdate>(OnNextTurn);
    }

    void OnNextTurn(EndTurnUpdate update)
    {
        SetTurnInfoText("Waiting for other players...");
        EventBus.Unsubscribe(_nextTurnSub);
    }

    void SetTurnInfoText(string textToDisplay)
    {
        _gameInfoDisplay.text = textToDisplay;
    }
}