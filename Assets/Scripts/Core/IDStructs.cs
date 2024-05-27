using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains ID structs
// ------------------------------------------------------------------

public readonly struct PlayerID : INetworkStruct
{
    public int ID { get; }

    public PlayerID(int idIn)
    {
        ID = idIn;
    }

    public override string ToString() => ID.ToString();
}

public readonly struct ContinentID : INetworkStruct
{
    public readonly int ID { get; }

    public ContinentID(int idIn)
    {
        ID = idIn;
    }

    public override string ToString() => ID.ToString();
}

public readonly struct UnitID : INetworkStruct
{
    public readonly int ID { get; }

    public UnitID(int idIn)
    {
        ID = idIn;
    }

    public override string ToString() => ID.ToString();
}
