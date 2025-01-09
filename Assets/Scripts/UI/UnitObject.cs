using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// ------------------------------------------------------------------
// Component representing an interactable, player-controllable unit
// ------------------------------------------------------------------

[RequireComponent(typeof(MoveableObject))]
public class UnitObject : SelectableObject
{
    public UnitID UnitID
    {
        get
        {
            if (_unitID.ID == -1)
                throw new RuntimeException("OwnerID not set");
            return _unitID;
        }
        set
        {
            _unitID = value;
        }
    }

    public Unit Unit { get => UnitManager.GetUnit(UnitID); }

    [SerializeField] List<Color> _unitColors = new();

    MoveableObject _moveable;
    MapVisuals _mapVisuals;
    Renderer _renderer;

    Subscription<TileSelectedEvent> _tileSelectedSub;
    Subscription<NewTileHoveredOverEvent> _tileHoveredSub;

    UnitID _unitID = new(-1);

    protected override void Start()
    {
        base.Start();

        _moveable = GetComponent<MoveableObject>();
        _mapVisuals = ProjectUtilities.FindMapVisuals();
        SetColor(); 
    }

    // Initializes _renderer and sets the color of this UnitObject based on its
    // OwnerID
    void SetColor()
    {
        GameObject visualsChild =
            UnityUtilities.GetFirstChildGameObject(gameObject);

        _renderer = visualsChild.GetComponent<Renderer>() ??
            throw new RuntimeException(
                "Failed to get Renderer component from child");

        if (OwnerID.ID >= _unitColors.Count)
            throw new RuntimeException("More players than unit colors");

        Color unitColor = _unitColors[OwnerID.ID];
        _renderer.material.color = unitColor;
    }

    protected override void OnSelectedByOwner()
    {
        Debug.Log("Unit " + UnitID.ToString() + " selected");

        // Select unit's tile as well to visually show that the unit is selected
        _mapVisuals.SelectTile(Unit.CurrentLocation.Hex);

        _tileSelectedSub =
            EventBus.Subscribe<TileSelectedEvent>(OnTileSelected);
        _tileHoveredSub =
            EventBus.Subscribe<NewTileHoveredOverEvent>(OnTileHovered);
    }

    // Moves this UnitObject to the given hex using its MoveableObject component
    public void MoveTo(HexCoordinateOffset hex)
    {
        List<HexCoordinateOffset> path = GameMap.FindPath(Unit, 
            GameMap.GetTile(hex));

        _moveable.MoveAlongPath(path, OnPathComplete);
    }

    void OnTileSelected(TileSelectedEvent tileSelectedEvent)
    {
        _selectedByOwner = false;
        EventBus.Unsubscribe(_tileSelectedSub);
        EventBus.Unsubscribe(_tileHoveredSub);

        HexCoordinateOffset requestedHex = tileSelectedEvent.Coordinate;
        //_mapVisuals.UnHighlightCurrentPath();

        MoveUnitUpdate update = new(UnitID,
            requestedHex,
            SelectingPlayerID);
        StateManager.RequestStateUpdate(update);
    }

    void OnTileHovered(NewTileHoveredOverEvent tileHoveredEvent)
    {
        GameTile goalTile = GameMap.GetTile(tileHoveredEvent.Coordinate);

        List<HexCoordinateOffset> path;
        try
        {
            path = GameMap.FindPath(Unit, goalTile);
        }
        catch (RuntimeException)
        {
            _mapVisuals.UnHighlightCurrentPath();
            return;
        }

        _mapVisuals.HighlightPath(path);
    }

    void OnPathComplete()
    {
        Debug.Log("Unit " + UnitID + " motion complete");
        _mapVisuals.UnHighlightCurrentPath();
        _mapVisuals.UnSelectCurrentlySelectedTile();
    }
}