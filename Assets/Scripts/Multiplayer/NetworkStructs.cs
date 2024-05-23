using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains structs deriving from INetworkStruct used in RPC calls
// ------------------------------------------------------------------

// Stores data sent to a client when all players have joined and the game starts
public readonly struct GameStartData : INetworkStruct
{
    public PlayerID PlayerID { get; }
    public GameTile StartLocation { get; }

    public GameStartData(PlayerID playerIDIn, 
        GameTile startLocationIn)
    {
        PlayerID = playerIDIn;
        StartLocation = startLocationIn;
    }
}

// Stores data sent to a client when it becomes their turn
public readonly struct TurnStartData : INetworkStruct
{
    // True if this if the first turn of the game
    public NetworkBool FirstTurn { get; }
    public Vector3Int PreviousPlayerHex { get; }

    public TurnStartData(NetworkBool firstTurnIn,
        Vector3Int previousPlayerHexIn = new Vector3Int())
    {
        FirstTurn = firstTurnIn;
        PreviousPlayerHex = previousPlayerHexIn;
    }
}

// Stores data sent to server when a player finishes their turn
public readonly struct TurnEndData : INetworkStruct
{
    public int PlayerID { get; }
    public Vector3Int SelectedHex { get; }

    public TurnEndData(int playerIDIn,
        Vector3Int selectedHexIn)
    {
        PlayerID = playerIDIn;
        SelectedHex = selectedHexIn;
    }
}

//// Stores data sent to server when a client requests to create a new unit
//public readonly struct CreateUnitRequest : INetworkStruct
//{
//    public UnitType Type { get; }
//    public Vector3Int Location { get; }
//    public int RequestingPlayerID { get; }

//    public CreateUnitRequest(UnitType typeIn,
//        Vector3Int locationIn, 
//        int requestingPlayerIDIn)
//    {
//        Type = typeIn;
//        Location = locationIn;
//        RequestingPlayerID = requestingPlayerIDIn;
//    }
//}