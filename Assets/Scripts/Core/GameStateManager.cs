using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component that handles storing and updating all game state, which
// consists of the static classes GameMap, UnitManager, and PlayerManager.
// All modifying of game state occurs here, both on the server and the
// clients.
// Once a ClientAction has been validated on the server, UpdateGameState()
// should be called with the corresponding update struct (found in
// GameStateUpdates.cs) and RPC, resulting in the game state being
// updated (for everyone) in one of the callback functions in this class.
// ------------------------------------------------------------------

[RequireComponent(typeof(UnitSpawner))]
public class GameStateManager : NetworkBehaviour
{
    UnitSpawner _unitSpawner;

    // Start is called before the first frame update
    void Start()
    {
        _unitSpawner = GetComponent<UnitSpawner>();

        // Game state update subscriptions
        EventBus.Subscribe<PlayerAddedUpdate>   (OnAddPlayer);
        EventBus.Subscribe<GameStartedUpdate> (OnGameStarted);
        EventBus.Subscribe<NextTurnUpdate>    (OnNextTurn);
        EventBus.Subscribe<UnitCreatedUpdate> (OnUnitCreated);
        EventBus.Subscribe<UnitMovedUpdate>   (OnUnitMoved);
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
                "UpdateGameState should only be called by the server");

        // Updates the game state on the server, only needed if a dedicated
        // server with no local player is running
        if (runner.GameMode == GameMode.Server)
            EventBus.Publish(updateData);

        // Updates the game state on all clients
        RPC_UpdateClientState(runner, updateData);
    }

    // Updates PlayerManager with new player, and allows server to send each 
    // client their PlayerID
    void OnAddPlayer(PlayerAddedUpdate update)
    {
        Debug.Log("Adding player to PlayerManager");
        PlayerID newPlayerID = PlayerManager.AddPlayer(update.PlayerRef);

        if (Runner.IsServer)
        {
            ServerMessages.RPC_SendPlayerID(Runner,
                update.PlayerRef,
                newPlayerID);
        }
    }

    // Generates the map, initializes map visuals, and spawns a unit at this 
    // player's starting tile
    void OnGameStarted(GameStartedUpdate update)
    {
        Debug.Log("Game starting, generating map and spawning starting unit");
        MapGenerationParameters parameters = 
            ProjectUtilities.FindMapGenerationParameters();

        if (parameters.RandomlyGenerateSeed)
            parameters.Seed = update.MapSeed;

        MapGenerator mapGenerator = new(parameters);
        mapGenerator.GenerateMap();

        MapVisuals mapVisuals = ProjectUtilities.FindMapVisuals();
        mapVisuals.GenerateVisuals(parameters.MapHeight,
            parameters.MapWidth);

        List<GameTile> startingTiles = 
            mapGenerator.GenerateStartingTiles(PlayerManager.NumPlayers);
        GameTile startingTile = startingTiles[PlayerManager.MyPlayerID.ID];
        UnitType startingUnitType = UnitType.land;
        _unitSpawner.RequestSpawnUnit(startingUnitType, startingTile.Hex);

        PlayerManager.NotifyActivePlayer();
    }

    // Updates the current active player, and publishes an event if it's this
    // client's turn
    void OnNextTurn(NextTurnUpdate update)
    {
        PlayerManager.UpdateCurrTurnIndex();
        if (Runner.IsClient && PlayerManager.MyTurn)
            EventBus.Publish(new MyTurnEvent());

        Debug.Log("ActivePlayer is now " + PlayerManager.ActivePlayer);
        Debug.Log("MyTurn: " + PlayerManager.MyTurn);
    }

    // Creates a unit based on the given update, and allows the server to spawn
    // a networked UnitObject
    void OnUnitCreated(UnitCreatedUpdate update)
    {
        UnitID newUnitID = UnitManager.CreateUnit(update.UnitInfo);
        Debug.Log("Created new unit " + newUnitID.ID.ToString());

        if (Runner.IsServer)
        {
            UnitObject newUnitObject = _unitSpawner.SpawnUnitObject(newUnitID,
                update.UnitInfo.RequestingPlayerID,
                update.UnitInfo.Location);
            UnitManager.GetUnit(newUnitID).UnitObject = newUnitObject;
        }
    }

    // Moves a unit based on the given update, and allows the server to move
    // the corresponding UnitObject
    void OnUnitMoved(UnitMovedUpdate update)
    {
        Debug.Log("Moving unit " + update.UnitID.ID.ToString());
        GameTile destTile = GameMap.GetTile(update.NewLocation);
        UnitManager.MoveUnit(update.UnitID,
            destTile);

        if (Runner.IsServer)
        {
            Unit movedUnit = UnitManager.GetUnit(update.UnitID);
            movedUnit.UnitObject.MoveTo(update.NewLocation);
        }

        if (PlayerManager.MyTurn)
            PlayerManager.EndMyTurn();
    }
}