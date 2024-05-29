using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

// ------------------------------------------------------------------
// Component representing an interactable, player-controllable unit
// ------------------------------------------------------------------

[RequireComponent(typeof(MoveableObject))]
public class UnitObject : SelectableObject
{
    [Networked] public UnitID UnitID { get; set; } = new(-1);

    MoveableObject _moveable;
    MapVisuals _mapVisuals;

    Subscription<TileSelectedEvent> _tileSelectedSub;
    Subscription<NewTileHoveredOverEvent> _tileHoveredSub;

    public override void Start()
    {
        base.Start();

        _moveable = GetComponent<MoveableObject>();
        _mapVisuals = ProjectUtilities.FindMapVisuals();
    }

    public override void OnSelectedByOwner()
    {
        if (UnitID.ID == -1)
            throw new RuntimeException("Unit ID not set");

        Debug.Log("Unit " + UnitID.ToString() + " selected");

        _tileSelectedSub =
            EventBus.Subscribe<TileSelectedEvent>(OnTileSelected);
        _tileHoveredSub =
            EventBus.Subscribe<NewTileHoveredOverEvent>(OnTileHovered);
    }

    // Moves this UnitObject to the given hex using its MoveableObject component
    public void MoveTo(HexCoordinateOffset hex)
    {
        _moveable.MoveTo(hex);
    }

    void OnTileSelected(TileSelectedEvent tileSelectedEvent)
    {
        _selectedByOwner = false;
        _mapVisuals.UnHighlightCurrentPath();
        EventBus.Unsubscribe(_tileSelectedSub);
        EventBus.Unsubscribe(_tileHoveredSub);

        HexCoordinateOffset requestedHex = tileSelectedEvent.Coordinate;

        MoveUnitAction request = new(UnitID,
            tileSelectedEvent.Coordinate,
            SelectingPlayerID);
        ClientActionManager.QueueClientAction(request,
            ClientMessages.RPC_MoveUnit);
    }

    void OnTileHovered(NewTileHoveredOverEvent tileHoveredEvent)
    {
        Unit thisUnit = UnitManager.GetUnit(UnitID);
        GameTile goalTile = GameMap.GetTile(tileHoveredEvent.Coordinate);

        List<HexCoordinateOffset> path;
        try
        {
            path = GameMap.FindPath(thisUnit,
                goalTile);
        }
        catch (RuntimeException)
        {
            return;
        }

        _mapVisuals.HighlightPath(path);
    }
}