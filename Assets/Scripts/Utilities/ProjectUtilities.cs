using Fusion;
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
    public const string MapGenerationParametersObjectName = "MapGenerationParameters";
    public const string UnitObjectSpawnerObjectName = "UnitObjectSpawner";
    public const string StructureObjectSpawnerObjectName = "StructureObjectSpawner";
    public const string GameParametersObjectName = "GameParameters";
    public const string NetworkedManagersObjectName = "NetworkedManagers";
    public const string PlayerColorsObjectName = "PlayerColors";

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

    public static UnitObjectSpawner FindUnitObjectSpawner()
    {
        return FindComponent<UnitObjectSpawner>(UnitObjectSpawnerObjectName);
    }

    public static StructureObjectSpawner FindStructureObjectSpawner()
    {
        return FindComponent<StructureObjectSpawner>(StructureObjectSpawnerObjectName);
    }

    public static GameParameters FindGameParameters()
    {
        return FindComponent<GameParameters>(GameParametersObjectName);
    }

    public static NetworkObject FindManagersNetworkObject()
    {
        return FindComponent<NetworkObject>(NetworkedManagersObjectName);
    }

    public static PlayerColors FindPlayerColors()
    {
        return FindComponent<PlayerColors>(PlayerColorsObjectName);
    }
}