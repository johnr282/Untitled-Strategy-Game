using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(MapVisuals))]
[RequireComponent(typeof(HexMap))]
[RequireComponent(typeof(MapGenerationParameters))]
public class MapGeneration : MonoBehaviour
{
    MapVisuals _mapVisuals;
    HexMap _hexMap;

    MapGenerationParameters _parameters;

    void Start()
    {
         _mapVisuals = GetComponent<MapVisuals>();   
        _hexMap = GetComponent<HexMap>();
        _parameters = GetComponent<MapGenerationParameters>();

        InitializeMapVisuals();
        UpdateMapVisuals();
    }

    // Initialize _tilemap with width and height from map generation parameters
    void InitializeMapVisuals()
    {
        _mapVisuals.InitializeVisuals(_parameters.mapHeight, _parameters.mapWidth);
    }

    // Randomly generate the game map based on map generation parameters
    void GenerateMap()
    {

    }

    // Updates map visuals based off terrain of tiles in _hexMap
    void UpdateMapVisuals()
    {
        _mapVisuals.UpdateVisuals(_hexMap);
    }

    
}
