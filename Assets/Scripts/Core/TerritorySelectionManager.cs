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
    const UnitType StartingUnitType = UnitType.Infantry;

    [SerializeField] int _startingUnitBudget; 
    int[] _remainingUnitBudgets = null;
    bool selectingTerritory = true;

    // Start is called before the first frame update
    void Start()
    {
        StateManager.RegisterStateUpdate<StartGameUpdate>(StateManager.DefaultValidator,
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
        StateManager.RequestStateUpdate(new CreateUnitUpdate(StartingUnitType,
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

        if (selectingTerritory)
        {
            StateManager.RequestStateUpdate(new PlaceTerritorySelectionUnitUpdate(
                e.Coordinate,
                PlayerManager.MyPlayerID));
        } 
        else
        {
            StateManager.RequestStateUpdate(new CreateStructureUpdate(
                StructureType.Capital,
                e.Coordinate,
                PlayerManager.MyPlayerID));
        }
    }

    void PlaceTerritorySelectionUnit(PlaceTerritorySelectionUnitUpdate update)
    {
        Debug.Log("Player " + update.RequestingPlayerID + 
            " claimed or reinforced tile " + update.Location);

        bool myRequest = PlayerManager.MyPlayerID.ID == update.RequestingPlayerID.ID;
        if (myRequest)
        {
            bool ok = StateManager.RequestStateUpdate(new CreateUnitUpdate(
                StartingUnitType,
                update.Location,
                update.RequestingPlayerID));
            if (!ok)
            {
                Debug.Log("Player " + update.RequestingPlayerID +
                    " failed to place territory selection unit on tile " + update.Location);
                return;
            }
        }

        _remainingUnitBudgets[update.RequestingPlayerID.ID]--;
        int remainingBudget = _remainingUnitBudgets[update.RequestingPlayerID.ID];

        if (AllPlayersOutOfUnits())
        {
            EventBus.Publish(new SelectingCapitalLocationsEvent());
            selectingTerritory = false;
        }

        if (myRequest)
        {
            EventBus.Publish(new TerritorySelectionUnitPlacedEvent(remainingBudget));
            PlayerManager.EndMyTurn();
        }
    }

    bool ValidatePlaceTerritorySelectionUnitUpdate(PlaceTerritorySelectionUnitUpdate update,
        out string failureReason)
    {
        failureReason = "";

        if (!PlayerManager.ThisPlayersTurn(update.RequestingPlayerID))
        {
            failureReason = "not your turn";
            return false;
        }

        // The player must have a remaning unit budget
        if (_remainingUnitBudgets[update.RequestingPlayerID.ID] <= 0)
        {
            failureReason = "out of territory selection units";
            return false;
        }

        GameTile selectedTile = GameMap.GetTile(update.Location);
        if (!selectedTile.Available(update.RequestingPlayerID))
        {
            failureReason = "tile is not available";
            return false;
        }

        if (selectedTile.IsOwnedBy(update.RequestingPlayerID))
            return true;

        // Selected territories must be adjacent to at least one already owned territory
        List<GameTile> neighbors = GameMap.Neighbors(selectedTile);
        foreach (GameTile neighbor in neighbors)
        {
            if (neighbor.IsOwnedBy(update.RequestingPlayerID))
                return true;
        }

        failureReason = "not adjacent to at least one owned tile";
        return false;
    }

    // Returns true if all players have placed all their units
    bool AllPlayersOutOfUnits()
    {
        foreach (int remainingUnits in _remainingUnitBudgets)
        {
            if (remainingUnits > 0)
                return false;
        }

        return true;
    }
}
