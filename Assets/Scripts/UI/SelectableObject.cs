using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Any object with a trigger collider and a component derived from
// SelectableObject can be selected by the player
// ------------------------------------------------------------------

[RequireComponent(typeof(Collider))]
public class SelectableObject : NetworkBehaviour
{
    public virtual void OnSelect()
    {
        Debug.Log(this.gameObject.name + " selected");
    }
}
