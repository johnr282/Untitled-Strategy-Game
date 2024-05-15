using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Any object that is spawned onto the map needs this component
// ------------------------------------------------------------------

public class SpawnableObject : MonoBehaviour
{
    [SerializeField] float _heightAboveTilemap;
    public Vector3 YOffset { get => Vector3.up * _heightAboveTilemap; }
}
