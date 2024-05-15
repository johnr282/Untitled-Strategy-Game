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

    void CreateUnitObject(UnitType unitType, 
        Vector3Int initialTile)
    {
        Vector3 spawnLocation = _tilemap.CellToWorld(initialTile) +
            0.5f * Vector3.up;
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
