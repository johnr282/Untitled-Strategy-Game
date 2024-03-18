using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ------------------------------------------------------------------
// Component for displaying coordinate of currently selected/highlighted
// tile
// ------------------------------------------------------------------

public class DisplayCoordinate : MonoBehaviour
{
    TextMeshProUGUI _coordinateDisplay;
    Subscription<NewTileHighlightedEvent> _tileHighlightedSub;

    void Awake()
    {
        _coordinateDisplay = GetComponent<TextMeshProUGUI>();
        _tileHighlightedSub = EventBus.Subscribe<NewTileHighlightedEvent>(ChangeDisplayCoordinate);    
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe(_tileHighlightedSub);    
    }

    void ChangeDisplayCoordinate(NewTileHighlightedEvent tileHighlightedEvent)
    {
        _coordinateDisplay.text = tileHighlightedEvent.Coordinate.ToString();
    }
}