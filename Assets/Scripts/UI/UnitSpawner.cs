using Fusion;
using System;
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

    // Start is called before the first frame update
    void Start()
    {
        _tilemap = ProjectUtilities.FindTilemap();
        _playerData = ProjectUtilities.FindClientPlayerData();

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
        //Debug.Log("Is resimulation before rpc call: " + Runner.IsResimulation.ToString());

        CreateUnitRequest request = new(unitType,
            initialTile,
            _playerData.PlayerID);
        var rpcAction = 
            (Action<NetworkRunner, PlayerRef, CreateUnitRequest>)ClientMessages.RPC_CreateUnit; 
        EventBus.Publish(new NetworkInputEvent(request,
            rpcAction,
            Runner));


        //while (Runner.IsResimulation) ;
        //RpcInvokeInfo info = ClientMessages.RPC_CreateUnit(Runner,
        //    PlayerRef.None,
        //    new CreateUnitRequest(unitType, 
        //        initialTile,
        //        _playerData.PlayerID));

        //Debug.Log("Is resimulation after rpc call: " + Runner.IsResimulation.ToString());
        //Debug.Log("Called RPC_CreateUnit, invoke info: " + info.ToString());
        //Debug.Log("RPC send result: " + info.SendResult.Result.ToString());
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
