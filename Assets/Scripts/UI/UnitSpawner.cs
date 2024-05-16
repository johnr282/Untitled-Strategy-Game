using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component used by clients to spawn UnitObjects into the map
// ------------------------------------------------------------------

public class UnitSpawner : NetworkBehaviour
{
    [SerializeField] GameObject _unitPrefab;
    [SerializeField] UnitType _unitType;

    Tilemap _tilemap;
    ClientPlayerData _playerData;

    Subscription<StartingLocationReceivedEvent> _startingLocationSub;
    Subscription<CreateUnitResponseEvent> _createUnitResponseSub;

    // Start is called before the first frame update
    void Start()
    {
        _tilemap = ProjectUtilities.FindTilemap();
        _playerData = ProjectUtilities.FindClientPlayerData();

        _createUnitResponseSub =
            EventBus.Subscribe<CreateUnitResponseEvent>(OnCreateUnitResponse);
        _startingLocationSub =
            EventBus.Subscribe<StartingLocationReceivedEvent>(OnStartingLocationReceived);
    }

    void OnStartingLocationReceived(StartingLocationReceivedEvent locationEvent)
    {
        Debug.Log("Requesting to spawn unit at " + locationEvent.Location.ToString());
        RequestSpawnUnit(_unitType, locationEvent.Location);
    }

    // Requests the server to create a new unit of the given type at the given tile
    public void RequestSpawnUnit(UnitType unitType,
        Vector3Int initialTile)
    {
        ClientMessages.RPC_CreateUnit(Runner,
            PlayerRef.None,
            new CreateUnitRequest(unitType, 
                initialTile,
                _playerData.PlayerID));
    }

    // Spawns a UnitObject if the request succeeded
    void OnCreateUnitResponse(CreateUnitResponseEvent createUnitResponseEvent)
    {
        if (!createUnitResponseEvent.Success)
        {
            Debug.Log("Unit could not be created");
            return;
        }

        SpawnUnitObject(createUnitResponseEvent.Request.Type,
            createUnitResponseEvent.Request.Location);
    }

    // Spawns a UnitObject onto the tilemap at the given tile
    // Throws a RuntimeException if the unit prefab is missing necessary components
    void SpawnUnitObject(UnitType unitType, 
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
    }
}
