using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component for testing unit spawning and movement
// ------------------------------------------------------------------

public class TestUnitMovement : MonoBehaviour
{
    [SerializeField] GameObject _unitPrefab;
    [SerializeField] UnitType _unitType;
    [SerializeField] Vector3Int _initialTile;

    UnitManager _unitManager;
    Tilemap _tilemap;

    void Start()
    {
        _unitManager = ProjectUtilities.FindUnitManager();
        _tilemap = ProjectUtilities.FindTilemap();

        CreateUnitObject(_unitType, _initialTile);
    }

    // Spawns a UnitObject onto the tilemap at the given tile and requests the 
    // unit manager to create a new unit of the given type at the hex corresponding
    // to the given tile
    // Throws a RuntimeException if the unit prefab is missing necessary components
    void CreateUnitObject(UnitType unitType, 
        Vector3Int initialTile)
    {
        SpawnableObject spawnable = _unitPrefab.GetComponent<SpawnableObject>() ??
            throw new RuntimeException(
                "Failed to get SpawnableObject component from unit prefab");

        Vector3 spawnLocation = _tilemap.CellToWorld(initialTile) + spawnable.YOffset;
        GameObject newUnitObject = Instantiate(_unitPrefab,
            spawnLocation,
            Quaternion.identity);

        UnitObject newUnit = newUnitObject.GetComponent<UnitObject>() ??
            throw new RuntimeException(
                "Failed to get UnitObject component from unit prefab");

        HexCoordinateOffset initialHex =
            HexUtilities.ConvertToHexCoordinateOffset(initialTile);
        newUnit.UnitRef = _unitManager.CreateUnit(unitType, initialHex);
    }
}
