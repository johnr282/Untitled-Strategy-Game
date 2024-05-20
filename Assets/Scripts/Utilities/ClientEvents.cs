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

// Published when this client receives their player ID from the server
public class PlayerIDReceivedEvent
{
    public int PlayerID { get; }

    public PlayerIDReceivedEvent(int playerIDIn)
    {
        PlayerID = playerIDIn;
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

// Published when it's this client's turn
public class PlayerTurnEvent
{
    public TurnStartData TurnStartInfo { get; }

    public PlayerTurnEvent(TurnStartData turnStartInfoIn)
    {
        TurnStartInfo = turnStartInfoIn;
    }
}

// Published when this client's turn finishes
public class TurnFinishedEventClient
{
    public TurnEndData TurnEndInfo { get; }

    public TurnFinishedEventClient(TurnEndData turnEndInfoIn)
    {
        TurnEndInfo = turnEndInfoIn;
    }
}

// Published when the server responds to a create unit request
public class CreateUnitResponseEvent
{
    public CreateUnitRequest Request { get; }
    public bool Success { get; }
       
    public CreateUnitResponseEvent(CreateUnitRequest requestIn, 
        bool successIn)
    {
        Request = requestIn;
        Success = successIn;
    }
}