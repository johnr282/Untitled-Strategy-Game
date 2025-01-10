using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component handling spawning new StructureObjects onto the map
// ------------------------------------------------------------------

public class StructureObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject _structurePrefab;

    Tilemap _tilemap;

    void Start()
    {
        _tilemap = ObjectFinder.FindTilemap();
    }

    // Spawns a StructureObject onto the tilemap at the given hex
    // Throws a RuntimeException if the unit prefab is missing necessary components
    public StructureObject SpawnStructureObject(StructureID structureID,
        PlayerID ownerID,
        HexCoordinateOffset hex)
    {
        SpawnableObject spawnable = _structurePrefab.GetComponent<SpawnableObject>() ??
            throw new RuntimeException(
                "Failed to get SpawnableObject component from structure prefab");

        StructureObject newStructureObject = SpawnableObject.SpawnObject<StructureObject>(_structurePrefab,
            hex,
            _tilemap,
            Vector3.zero); // TODO

        newStructureObject.StructureID = structureID;
        newStructureObject.OwnerID = ownerID;
        return newStructureObject;
    }
}
