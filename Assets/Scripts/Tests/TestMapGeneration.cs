using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component for testing map generation in isolation from multiplayer
// ------------------------------------------------------------------

public class TestMapGeneration : MonoBehaviour
{
    MapGenerationParameters _parameters;
    MapVisuals _mapVisuals;

    void Start()
    {
        _parameters = ObjectFinder.FindMapGenerationParameters();
        _mapVisuals = ObjectFinder.FindMapVisuals();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RegenerateMap();
        }
    }

    void RegenerateMap()
    {
        GameMap.ClearMap();
        if (_parameters.RandomlyGenerateSeed)
            _parameters.Seed = MapGenerator.GenerateRandomSeed();

        MapGenerator generator = new(_parameters);
        generator.GenerateMap();
        _mapVisuals.GenerateVisuals(_parameters.MapHeight,
            _parameters.MapWidth);
    }
}
