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
// Once a ClientRequest has been validated on the server, UpdateGameState()
// should be called with the corresponding update struct (found in
// GameStateUpdates.cs) and RPC, resulting in the game state being
// updated (for everyone) in one of the callback functions in this class.
// ------------------------------------------------------------------

[RequireComponent(typeof(UnitObjectSpawner))]
public class GameStateManager : NetworkBehaviour
{
    UnitObjectSpawner _unitSpawner;

    // Start is called before the first frame update
    void Start()
    {
        _unitSpawner = GetComponent<UnitObjectSpawner>();

        // Game state update subscriptions
        //EventBus.Subscribe<AddPlayerUpdate> (OnAddPlayer);
        //EventBus.Subscribe<StartGameUpdate> (OnGameStarted);
        //EventBus.Subscribe<EndTurnUpdate>    (OnNextTurn);
        //EventBus.Subscribe<CreateUnitUpdate> (OnUnitCreated);
        //EventBus.Subscribe<MoveUnitUpdate>   (OnUnitMoved);
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
    //void OnAddPlayer(AddPlayerUpdate update)
    //{
    //    //Debug.Log("Adding player to PlayerManager");
    //    //PlayerID newPlayerID = PlayerManager.AddPlayerUpdate(update.PlayerRef);

    //    //if (Runner.IsServer)
    //    //{
    //    //    ServerMessages.RPC_SendPlayerID(Runner,
    //    //        update.PlayerRef,
    //    //        newPlayerID);
    //    //}
    //}

    // Generates the map, initializes map visuals, and spawns a unit at this 
    // player's starting tile
    //void OnGameStarted(StartGameUpdate update)
    //{
    //    Debug.Log("Game starting, generating map and spawning starting unit");
    //    MapGenerationParameters parameters = 
    //        ProjectUtilities.FindMapGenerationParameters();

    //    if (parameters.RandomlyGenerateSeed)
    //        parameters.Seed = update.MapSeed;

    //    MapGenerator mapGenerator = new(parameters);
    //    mapGenerator.GenerateMap();

    //    MapVisuals mapVisuals = ProjectUtilities.FindMapVisuals();
    //    mapVisuals.GenerateVisuals(parameters.MapHeight,
    //        parameters.MapWidth);

    //    List<GameTile> startingTiles = 
    //        mapGenerator.GenerateStartingTiles(PlayerManager.NumPlayers);
    //    GameTile startingTile = startingTiles[PlayerManager.MyPlayerID.ID];
    //    UnitType startingUnitType = UnitType.land;
    //    _unitSpawner.RequestSpawnUnit(startingUnitType, startingTile.Hex);

    //    PlayerManager.NotifyActivePlayer();
    //}

    //// Updates the current active player and notifies them
    //void OnNextTurn(EndTurnUpdate update)
    //{
    //    PlayerManager.UpdateCurrTurnIndex();
    //    if (Runner.IsPlayer)
    //        PlayerManager.NotifyActivePlayer();

    //    Debug.Log("ActivePlayer is now " + PlayerManager.ActivePlayer);
    //    Debug.Log("MyTurn: " + PlayerManager.MyTurn);
    //}

    // Creates a unit and its corresponding UnitObjectbased on the given update
    //void OnUnitCreated(CreateUnitUpdate update)
    //{
    //    UnitID newUnitID = UnitManager.CreateUnit(update.UnitInfo);
    //    Debug.Log("Created new unit " + newUnitID.ID.ToString());

    //    UnitObject newUnitObject = _unitSpawner.SpawnUnitObject(newUnitID,
    //        update.UnitInfo.RequestingPlayerID,
    //        update.UnitInfo.Location);
    //    UnitManager.GetUnit(newUnitID).UnitObject = newUnitObject;
    //}

    //// Moves a unit and its corresponding UnitObject based on the given update
    //void OnUnitMoved(MoveUnitUpdate update)
    //{
    //    Debug.Log("Moving unit " + update.UnitID.ID.ToString());

    //    // Need to move UnitObject before updating state
    //    GameTile destTile = GameMap.GetTile(update.NewLocation);
    //    Unit unitToMove = UnitManager.GetUnit(update.UnitID);
    //    unitToMove.UnitObject.MoveTo(update.NewLocation);

    //    UnitManager.MoveUnit(update.UnitID,
    //        destTile);

    //    if (PlayerManager.MyTurn)
    //        PlayerManager.EndMyTurn();
    //}
}