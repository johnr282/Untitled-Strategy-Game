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
    [SerializeField] float _unitStackYDistance; // Y distance between units when they are stacked

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

        float stackYOffset = GameMap.GetTile(hex).GetNumUnits() * _unitStackYDistance;
        Vector3 spawnLocationModifier = spawnable.YOffset + Vector3.up * stackYOffset;
        UnitObject newUnitObject = SpawnableObject.SpawnObject<UnitObject>(_unitPrefab,
            hex,
            _tilemap,
            spawnLocationModifier);

        newUnitObject.UnitID = unitID;
        newUnitObject.OwnerID = ownerID;
        return newUnitObject;
    }
}