//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Tilemaps;

//// ------------------------------------------------------------------
//// Component that displays turn info to the player
//// ------------------------------------------------------------------

//[RequireComponent(typeof(TextMeshProUGUI))]
//public class DisplayTurnInfo : MonoBehaviour
//{
//    TextMeshProUGUI _turnInfoDisplay;
//    Subscription<TurnFinishedEventClient> _hexSelectedSub;
//    Subscription<PlayerTurnEvent> _playerTurnSub;

//    void Start()
//    {
//        _turnInfoDisplay = GetComponent<TextMeshProUGUI>();
//        _turnInfoDisplay.text = "";

//        _hexSelectedSub = EventBus.Subscribe<TurnFinishedEventClient>(DisplayTurnEndInfo);
//        _playerTurnSub = EventBus.Subscribe<PlayerTurnEvent>(DisplayTurnStartInfo);
//    }

//    void OnDestroy()
//    {
//        EventBus.Unsubscribe(_hexSelectedSub);    
//        EventBus.Unsubscribe(_playerTurnSub);
//    }

//    void DisplayTurnStartInfo(PlayerTurnEvent turnEvent)
//    {
//        string textToDisplay;
//        if (turnEvent.TurnStartInfo.FirstTurn)
//        {
//            textToDisplay = "It's your turn! Select a hex on the map.";
//        }
//        else
//        {
//            textToDisplay = "It's your turn! Select a hex on the map.\n" +
//                "Previous player selected " +
//                turnEvent.TurnStartInfo.PreviousPlayerHex.ToString();
//        }

//        SetTurnInfoText(textToDisplay);
//    }

//    void DisplayTurnEndInfo(TurnFinishedEventClient turnFinishedEvent)
//    {
//        SetTurnInfoText("You selected " + 
//            turnFinishedEvent.TurnEndInfo.SelectedHex.ToString() +
//            "\nWaiting for other players...");
//    }

//    void SetTurnInfoText(string textToDisplay)
//    {
//        _turnInfoDisplay.text = textToDisplay;
//    }
//}
