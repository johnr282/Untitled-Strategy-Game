using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Utility functions used throughout this project
// ------------------------------------------------------------------

public static class ProjectUtilities
{
    public const string MapObjectName = "Map";
    public const string GameStateManagerObjectName = "GameStateManager";
    public const string MapGenerationParametersObjectName = "MapGenerationParameters";

    // Finds and returns component of given type on GameObject of given name 
    // Throws RuntimeException if GameObject or component cannot be found
    public static TComponent FindComponent<TComponent>(string gameObjectName)
    {
        GameObject gameObject = GameObject.Find(gameObjectName) ??
            throw new RuntimeException(
                "Cannot find " + gameObjectName + " GameObject");

        TComponent component = gameObject.GetComponent<TComponent>() ??
            throw new RuntimeException(
                "Cannot find " + typeof(TComponent).ToString() + 
                " component on " + gameObjectName);

        return component;
    }

    public static Tilemap FindTilemap()
    {
        return FindComponent<Tilemap>(MapObjectName);
    }

    public static MapGenerationParameters FindMapGenerationParameters()
    {
        return FindComponent<MapGenerationParameters>(MapGenerationParametersObjectName);
    }

    public static MapVisuals FindMapVisuals()
    {
        return FindComponent<MapVisuals>(MapObjectName);
    }
}