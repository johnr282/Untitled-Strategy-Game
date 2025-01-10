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
    Subscription<TileSelectedEvent> _tileSelectedSubscription;
    int[] _remainingUnitBudgets = null;
    bool _selectingTerritory = true;
    bool[] _capitalsSelected = null;

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

        StateManager.RegisterStateUpdate<PlaceCapitalUpdate>(StateManager.DefaultValidator,
            PlaceCapital,
            StateManagerRPCs.RPC_PlaceCapitalServer,
            StateManagerRPCs.RPC_PlaceCapitalClient);
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
            ObjectFinder.FindMapGenerationParameters();
        if (parameters.RandomlyGenerateSeed)
            parameters.Seed = mapSeed;

        MapGenerator mapGenerator = new(parameters);
        mapGenerator.GenerateMap();

        MapVisuals mapVisuals = ObjectFinder.FindMapVisuals();
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
        _capitalsSelected = new bool[PlayerManager.NumPlayers];

        for (int i = 0; i < PlayerManager.NumPlayers; i++)
        {
            _remainingUnitBudgets[i] = startingUnitBudget;
            _capitalsSelected[i] = false;
        }

        // Spawn first unit for this player at their start tile
        StateManager.RequestStateUpdate(new CreateUnitUpdate(StartingUnitType,
            thisPlayerStartTile.Hex,
            PlayerManager.MyPlayerID));

        Debug.Log("Finished initializing Territory Selection Phase");
        EventBus.Publish(new TerritorySelectionPhaseStartedEvent(startingUnitBudget));
        _tileSelectedSubscription = EventBus.Subscribe<TileSelectedEvent>(OnTileSelected);
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

        if (_selectingTerritory)
            RequestPlaceTerritorySelectionUnit(e.Coordinate);
        else
            RequestPlaceCapital(e.Coordinate);
    }

    void RequestPlaceTerritorySelectionUnit(HexCoordinateOffset hex)
    {
        StateManager.RequestStateUpdate(new PlaceTerritorySelectionUnitUpdate(
            hex,
            PlayerManager.MyPlayerID));
    }

    void RequestPlaceCapital(HexCoordinateOffset hex)
    {
        StateManager.RequestStateUpdate(new PlaceCapitalUpdate(
            hex,
            PlayerManager.MyPlayerID));
    }

    void PlaceTerritorySelectionUnit(PlaceTerritorySelectionUnitUpdate update)
    {
        Debug.Log("Player " + update.RequestingPlayerID + 
            " claimed or reinforced tile " + update.Location);

        //bool myRequest = PlayerManager.MyPlayerID.ID == update.RequestingPlayerID.ID;
        //if (myRequest)
        //{
        //    bool ok = StateManager.RequestStateUpdate(new CreateUnitUpdate(
        //        StartingUnitType,
        //        update.Location,
        //        update.RequestingPlayerID));
        //    if (!ok)
        //        return;
        //}

        _remainingUnitBudgets[update.RequestingPlayerID.ID]--;
        int remainingBudget = _remainingUnitBudgets[update.RequestingPlayerID.ID];

        EventBus.Publish(new TerritorySelectionUnitPlacedEvent(
            update.RequestingPlayerID,
            remainingBudget));

        if (AllPlayersOutOfUnits())
        {
            EventBus.Publish(new SelectingCapitalLocationsEvent());
            _selectingTerritory = false;
        }
    }

    bool ValidatePlaceTerritorySelectionUnitUpdate(PlaceTerritorySelectionUnitUpdate update,
        out string failureReason)
    {
        failureReason = "";

        if (!PlayerManager.ThisPlayersTurn(update.RequestingPlayerID))
        {
            failureReason = "Not your turn";
            return false;
        }

        // The player must have a remaning unit budget
        if (_remainingUnitBudgets[update.RequestingPlayerID.ID] <= 0)
        {
            failureReason = "Out of territory selection units";
            return false;
        }

        GameTile selectedTile = GameMap.GetTile(update.Location);
        if (!selectedTile.Available(update.RequestingPlayerID))
        {
            failureReason = "Tile is not available";
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

        failureReason = "Not adjacent to at least one owned tile";
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

    void PlaceCapital(PlaceCapitalUpdate update)
    {
        Debug.Log($"Player {update.RequestingPlayerID} placed capital at {update.Location}");
        _capitalsSelected[update.RequestingPlayerID.ID] = true;

        if (AllCapitalsSelected())
        {
            Debug.Log("All players have placed their capitals, territory selection is complete");
            EventBus.Publish(new TerritorySelectionPhaseEndedEvent());
            EventBus.Unsubscribe(_tileSelectedSubscription);
        }
    }

    bool AllCapitalsSelected()
    {
        foreach (bool selected in _capitalsSelected)
        {
            if (!selected)
                return false;
        }

        return true;
    }
}

// Places a player's capital, the final step in the territory selection phase
public readonly struct PlaceCapitalUpdate : IStateUpdate
{
    public HexCoordinateOffset Location { get; }
    public PlayerID RequestingPlayerID { get; }

    public PlaceCapitalUpdate(HexCoordinateOffset locationIn,
        PlayerID requestingPlayerIDIn)
    {
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }

    public List<IStateUpdate> GetStateUpdatesInOrder()
    {
        return new List<IStateUpdate>
        {
            new CreateStructureUpdate(StructureType.Capital, Location, RequestingPlayerID),
            this,
            new EndActivePlayersTurnUpdate(RequestingPlayerID)
        };
    }
}

// Starts the game after all players have joined
public readonly struct StartGameUpdate : IStateUpdate
{
    public int MapSeed { get; }

    public StartGameUpdate(int mapSeedIn)
    {
        MapSeed = mapSeedIn;
    }

    public List<IStateUpdate> GetStateUpdatesInOrder() => new List<IStateUpdate> { this };
}

// Places a territory selection unit to either claim a new tile or
// reinforce an already claimed tile
public readonly struct PlaceTerritorySelectionUnitUpdate : IStateUpdate
{
    public HexCoordinateOffset Location { get; }
    public PlayerID RequestingPlayerID { get; }

    public PlaceTerritorySelectionUnitUpdate(HexCoordinateOffset locationIn,
        PlayerID requestingPlayerIDIn)
    {
        Location = locationIn;
        RequestingPlayerID = requestingPlayerIDIn;
    }

    public List<IStateUpdate> GetStateUpdatesInOrder()
    {
        return new List<IStateUpdate>
        {
            new CreateUnitUpdate(UnitType.Infantry, Location, RequestingPlayerID),
            this,
            new EndActivePlayersTurnUpdate(RequestingPlayerID)
        };
    }
}