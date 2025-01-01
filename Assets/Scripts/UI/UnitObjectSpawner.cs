using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component handling spawning new UnitObjects onto the map
// ------------------------------------------------------------------

public class UnitObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject _unitPrefab;
    [SerializeField] UnitType _unitType;

    Tilemap _tilemap;

    // Start is called before the first frame update
    void Start()
    {
        _tilemap = ProjectUtilities.FindTilemap();
    }

    // Spawns a UnitObject onto the tilemap at the given hex with the given
    // unit ID; returns the spawned UnitObject
    // Throws a RuntimeException if the unit prefab is missing necessary components
    public UnitObject SpawnUnitObject(UnitID unitID, 
        PlayerID ownerID,
        HexCoordinateOffset hex)
    {
        SpawnableObject spawnable = _unitPrefab.GetComponent<SpawnableObject>() ??
            throw new RuntimeException(
                "Failed to get SpawnableObject component from unit prefab");

        Vector3Int tilemapLocation = hex.ConvertToVector3Int();

        Vector3 spawnLocation = _tilemap.CellToWorld(tilemapLocation) + 
            spawnable.YOffset;
        GameObject newUnitGameObject = Instantiate(_unitPrefab,
            spawnLocation,
            Quaternion.identity);

        UnitObject newUnitObject = newUnitGameObject.GetComponent<UnitObject>() ??
            throw new RuntimeException(
                "Failed to get UnitObject component from unit prefab");

        newUnitObject.UnitID = unitID;
        newUnitObject.OwnerID = ownerID;
        return newUnitObject;
    }
}