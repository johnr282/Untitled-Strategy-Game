using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Any object that is spawned onto the map needs this component
// ------------------------------------------------------------------

// A spawn configuration is a set of spawn locations; the caller of 
// SpawnObject chooses which configuration to use based on how many
// objects are in the tile
[Serializable] public struct SpawnConfiguration
{
    public List<Vector3> OffsetsFromTileCenter { get => _offsetsFromTileCenter; }
    [SerializeField] List<Vector3> _offsetsFromTileCenter;

    public SpawnConfiguration(List<Vector3> offsetsFromTileCenter)
    {
        _offsetsFromTileCenter = offsetsFromTileCenter;
    }
}

public class SpawnableObject : MonoBehaviour
{
    [SerializeField] List<SpawnConfiguration> _spawnConfigurations;
    public List<SpawnConfiguration> SpawnConfigurations { get => _spawnConfigurations; }
    
    // Index into SpawnConfigurations
    public int CurrentConfiguration { get; private set; } = -1;

    // Index into the current configuration's OffsetsFromTileCenter list
    public int CurrentOffset { get; private set; } = -1;

    // Spawns a new GameObject of the given prefab onto the tilemap at the given hex
    public static TObject SpawnObject<TObject>(GameObject prefab,
        HexCoordinateOffset hex,
        Tilemap tilemap,
        Vector3 offsetFromTileCenter)
    {
        Vector3 tileWorldPosition = GetTileWorldPosition(hex, tilemap);
        Vector3 spawnLocation = tileWorldPosition + offsetFromTileCenter;
        GameObject newGameObject = Instantiate(prefab,
            spawnLocation,
            Quaternion.identity);

        TObject newUniqueObject = newGameObject.GetComponent<TObject>() ??
            throw new RuntimeException(
                $"Failed to get {typeof(TObject)} component from prefab");

        return newUniqueObject;
    }

    // Moves the given object to the given hex with given offset
    public static void MoveObject(GameObject obj,
        HexCoordinateOffset hex,
        Tilemap tilemap,
        Vector3 offsetFromTileCenter)
    {
        Vector3 tileWorldPosition = GetTileWorldPosition(hex, tilemap);
        Vector3 newLocation = tileWorldPosition + offsetFromTileCenter;
        obj.transform.position = newLocation;
    }

    public static Vector3 GetTileWorldPosition(HexCoordinateOffset hex,
        Tilemap tilemap)
    {
        return tilemap.CellToWorld(hex.ConvertToVector3Int());
    }
}
