using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Base class that all other units derive from
// ------------------------------------------------------------------

public class Unit : MonoBehaviour
{
    [SerializeField] int _strength;
    [SerializeField] int _size;
    [SerializeField] UnitType _type;
}
