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

    void Start()
    {
        _turnInfoDisplay = GetComponent<TextMeshProUGUI>();
        _turnInfoDisplay.text = "";

        EventBus.Subscribe<MyTurnEvent>(OnMyTurn);
        EventBus.Subscribe<EndTurnAction>(OnEndTurn);
    }

    void OnMyTurn(MyTurnEvent myTurn)
    {
        SetTurnInfoText("It's your turn, select and move your unit!");
    }

    void OnEndTurn(EndTurnAction endTurn)
    {
        SetTurnInfoText("Waiting for other players...");
    }

    void SetTurnInfoText(string textToDisplay)
    {
        _turnInfoDisplay.text = textToDisplay;
    }
}