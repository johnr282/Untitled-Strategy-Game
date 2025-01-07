using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component for storing and accessing the current game phase, as well
// as handling the transitions between different phases
// ------------------------------------------------------------------

public enum GamePhase
{
    StartingSession,
    TerritorySelection,
    Normal
}

public class GamePhaseManager : MonoBehaviour
{
    public static GamePhase CurrentPhase { get; private set; } = GamePhase.StartingSession;

    void Start()
    {
        EventBus.Subscribe<TerritorySelectionPhaseStartedEvent>(OnTerritorySelectionPhaseStarted);    
    }

    void OnTerritorySelectionPhaseStarted(TerritorySelectionPhaseStartedEvent e)
    {
        Debug.Log("Current game phase is now TerritorySelection");
        CurrentPhase = GamePhase.TerritorySelection;
    }
}
