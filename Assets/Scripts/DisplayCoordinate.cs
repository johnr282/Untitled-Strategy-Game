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
    Subscription<NewTileSelectedEvent> _tileSelectedSub;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        _tileSelectedSub = EventBus.Subscribe<NewTileSelectedEvent>(ChangeDisplayCoordinate);    
    }

    void ChangeDisplayCoordinate(NewTileSelectedEvent tileSelectedEvent)
    {
        tmp.text = tileSelectedEvent.coordinate.ToString();
    }
}