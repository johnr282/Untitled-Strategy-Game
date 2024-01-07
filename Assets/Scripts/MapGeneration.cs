using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(MapVisuals))]
[RequireComponent(typeof(GameMap))]
[RequireComponent(typeof(MapGenerationParameters))]
public class MapGeneration : MonoBehaviour
{
    MapVisuals _mapVisuals;
    GameMap _gameMap;
    MapGenerationParameters _parameters;

    void Start()
    {
         _mapVisuals = GetComponent<MapVisuals>();   
        _gameMap = GetComponent<GameMap>();
        _parameters = GetComponent<MapGenerationParameters>();

        // Initialize _tilemap with width and height from map generation parameters
        _mapVisuals.InitializeVisuals(_parameters.mapHeight, _parameters.mapWidth);

        GenerateMap();

        // Updates map visuals based off terrain of tiles in _hexMap
        _mapVisuals.UpdateVisuals(_gameMap);
    }

    // Randomly generate the game map based on map generation parameters
    void GenerateMap()
    {
        
    }   

    // Initialize every tile in _gameMap to sea
    void InitializeEveryTileToSea()
    {
        for(int row = 0; row < _parameters.mapHeight; row++)
        {
            for(int col = 0;  col < _parameters.mapWidth; col++)
            {
                
            }
        }
    }
}
