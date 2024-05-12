using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component for testing map generation in isolation from multiplayer
// ------------------------------------------------------------------

public class TestMapGeneration : MonoBehaviour
{
    [SerializeField] MapGeneration _mapGenerator;
    [SerializeField] GameMap _gameMap;

    void Start()
    {
        _mapGenerator.GenerateRandomSeed();
        EventBus.Publish(new GenerateMapEvent(_mapGenerator.GetMapSeed()));
    }
}
