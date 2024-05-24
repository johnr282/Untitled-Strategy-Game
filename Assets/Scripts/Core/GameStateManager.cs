using Fusion;
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
        EventBus.Subscribe<TurnStarted>(OnTurnStarted);
        EventBus.Subscribe<UnitCreated>(OnUnitCreated);
        EventBus.Subscribe<UnitMoved>(OnUnitMoved);
    }

    void OnGameStarted(GameStarted gameStarted)
    {
        
    }

    void OnTurnStarted(TurnStarted turnStarted)
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
