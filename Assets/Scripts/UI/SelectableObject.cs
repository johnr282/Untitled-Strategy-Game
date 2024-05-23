using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Any object with a trigger collider and a component derived from
// SelectableObject can be selected by the owning player
// ------------------------------------------------------------------

[RequireComponent(typeof(Collider))]
public abstract class SelectableObject : NetworkBehaviour
{
    [Networked] public PlayerID OwnerID { get; set; }

    protected ClientPlayerData _playerData;
    protected bool _selectedByOwner = false;

    public virtual void Start()
    {
        _playerData = ProjectUtilities.FindClientPlayerData();
    }

    // Called when this object is selected by any player
    public void OnSelect()
    {
        // Prevents same object being selected multiple times before selection
        // operation is complete, whatever that may be
        // Responsibility of the derived class to set _selectedByOwner to false
        // when selection operation is finished
        if (_selectedByOwner)
        {
            Debug.Log("Object already selected");
            return;
        }

        Debug.Log(this.gameObject.name + " selected");

        PlayerID selectingPlayerID = _playerData.PlayerID;
        if (OwnerID.ID == selectingPlayerID.ID)
        {
            _selectedByOwner = true;
            OnSelectedByOwner();
        }
    }

    // Called when this object is selected by its owner
    public abstract void OnSelectedByOwner();
}
