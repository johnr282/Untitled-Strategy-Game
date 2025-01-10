using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        _tilemap = ObjectFinder.FindTilemap();
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

        GameTile tile = GameMap.GetTile(hex);
        Vector3 offsetFromTileCenter = CalculateNewUnitOffsetAndReconfigure(tile,
            _tilemap,
            spawnable,
            _unitStackYDistance);

        UnitObject newUnitObject = SpawnableObject.SpawnObject<UnitObject>(_unitPrefab,
            hex,
            _tilemap,
            offsetFromTileCenter);

        newUnitObject.UnitID = unitID;
        newUnitObject.OwnerID = ownerID;
        return newUnitObject;
    }

    // Calculates and returns the offset from tile center for a new unit added
    // to the given tile; if necessary, moves existing units in tile to new
    // configuration to accommodate new tile
    static Vector3 CalculateNewUnitOffsetAndReconfigure(GameTile tile,
        Tilemap tilemap,
        SpawnableObject spawnable,
        float unitStackYDistance)
    {
        int numUnitsOnTile = tile.GetNumUnits();

        // UnitObjects have 3 spawn configurations: 1 stack, 2 stacks, and 3 stacks
        // Check if we need to change configurations when spawning new unit
        int numUnitsOnTileAfterNewUnit = numUnitsOnTile + 1;
        int currConfigIndex = GetSpawnConfigurationIndex(numUnitsOnTile);
        int newConfigIndex = GetSpawnConfigurationIndex(numUnitsOnTileAfterNewUnit);
        bool needToChangeConfigurations = currConfigIndex != newConfigIndex;

        Vector3 offsetFromTileCenter;
        if (!needToChangeConfigurations)
        {
            // Spawn unit on last stack of current config
            int currNumUnitsInLastStack = numUnitsOnTile % GetMaxUnitsPerStack();
            float stackYOffset = currNumUnitsInLastStack * unitStackYDistance;

            SpawnConfiguration spawnConfiguration = spawnable.SpawnConfigurations[currConfigIndex];
            // 1 stack in 0th config, 2 stacks in 1st, 3 stacks in 2nd
            int numStacksInConfig = currConfigIndex + 1;
            int lastStackIndex = numStacksInConfig - 1;
            offsetFromTileCenter = spawnConfiguration.OffsetsFromTileCenter[lastStackIndex] +
                Vector3.up * stackYOffset;
        }
        else
        {
            // Need to move all units on tile to new configuration
            SpawnConfiguration newConfig = spawnable.SpawnConfigurations[newConfigIndex];
            int stackNum = 0;
            int unitsMovedToStack = 0;
            int maxUnitsPerStack = GetMaxUnitsPerStack();
            foreach (UnitID id in tile.UnitsOnTile)
            {
                if (unitsMovedToStack >= maxUnitsPerStack)
                {
                    stackNum++;
                    unitsMovedToStack = 0;
                }

                Vector3 stackYOffset = Vector3.up * (unitsMovedToStack * unitStackYDistance);
                Vector3 newOffset = newConfig.OffsetsFromTileCenter[stackNum];
                Unit unit = UnitManager.GetUnit(id);
                SpawnableObject.MoveObject(unit.UnitObject.gameObject,
                    tile.Hex,
                    tilemap,
                    newOffset + stackYOffset);
                unitsMovedToStack++;
            }
            // Old units will fill up first stacks, new unit will be alone in last stack
            offsetFromTileCenter = newConfig.OffsetsFromTileCenter[stackNum + 1];
        }

        return offsetFromTileCenter;
    }

    // Return the spawn configuration index for the given number of units
    static int GetSpawnConfigurationIndex(int numUnitsOnTile)
    {
        int tileUnitCapacity = ObjectFinder.FindGameParameters().TileUnitCapacity;
        if (numUnitsOnTile <= tileUnitCapacity / 3)
            return 0;
        else if (numUnitsOnTile <= (2 * tileUnitCapacity) / 3)
            return 1;
        else
            return 2;
    }

    static int GetMaxUnitsPerStack()
    {
        int tileUnitCapacity = ObjectFinder.FindGameParameters().TileUnitCapacity;
        return tileUnitCapacity / 3;
    }
}