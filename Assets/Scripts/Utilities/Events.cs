using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains Event definitions used with EventBus system
// ------------------------------------------------------------------

// Published when a new tile is highlighted
public class NewTileHighlightedEvent
{
    public Vector3Int Coordinate { get; }

    public NewTileHighlightedEvent(Vector3Int coordinateIn)
    {
        Coordinate = coordinateIn;
    }
}

// Published when a tile is selected
public class TileSelectedEvent
{ 
    public Vector3Int Coordinate { get; }

    public TileSelectedEvent(Vector3Int coordinateIn)
    {
        Coordinate = coordinateIn;
    }
}

// Published to a client when they should generate the map with the given
// seed
public class GenerateMapEvent
{
    public int MapSeed { get; }

    public GenerateMapEvent(int mapSeedIn)
    {
        MapSeed = mapSeedIn;
    }
}

// Published to a client when it's their turn
public class PlayerTurnEvent { }

// Published to a client when they receive their player ID from the server
public class PlayerIDReceivedEvent
{ 
    public int PlayerID { get; }

    public PlayerIDReceivedEvent(int playerIDIn)
    {
        PlayerID = playerIDIn;
    }
}

// Published to the server when a client finishes their turn
public class TurnFinishedEvent
{
    public int PlayerID { get; }
    
    public TurnFinishedEvent(int playerIDIn)
    {
        PlayerID = playerIDIn;
    }
}
