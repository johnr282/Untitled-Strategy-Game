using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component that handles storing and updating all game state. All 
// modifying of game state occurs here, both on the server and the
// clients. Game state updates are only published after the client
// request that resulted in the state update has been validated. 
// ------------------------------------------------------------------

[RequireComponent(typeof(GameMap))]
[RequireComponent(typeof(UnitManager))]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(UnitSpawner))]
public class GameStateManager : NetworkBehaviour
{
    // All game state is contained within these three components
    GameMap _gameMap;
    UnitManager _unitManager;
    PlayerManager _playerManager;

    UnitSpawner _unitSpawner;

    // Start is called before the first frame update
    void Start()
    {
        _gameMap = GetComponent<GameMap>();
        _unitManager = GetComponent<UnitManager>();
        _playerManager = GetComponent<PlayerManager>();
        _unitSpawner = GetComponent<UnitSpawner>();

        // Game state update subscriptions
        EventBus.Subscribe<GameStarted>(OnGameStarted);
        EventBus.Subscribe<TurnChanged>(OnTurnStarted);
        EventBus.Subscribe<UnitCreated>(OnUnitCreated);
        EventBus.Subscribe<UnitMoved>(OnUnitMoved);
    }

    // Handles updating game state on the server and all clients with the given
    // updateData and RPC; given RPC should publish updateData as well, allowing
    // the game state to be updated on all clients
    public static void UpdateGameState<TUpdate>(NetworkRunner runner,
        TUpdate updateData,
        Action<NetworkRunner, TUpdate> RPC_UpdateClientState)
        where TUpdate : struct, INetworkStruct
    {
        if (!runner.IsServer)
            throw new ArgumentException(
                "UpdateGameState shoould only be called by the server");

        // Updates the game state on the server, only needed if a dedicated
        // server with no local player is running
        if (runner.GameMode == GameMode.Server)
            EventBus.Publish(updateData);

        // Updates the game state on all clients
        RPC_UpdateClientState(runner, updateData);
    }

    // Generates the map, initializes map visuals, and spawns a unit at this 
    // player's starting tile
    void OnGameStarted(GameStarted gameStarted)
    {
        MapGenerationParameters parameters = 
            ProjectUtilities.FindMapGenerationParameters();

        if (parameters.RandomlyGenerateSeed)
            parameters.Seed = gameStarted.MapSeed;

        MapGenerator mapGenerator = new(_gameMap, parameters);
        mapGenerator.GenerateMap();

        MapVisuals mapVisuals = ProjectUtilities.FindMapVisuals();
        mapVisuals.GenerateVisuals(_gameMap,
            parameters.MapHeight,
            parameters.MapWidth);

        List<GameTile> startingTiles = 
            mapGenerator.GenerateStartingTiles(_playerManager.NumPlayers);
        GameTile startingTile = startingTiles[_playerManager.ThisPlayerID.ID];
        UnitType startingUnitType = UnitType.land;
        _unitSpawner.RequestSpawnUnit(startingUnitType, startingTile.Hex);
    }

    void OnTurnStarted(TurnChanged turnStarted)
    {

    }

    void OnUnitCreated(UnitCreated unitCreated)
    {
        UnitID newUnitID = _unitManager.CreateUnit(unitCreated.UnitInfo);

        if (Runner.IsServer)
        {
            _unitSpawner.SpawnUnitObject(newUnitID,
                unitCreated.UnitInfo.RequestingPlayerID,
                unitCreated.UnitInfo.Location);
        }
    }

    void OnUnitMoved(UnitMoved unitMoved)
    {
        GameTile destTile = _gameMap.GetTile(unitMoved.NewLocation);
        _unitManager.MoveUnit(unitMoved.UnitID,
            destTile);
    }
}
