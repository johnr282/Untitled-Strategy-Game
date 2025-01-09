using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// ------------------------------------------------------------------
// Component for displaying each player's remaining infantry budget
// during territory selection
// ------------------------------------------------------------------

[RequireComponent(typeof(TextMeshProUGUI))]
public class TerritorySelectionInfantryBudgetDisplay : MonoBehaviour
{
    TextMeshProUGUI _infantryBudgetDisplay;

    void Start()
    {
        _infantryBudgetDisplay = GetComponent<TextMeshProUGUI>();
        gameObject.SetActive(false); // Off by default

        EventBus.Subscribe<TerritorySelectionPhaseStartedEvent>(OnTerritorySelectionPhaseStarted);
        EventBus.Subscribe<TerritorySelectionUnitPlacedEvent>(OnNewTerritorySelectionInfantryPlaced);
        EventBus.Subscribe<TerritorySelectionPhaseEndedEvent>(OnTerritorySelectionPhaseEnded);
    }

    void OnTerritorySelectionPhaseStarted(TerritorySelectionPhaseStartedEvent e)
    {
        SetRemainingInfantry(e.InfantryBudget);
        gameObject.SetActive(true);
    }

    void OnNewTerritorySelectionInfantryPlaced(TerritorySelectionUnitPlacedEvent e)
    {
        if (e.PlayerID.ID == PlayerManager.MyPlayerID.ID)
            SetRemainingInfantry(e.NewRemainingInfantryBudget);
    }

    void OnTerritorySelectionPhaseEnded(TerritorySelectionPhaseEndedEvent e)
    {
        gameObject.SetActive(false);
    }

    void SetRemainingInfantry(int remainingInfantry)
    {
        _infantryBudgetDisplay.text = "Remaining Infantry: " + remainingInfantry;
    }
}