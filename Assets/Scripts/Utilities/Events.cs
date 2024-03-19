using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains Event definitions used with EventBus system
// ------------------------------------------------------------------

// Published on a client when a new tile is highlighted
public class NewTileHighlightedEvent
{
    public Vector3Int Coordinate { get; }

    public NewTileHighlightedEvent(Vector3Int coordinateIn)
    {
        Coordinate = coordinateIn;
    }
}

// Published on a client when a tile is selected
public class TileSelectedEvent
{ 
    public Vector3Int Coordinate { get; }

    public TileSelectedEvent(Vector3Int coordinateIn)
    {
        Coordinate = coordinateIn;
    }
}

// Published on a client when they should generate the map with the given
// seed
public class GenerateMapEvent
{
    public int MapSeed { get; }

    public GenerateMapEvent(int mapSeedIn)
    {
        MapSeed = mapSeedIn;
    }
}

// Published on a client when they receive their player ID from the server
public class PlayerIDReceivedEvent
{ 
    public int PlayerID { get; }

    public PlayerIDReceivedEvent(int playerIDIn)
    {
        PlayerID = playerIDIn;
    }
}

// Published on a client when it's their turn
public class PlayerTurnEvent
{
    public TurnStartData TurnStartInfo { get; }

    public PlayerTurnEvent(TurnStartData turnStartInfoIn)
    {
        TurnStartInfo = turnStartInfoIn;
    }
}

// Published on the server when a client finishes their turn
public class TurnFinishedEventServer
{
    public TurnEndData TurnEndInfo { get; }
    
    public TurnFinishedEventServer(TurnEndData turnEndInfoIn)
    {
        TurnEndInfo = turnEndInfoIn;
    }
}

// Published on a client when their turn finishes
public class TurnFinishedEventClient
{
    public TurnEndData TurnEndInfo { get; }

    public TurnFinishedEventClient(TurnEndData turnEndInfoIn)
    {
        TurnEndInfo = turnEndInfoIn;
    }
}