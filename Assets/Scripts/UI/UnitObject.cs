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

    UnitManager _unitManager;
    GameMap _gameMap;
    MoveableObject _moveable;
    MapVisuals _mapVisuals;

    Subscription<TileSelectedEvent> _tileSelectedSub;
    Subscription<NewTileHoveredOverEvent> _tileHoveredSub;
    Subscription<MoveUnitRequest> _moveUnitRequestSub;

    public override void Start()
    {
        base.Start();

        _unitManager = ProjectUtilities.FindUnitManager();
        _gameMap = ProjectUtilities.FindGameMap();
        _moveable = GetComponent<MoveableObject>();
        _mapVisuals = 
            ProjectUtilities.FindComponent<MapVisuals>(ProjectUtilities.MapObjectName);

        _moveUnitRequestSub = 
            EventBus.Subscribe<MoveUnitRequest>(OnMoveUnitRequest);
    }

    public override void FixedUpdateNetwork()
    {
        if (Input.GetKeyDown(KeyCode.Space) && HasStateAuthority)
        {
            transform.position += Vector3.right;
        }
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

    void OnTileSelected(TileSelectedEvent tileSelectedEvent)
    {
        _selectedByOwner = false;
        _mapVisuals.UnHighlightCurrentPath();
        EventBus.Unsubscribe(_tileSelectedSub);
        EventBus.Unsubscribe(_tileHoveredSub);

        HexCoordinateOffset requestedHex = tileSelectedEvent.Coordinate;

        MoveUnitRequest request = new(UnitID,
            tileSelectedEvent.Coordinate,
            SelectingPlayerID);
        NetworkInputManager.QueueNetworkInputEvent(request,
            ClientMessages.RPC_MoveUnit);
    }

    void OnTileHovered(NewTileHoveredOverEvent tileHoveredEvent)
    {
        Unit thisUnit = _unitManager.GetUnit(UnitID);
        GameTile goalTile = _gameMap.GetTile(tileHoveredEvent.Coordinate);

        List<HexCoordinateOffset> path;
        try
        {
            path = _gameMap.FindPath(thisUnit,
                goalTile);
        }
        catch (RuntimeException)
        {
            return;
        }

        _mapVisuals.HighlightPath(path);
    }

    // Called on the server when it receives a MoveUnitRequest
    // If request is valid, moves this UnitObject to the correct location on the
    // map and moves the requested Unit to its new location
    void OnMoveUnitRequest(MoveUnitRequest request)
    {
        if (request.UnitID.ID != UnitID.ID)
            return;

        Debug.Log("Handling move unit request from player " + 
            request.RequestingPlayerID);

        if (_unitManager.ValidateMoveUnitRequest(request))
        {
            Debug.Log("Request completed");
            _moveable.MoveTo(request.Location);

            UnitMoved unitMoved = new(request.UnitID,
                request.Location);
            GameStateManager.UpdateGameState(Runner,
                unitMoved,
                ServerMessages.RPC_UnitMoved);
        }
        else
        {
            Debug.Log("Request denied");
        }
    }
}