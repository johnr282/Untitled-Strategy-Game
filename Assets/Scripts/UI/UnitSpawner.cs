using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component handling spawning new UnitObjects onto the map
// ------------------------------------------------------------------

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(UnitManager))]
public class UnitSpawner : NetworkBehaviour
{
    [SerializeField] NetworkObject _unitPrefab;
    [SerializeField] UnitType _unitType;

    Tilemap _tilemap;
    UnitManager _unitManager;
    ClientPlayerData _playerData;

    // Start is called before the first frame update
    void Start()
    {
        _tilemap = GetComponent<Tilemap>();
        _unitManager = GetComponent<UnitManager>();
        _playerData = ProjectUtilities.FindClientPlayerData();

        EventBus.Subscribe<CreateUnitRequest>(OnCreateUnitRequest);
    }

    // Requests the server to create a new unit of the given type at the given tile
    public void RequestSpawnUnit(UnitType unitType,
        HexCoordinateOffset initialHex)
    {
        CreateUnitRequest request = new(unitType,
            initialHex,
            _playerData.PlayerID);
        var rpcAction = new Action<NetworkRunner, PlayerRef, CreateUnitRequest>(
            ClientMessages.RPC_CreateUnit); 
        EventBus.Publish(new NetworkInputEvent(request,
            rpcAction,
            Runner));
    }

    // If the given create unit request is valid, spawns a new unit object and
    // creates a new unit; does nothing if the request is invalid
    void OnCreateUnitRequest(CreateUnitRequest request)
    {
        Debug.Log("Received create unit request for player " +
            request.RequestingPlayerID.ToString() + " at " + 
            request.Location.ToString());

        if (_unitManager.ValidateCreateUnitRequest(request))
        {
            Debug.Log("Request successful");
            UnitCreated unitCreated = new(request);
            EventBus.Publish(unitCreated);
            ServerMessages.RPC_UnitCreated(Runner,
                unitCreated);
        }
    }

    // Spawns a UnitObject onto the tilemap at the given hex with the given unit ID
    // Throws a RuntimeException if the unit prefab is missing necessary components
    public void SpawnUnitObject(UnitID unitID, 
        PlayerID ownerID,
        HexCoordinateOffset hex)
    {
        SpawnableObject spawnable = _unitPrefab.GetComponent<SpawnableObject>() ??
            throw new RuntimeException(
                "Failed to get SpawnableObject component from unit prefab");

        Vector3Int tilemapLocation = hex.ConvertToVector3Int();

        Vector3 spawnLocation = _tilemap.CellToWorld(tilemapLocation) + 
            spawnable.YOffset;
        NetworkObject newUnitObject = Runner.Spawn(_unitPrefab,
            spawnLocation,
            Quaternion.identity);

        UnitObject newUnit = newUnitObject.GetComponent<UnitObject>() ??
            throw new RuntimeException(
                "Failed to get UnitObject component from unit prefab");

        newUnit.UnitID = unitID;
        newUnit.OwnerID = ownerID;
    }
}
