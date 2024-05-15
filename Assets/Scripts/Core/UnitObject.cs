using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component representing an interactable, player-controllable unit
// ------------------------------------------------------------------

[RequireComponent(typeof(MoveableObject))]
public class UnitObject : SelectableObject
{
    public Unit UnitRef { get; set; }

    UnitManager _unitManager;
    MoveableObject _moveable;

    Subscription<TileSelectedEvent> _tileSelectedSubscription;

    void Start()
    {
        _unitManager = ProjectUtilities.FindUnitManager();
        _moveable = GetComponent<MoveableObject>();
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe(_tileSelectedSubscription);    
    }

    public override void OnSelect()
    {
        Debug.Log("Unit selected");
        _tileSelectedSubscription =
            EventBus.Subscribe<TileSelectedEvent>(OnTileSelected);
    }

    void OnTileSelected(TileSelectedEvent tileSelectedEvent)
    {
        HexCoordinateOffset requestedHex = 
            HexUtilities.ConvertToHexCoordinateOffset(tileSelectedEvent.Coordinate);

        if (_unitManager.TryMoveUnit(UnitRef, 
            requestedHex,
            out List<HexCoordinateOffset> pathTaken))
        {
            EventBus.Unsubscribe(_tileSelectedSubscription);
            _moveable.MoveTo(tileSelectedEvent.Coordinate);
        }
        else
        {
            Debug.Log("Can't move unit there, select a different tile");
        }
    }
}