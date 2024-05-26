using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component for testing map generation in isolation from multiplayer
// ------------------------------------------------------------------

public class TestMapGeneration : MonoBehaviour
{
    void Start()
    {
        EventBus.Publish(new GameStarted(MapGenerator.GenerateRandomSeed()));
    }
}
