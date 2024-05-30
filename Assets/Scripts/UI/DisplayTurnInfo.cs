using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component that displays turn info to the player
// ------------------------------------------------------------------

[RequireComponent(typeof(TextMeshProUGUI))]
public class DisplayTurnInfo : MonoBehaviour
{
    TextMeshProUGUI _turnInfoDisplay;

    Subscription<NextTurnUpdate> _nextTurnSub;

    void Start()
    {
        _turnInfoDisplay = GetComponent<TextMeshProUGUI>();
        _turnInfoDisplay.text = "";

        EventBus.Subscribe<MyTurnEvent>(OnMyTurn);
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
        _turnInfoDisplay.text = textToDisplay;
    }
}