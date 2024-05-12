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

        HexCoordinateOffset hex = new HexCoordinateOffset(0, 0);
        HexCoordinateOffset[] neighbors = hex.Neighbors();
        foreach(HexCoordinateOffset neighbor in neighbors)
        {
            Debug.Log("Distance between neighbor and hex: " + 
                HexUtilities.DistanceBetween(hex, neighbor));
            //Debug.Assert(HexUtilities.AreAdjacent(hex, neighbor));
        }
    }
}
