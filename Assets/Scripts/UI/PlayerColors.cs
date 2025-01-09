using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColors : MonoBehaviour
{
    [SerializeField] List<Color> _playerColors = new();

    public Color GetPlayerColor(PlayerID playerID)
    {
        if (playerID.ID >= _playerColors.Count)
            throw new ArgumentException($"No color exists for player {playerID}");

        return _playerColors[playerID.ID];
    }

    // Sets the color of obj based on the given owner ID
    // Assumes that obj is structured such that its first child is a 
    // GameObject with a Renderer component
    public static void SetObjectColor(GameObject obj,
        PlayerID ownerID)
    {
        GameObject visualsChild = UnityUtilities.GetFirstChildGameObject(obj);
        Renderer renderer = visualsChild.GetComponent<Renderer>() ??
            throw new RuntimeException(
                "Failed to get Renderer component from child");

        PlayerColors colors = ProjectUtilities.FindPlayerColors();
        renderer.material.color = colors.GetPlayerColor(ownerID);
    }
}
