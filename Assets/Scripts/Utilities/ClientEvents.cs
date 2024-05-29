using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains event definitions used with EventBus system that are
// published on a client
// ------------------------------------------------------------------   

// Published when a new tile is hovered over
public class NewTileHoveredOverEvent
{
    public HexCoordinateOffset Coordinate { get; }

    public NewTileHoveredOverEvent(HexCoordinateOffset coordinateIn)
    {
        Coordinate = coordinateIn;
    }
}

// Published when a tile is selected
public class TileSelectedEvent
{
    public HexCoordinateOffset Coordinate { get; }

    public TileSelectedEvent(HexCoordinateOffset coordinateIn)
    {
        Coordinate = coordinateIn;
    }
}

// Published when it's this client's turn
public class MyTurnEvent
{
}