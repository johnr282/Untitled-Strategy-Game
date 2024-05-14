using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ------------------------------------------------------------------
// Component for displaying coordinate of currently selected/highlighted
// tile
// ------------------------------------------------------------------

[RequireComponent(typeof(TextMeshProUGUI))]
public class DisplayCoordinate : MonoBehaviour
{
    TextMeshProUGUI _coordinateDisplay;
    Subscription<NewTileHoveredOverEvent> _tileHighlightedSub;

    void Awake()
    {
        _coordinateDisplay = GetComponent<TextMeshProUGUI>();
        _tileHighlightedSub = EventBus.Subscribe<NewTileHoveredOverEvent>(ChangeDisplayCoordinate);    
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe(_tileHighlightedSub);    
    }

    void ChangeDisplayCoordinate(NewTileHoveredOverEvent tileHighlightedEvent)
    {
        _coordinateDisplay.text = tileHighlightedEvent.Coordinate.ToString();
    }
}