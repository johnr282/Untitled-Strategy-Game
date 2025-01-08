using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// ------------------------------------------------------------------
// Component managing the map initialization and territory selection phase
// of the game; it destroys itself when territory selection ends
// ------------------------------------------------------------------

public class TerritorySelectionManager : SimulationBehaviour
{
    int[] _remainingUnitBudgets = null;
    [SerializeField] int _startingUnitBudget; 

    // Start is called before the first frame update
    void Start()
    {
        StateManager.RegisterStateUpdate<StartGameUpdate>(gameStarted => true,
            OnGameStarted,
            StateManagerRPCs.RPC_StartGameServer,
            StateManagerRPCs.RPC_StartGameClient);

        StateManager.RegisterStateUpdate<PlaceTerritorySelectionUnitUpdate>(ValidatePlaceTerritorySelectionUnitUpdate,
            PlaceTerritorySelectionUnit,
            StateManagerRPCs.RPC_PlaceTerritorySelectionUnitServer,
            StateManagerRPCs.RPC_PlaceTerritorySelectionUnitClient);
    }

    void OnGameStarted(StartGameUpdate startGameUpdate)
    {
        EventBus.Publish(startGameUpdate);
        GameTile thisPlayerStartTile = InitializeMap(startGameUpdate.MapSeed);
        InitializeTerritorySelectionPhase(thisPlayerStartTile);
    }

    // Generates the map and map visuals, returns this player's starting tile
    GameTile InitializeMap(int mapSeed)
    {
        Debug.Log("Initializaing map");

        MapGenerationParameters parameters = 
            ProjectUtilities.FindMapGenerationParameters();
        if (parameters.RandomlyGenerateSeed)
            parameters.Seed = mapSeed;

        MapGenerator mapGenerator = new(parameters);
        mapGenerator.GenerateMap();

        MapVisuals mapVisuals = ProjectUtilities.FindMapVisuals();
        mapVisuals.GenerateVisuals(parameters.MapHeight,
            parameters.MapWidth);

        GameTile[] startingTiles = 
            mapGenerator.GenerateStartingTiles(PlayerManager.NumPlayers);
        return startingTiles[PlayerManager.MyPlayerID.ID];
    }

    void InitializeTerritorySelectionPhase(GameTile thisPlayerStartTile)
    {
        Debug.Log("Initializaing territory selection phase");

        int startingUnitBudget = CalculateStartingUnitBudget();
        _remainingUnitBudgets = new int[PlayerManager.NumPlayers];
        for (int i = 0; i < _remainingUnitBudgets.Length; i++)
        {
            _remainingUnitBudgets[i] = startingUnitBudget;
        }

        // Spawn first unit for this player at their start tile
        UnitType startingUnitType = UnitType.land;
        StateManager.RequestStateUpdate(new CreateUnitUpdate(startingUnitType,
            thisPlayerStartTile.Hex,
            PlayerManager.MyPlayerID));

        Debug.Log("Finished initializing Territory Selection Phase");
        EventBus.Publish(new TerritorySelectionPhaseStartedEvent(startingUnitBudget));
        EventBus.Subscribe<TileSelectedEvent>(OnTileSelected);
        PlayerManager.NotifyActivePlayer();
    }

    // Calculates the starting unit budget based on the number of players
    // and map size, not counting the player's start unit
    int CalculateStartingUnitBudget()
    {
        // TODO
        return _startingUnitBudget;
    }

    void OnTileSelected(TileSelectedEvent e)
    {
        if (!PlayerManager.MyTurn)
            return;

        StateManager.RequestStateUpdate(new PlaceTerritorySelectionUnitUpdate(
            e.Coordinate,
            PlayerManager.MyPlayerID));
    }

    void PlaceTerritorySelectionUnit(PlaceTerritorySelectionUnitUpdate update)
    {
        Debug.Log("Player " + update.RequestingPlayerID + 
            " claimed or reinforced tile " + update.Location);

        bool ok = StateManager.RequestStateUpdate(new CreateUnitUpdate(UnitType.land,
            update.Location,
            update.RequestingPlayerID), 
            true);
        if (!ok)
        {
            Debug.Log("Player " + update.RequestingPlayerID + 
                " failed to place territory selection unit on tile " + update.Location);
            return;
        }

        _remainingUnitBudgets[update.RequestingPlayerID.ID]--;
        int remainingBudget = _remainingUnitBudgets[update.RequestingPlayerID.ID];

        if (PlayerManager.MyPlayerID.ID == update.RequestingPlayerID.ID)
            EventBus.Publish(new TerritorySelectionUnitPlacedEvent(remainingBudget));
    }

    bool ValidatePlaceTerritorySelectionUnitUpdate(PlaceTerritorySelectionUnitUpdate update)
    {
        if (!PlayerManager.ThisPlayersTurn(update.RequestingPlayerID))
            return false;

        // The player must have a remaning unit budget
        if (_remainingUnitBudgets[update.RequestingPlayerID.ID] <= 0)
            return false;

        GameTile selectedTile = GameMap.GetTile(update.Location);
        if (!selectedTile.Available(update.RequestingPlayerID))
            return false;

        // Selected territories must be adjacent to at least one already owned territory
        List<GameTile> neighbors = GameMap.Neighbors(selectedTile);
        foreach (GameTile neighbor in neighbors)
        {
            if (neighbor.IsOwnedBy(update.RequestingPlayerID))
                return true;
        }

        return false;
    }
}
