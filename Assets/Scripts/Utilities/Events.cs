using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains Event definitions used with EventBus system
// ------------------------------------------------------------------

public class NewTileHighlightedEvent
{
    public Vector3Int Coordinate { get; }

    public NewTileHighlightedEvent(Vector3Int coordinateIn)
    {
        Coordinate = coordinateIn;
    }
}

public class TileSelectedEvent
{ 
    public Vector3Int Coordinate { get; }

    public TileSelectedEvent(Vector3Int coordinateIn)
    {
        Coordinate = coordinateIn;
    }
}

public class GenerateMapEvent
{
    public int MapSeed { get; }

    public GenerateMapEvent(int mapSeedIn)
    {
        MapSeed = mapSeedIn;
    }
}