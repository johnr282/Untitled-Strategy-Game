using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains definitions of structs implementing the IClientRequestData
// interface used in the StateManager system
// ------------------------------------------------------------------

public readonly struct EndTurnRequest : IClientRequestData
{
    public PlayerID EndingPlayerID { get; }

    public EndTurnRequest(PlayerID playerIDIn)
    {
        EndingPlayerID = playerIDIn;
    }
}

public readonly struct CreateUnitRequest : IClientRequestData 
{
    public UnitType Type { get; }
    public HexCoordinateOffset Location { get; }
    public PlayerID RequestingPlayerID { get; }

    public CreateUnitRequest(UnitType typeIn,
        HexCoordinateOffset locationIn,
        PlayerID requestingPlayerIDIn)
    {
        Type = typeIn;
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }
}

public readonly struct MoveUnitRequest : IClientRequestData 
{
    public UnitID UnitID { get; }
    public HexCoordinateOffset Location { get; }
    public PlayerID RequestingPlayerID { get; }

    public MoveUnitRequest(UnitID unitIDIn,
        HexCoordinateOffset locationIn,
        PlayerID requestingPlayerIDIn)
    {
        UnitID = unitIDIn;
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }
}