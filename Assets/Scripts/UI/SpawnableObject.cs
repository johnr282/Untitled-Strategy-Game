using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Any object that is spawned onto the map needs this component
// ------------------------------------------------------------------

public class SpawnableObject : MonoBehaviour
{
    [SerializeField] float _heightAboveTilemap;
    public Vector3 YOffset { get => Vector3.up * _heightAboveTilemap; }

    // Spawns a new GameObject of the given prefab onto the tilemap at the given hex
    public static TObject SpawnObject<TObject>(GameObject prefab,
        HexCoordinateOffset hex,
        Tilemap tilemap,
        Vector3 spawnLocationModifier)
    {
        Vector3Int tilemapLocation = hex.ConvertToVector3Int();
        Vector3 spawnLocation = tilemap.CellToWorld(tilemapLocation) + spawnLocationModifier;
        GameObject newGameObject = Instantiate(prefab,
            spawnLocation,
            Quaternion.identity);

        TObject newUniqueObject = newGameObject.GetComponent<TObject>() ??
            throw new RuntimeException(
                $"Failed to get {typeof(TObject)} component from prefab");

        return newUniqueObject;
    }
}
