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
    TextMeshProUGUI tmp;
    Subscription<NewTileHighlightedEvent> _tileSelectedSub;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        _tileSelectedSub = EventBus.Subscribe<NewTileHighlightedEvent>(ChangeDisplayCoordinate);    
    }

    void ChangeDisplayCoordinate(NewTileHighlightedEvent tileSelectedEvent)
    {
        tmp.text = tileSelectedEvent.Coordinate.ToString();
    }
}