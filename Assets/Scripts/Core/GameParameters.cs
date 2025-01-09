using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component for storing and accessing game parameters
// ------------------------------------------------------------------

public class GameParameters : MonoBehaviour
{
    public int TileUnitCapacity { get => _TileUnitCapacity; }
    [SerializeField] int _TileUnitCapacity; // Real value will be 12

    public int TileStructureCapacity { get => _tileStructureCapacity; }
    [SerializeField] int _tileStructureCapacity; // Real value will be 3
}
