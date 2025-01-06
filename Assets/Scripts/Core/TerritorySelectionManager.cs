using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// ------------------------------------------------------------------
// Component managing the map initialization and territory selection phase
// of the game; it is created by the SessionManager when the phase starts
// and destroyed by itself when it ends
// ------------------------------------------------------------------

public class TerritorySelectionManager : SimulationBehaviour
{
    int[] _remainingInfantryBudgets = new int[PlayerManager.NumPlayers];

    // Start is called before the first frame update
    void Start()
    {
        GameTile thisPlayerStartTile = InitializeMap();
        InitializeTerritorySelectionPhase(thisPlayerStartTile);   
    }

    // Generates the map and map visuals, returns this player's starting tile
    GameTile InitializeMap()
    {
        Debug.Log("Initializaing map");
        MapGenerationParameters parameters = 
            ProjectUtilities.FindMapGenerationParameters();
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
        int startingInfantryBudget = CalculateStartingInfantryBudget();
        for (int i = 0; i < _remainingInfantryBudgets.Length; i++)
        {
            _remainingInfantryBudgets[i] = startingInfantryBudget;
        }

        // Spawn first unit for this player at their start tile
        UnitType startingUnitType = UnitType.land;
        StateManager.RequestStateUpdate(new CreateUnitUpdate(startingUnitType,
            thisPlayerStartTile.Hex,
            PlayerManager.MyPlayerID));

        Debug.Log("Finished initializing Territory Selection Phase, notifying active player");
        EventBus.Publish(new TerritorySelectionPhaseStartedEvent(startingInfantryBudget));
        PlayerManager.NotifyActivePlayer();
    }

    // Calculates the starting infantry budget based on the number of players
    // and map size
    int CalculateStartingInfantryBudget()
    {
        // TODO
        return 5;
    }
}
