using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component for testing map generation in isolation from multiplayer
// ------------------------------------------------------------------

public class TestMapGeneration : MonoBehaviour
{
    MapGeneration _mapGenerator;

    void Start()
    {
        _mapGenerator = ProjectUtilities.FindComponent<MapGeneration>("GameMap");
        _mapGenerator.GenerateRandomSeed();
        EventBus.Publish(new GenerateMapEvent(_mapGenerator.GetMapSeed()));
    }
}