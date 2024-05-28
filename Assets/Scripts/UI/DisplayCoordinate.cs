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

    void Awake()
    {
        _coordinateDisplay = GetComponent<TextMeshProUGUI>();
        EventBus.Subscribe<NewTileHoveredOverEvent>(ChangeDisplayCoordinate);    
    }

    void ChangeDisplayCoordinate(NewTileHoveredOverEvent tileHighlightedEvent)
    {
        _coordinateDisplay.text = 
            tileHighlightedEvent.Coordinate.ConvertToVector3Int().ToString();
    }
}