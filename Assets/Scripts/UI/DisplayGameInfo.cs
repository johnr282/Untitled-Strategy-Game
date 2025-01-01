using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component that displays game info to the player
// ------------------------------------------------------------------

public struct DisplayGameInfoEvent
{
    public string InfoToDisplay { get; }

    public DisplayGameInfoEvent(string infoToDisplay)
    {
        InfoToDisplay = infoToDisplay;
    }
}

[RequireComponent(typeof(TextMeshProUGUI))]
public class DisplayGameInfo : MonoBehaviour
{
    TextMeshProUGUI _gameInfoDisplay;

    Subscription<NextTurnUpdate> _nextTurnSub;

    void Start()
    {
        _gameInfoDisplay = GetComponent<TextMeshProUGUI>();
        _gameInfoDisplay.text = "Waiting to host or join a session...";

        EventBus.Subscribe<DisplayGameInfoEvent>(Display);
        EventBus.Subscribe<MyTurnEvent>(OnMyTurn);
        EventBus.Subscribe<StartGameUpdate>(OnGameStarted);
    }

    void Display(DisplayGameInfoEvent displayGameInfoEvent)
    {
        SetTurnInfoText(displayGameInfoEvent.InfoToDisplay);
    }

    void OnGameStarted(StartGameUpdate update)
    {
        if (!PlayerManager.MyTurn)
            SetTurnInfoText("Waiting for other players...");
    }

    void OnMyTurn(MyTurnEvent myTurn)
    {
        SetTurnInfoText("It's your turn, select and move your unit!");
        _nextTurnSub = EventBus.Subscribe<NextTurnUpdate>(OnNextTurn);
    }

    void OnNextTurn(NextTurnUpdate update)
    {
        SetTurnInfoText("Waiting for other players...");
        EventBus.Unsubscribe(_nextTurnSub);
    }

    void SetTurnInfoText(string textToDisplay)
    {
        _gameInfoDisplay.text = textToDisplay;
    }
}