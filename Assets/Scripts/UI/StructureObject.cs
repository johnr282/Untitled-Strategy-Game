using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureObject : SelectableObject
{
    public StructureID StructureID
    {
        get
        {
            if (_structureID.ID == -1)
                throw new RuntimeException("StructureID not set");
            return _structureID;
        }
        set
        {
            _structureID = value;
        }
    }

    StructureID _structureID = new(-1);

    protected override void Start()
    {
        base.Start();
        PlayerColors.SetObjectColor(gameObject, OwnerID);
    }

    protected override void OnSelectedByOwner()
    {
        Debug.Log("Structure " + StructureID + " selected");
    }
}
