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
    [Networked] public int UnitID { get; set; } = -1;

    UnitManager _unitManager;
    MoveableObject _moveable;

    Subscription<TileSelectedEvent> _tileSelectedSub;
    Subscription<NewTileHoveredOverEvent> _tileHoveredSub;
    Subscription<MoveUnitRequest> _moveUnitRequestSub;

    public override void Start()
    {
        base.Start();

        _unitManager = ProjectUtilities.FindUnitManager();
        _moveable = GetComponent<MoveableObject>();

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
        Debug.Log("Unit " + UnitID.ToString() + " selected");

        _tileSelectedSub =
            EventBus.Subscribe<TileSelectedEvent>(OnTileSelected);
        _tileHoveredSub =
            EventBus.Subscribe<NewTileHoveredOverEvent>(OnTileHovered);
    }

    void OnTileSelected(TileSelectedEvent tileSelectedEvent)
    {
        _selectedByOwner = false;
        EventBus.Unsubscribe(_tileSelectedSub);
        EventBus.Unsubscribe(_tileHoveredSub);

        MoveUnitRequest request = new(UnitID,
            tileSelectedEvent.Coordinate,
            _playerData.PlayerID);

        var rpcAction = new Action<NetworkRunner, PlayerRef, MoveUnitRequest>(
            ClientMessages.RPC_MoveUnit);
        EventBus.Publish(new NetworkInputEvent(request,
            rpcAction,
            Runner));
    }

    void OnTileHovered(NewTileHoveredOverEvent tileHoveredEvent)
    {

    }

    // Called on the server when it receives a MoveUnitRequest
    // If request is valid, moves this UnitObject to the correct location on the
    // map and moves the requested Unit to its new location
    void OnMoveUnitRequest(MoveUnitRequest request)
    {
        if (request.UnitID != UnitID)
            return;

        Debug.Log("Handling move unit request from player " + 
            request.RequestingPlayerID);

        HexCoordinateOffset requestedHex = 
            HexUtilities.ConvertToHexCoordinateOffset(request.Location);

        if (_unitManager.TryMoveUnit(request.UnitID,
            requestedHex,
            out List<HexCoordinateOffset> pathTaken))
        {
            _moveable.MoveTo(request.Location);
            Debug.Log("Request completed");
        }
        else
        {
            Debug.Log("Request denied");
        }
    }
}