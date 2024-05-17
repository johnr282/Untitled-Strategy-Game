using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// ------------------------------------------------------------------
// Component representing an interactable, player-controllable unit
// ------------------------------------------------------------------

[RequireComponent(typeof(MoveableObject))]
public class UnitObject : SelectableObject
{
    [Networked] public int UnitID { get; set; }

    UnitManager _unitManager;
    MoveableObject _moveable;

    Subscription<TileSelectedEvent> _tileSelectedSubscription;

    void Start()
    {
        _unitManager = ProjectUtilities.FindUnitManager();
        _moveable = GetComponent<MoveableObject>();
    }

    public override void OnSelect()
    {
        Debug.Log("Unit " + UnitID.ToString() + " selected");
        _tileSelectedSubscription =
            EventBus.Subscribe<TileSelectedEvent>(OnTileSelected);
    }

    void OnTileSelected(TileSelectedEvent tileSelectedEvent)
    {
        HexCoordinateOffset requestedHex = 
            HexUtilities.ConvertToHexCoordinateOffset(tileSelectedEvent.Coordinate);

        if (_unitManager.TryMoveUnit(UnitID, 
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