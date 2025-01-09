using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class StructureManager : SimulationBehaviour
{
    // Will eventually be loaded in from data files
    static Dictionary<StructureType, StructureAttributes> _structureTypeAttributes = new()
    {
        { StructureType.Capital, new StructureAttributes(2) }
    };

    static Dictionary<StructureID, Structure> _structures = new();

    static StructureObjectSpawner _structureObjectSpawner = null;

    static int _nextStructureID = 0;

    void Start()
    {
        _structureObjectSpawner = ProjectUtilities.FindStructureObjectSpawner();

        StateManager.RegisterStateUpdate<CreateStructureUpdate>(ValidateCreateStructureUpdate,
            CreateStructure,
            StateManagerRPCs.RPC_CreateStructureServer,
            StateManagerRPCs.RPC_CreateStructureClient);
    }

    // Returns the structure corresponding to the given unit ID
    // Throws an ArgumentException if given ID has no corresponding structure
    public static Structure GetStructure(StructureID structureID)
    {
        if (!_structures.TryGetValue(structureID, out Structure structure))
            throw new System.ArgumentException("No structure exists with the given structure ID");

        return structure;
    }

    // Returns the structure attributes corresponding to the given structure type
    // Throws an ArgumentException if given type has no corresponding attributes
    public static StructureAttributes GetStructureAttributes(StructureType type)
    {
        if (!_structureTypeAttributes.TryGetValue(type, out StructureAttributes attributes))
            throw new System.ArgumentException("No attributes exist for the given structure type");

        return attributes;
    }

    // Creates a new structure according to the given update
    // Throws an ArgumentException if no GameTile exists at the update location
    static void CreateStructure(CreateStructureUpdate update)
    {
        GameTile tile = GameMap.GetTile(update.Location);

        // Hardcode attributes for now
        StructureAttributes attributes = new StructureAttributes(2);
        Structure newStructure = new Structure(update.Type,
            attributes,
            GetNextStructureID(),
            tile,
            update.RequestingPlayerID);
        _structures.Add(newStructure.StructureID, newStructure);

        Debug.Log("Created structure " + newStructure.StructureID);

        StructureObject newStructureObject = _structureObjectSpawner.SpawnStructureObject(newStructure.StructureID,
            update.RequestingPlayerID,
            update.Location);
        newStructure.StructureObject = newStructureObject;
        tile.AddStructure(newStructure);
    }

    static bool ValidateCreateStructureUpdate(CreateStructureUpdate update,
        out string failureReason)
    {
        GameTile tile = GameMap.GetTile(update.Location);

        if (!tile.IsOwnedBy(update.RequestingPlayerID))
        {
            failureReason = $"Player {update.RequestingPlayerID} does not own tile, tile is owned by {tile.OwnerID}";
            return false;
        }

        if (tile.ExceedsStructureCapacity(update.Type))
        {
            failureReason = "Structure would exceed tile structure capacity";
            return false;
        }

        failureReason = "";
        return true;
    }

    static StructureID GetNextStructureID()
    {
        if (_nextStructureID >= ushort.MaxValue)
            throw new RuntimeException("Ran out of structure IDs");

        StructureID nextID = new(_nextStructureID);
        _nextStructureID++;
        return nextID;
    }
}
