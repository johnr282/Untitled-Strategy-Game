using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component for storing and accessing game parameters
// ------------------------------------------------------------------

public class GameParameters : MonoBehaviour
{
    public int TileUnitCapacity { get => _TileUnitCapacity; }
    [SerializeField] int _TileUnitCapacity;

    public int TileStructureCapacity { get => _tileStructureCapacity; }
    [SerializeField] int _tileStructureCapacity;
}
