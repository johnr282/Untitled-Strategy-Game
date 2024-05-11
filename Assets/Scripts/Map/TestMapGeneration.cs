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

        HexCoordinateOffset start = new HexCoordinateOffset(7, 0);
        HexCoordinateOffset goal = new HexCoordinateOffset(1, 1);
        List<HexCoordinateOffset> path = PathPlanning.FindShortestPath(start,
            goal,
            HexUtilities.DistanceBetween);
    }
}
