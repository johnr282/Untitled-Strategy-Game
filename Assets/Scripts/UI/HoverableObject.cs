using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Any object with a trigger collider and a component derived from
// HoverableObject can be hovered over by the player
// ------------------------------------------------------------------

[RequireComponent(typeof(Collider))]
public class HoverableObject : MonoBehaviour
{
    public virtual void OnHover()
    {
        Debug.Log(this.gameObject.name + " hovered over");
    }
}
