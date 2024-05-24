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

// Published when this client should generate the map with the given seed
public class GenerateMapEvent
{
    public int MapSeed { get; }

    public GenerateMapEvent(int mapSeedIn)
    {
        MapSeed = mapSeedIn;
    }
}

// Published when this client receives their starting location from the server
public class StartingLocationReceivedEvent
{
    public Vector3Int Location { get; }

    public StartingLocationReceivedEvent(Vector3Int locationIn)
    {
        Location = locationIn;
    }
}