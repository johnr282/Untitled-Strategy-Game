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

        CalculateMapDimensions();
        // Initialize _tilemap with width and height from map generation parameters
        _mapVisuals.InitializeVisuals(_parameters.GetMapHeight(), _parameters.GetMapWidth());

        GenerateMap();

        // Updates map visuals based off terrain of tiles in _hexMap
        _mapVisuals.UpdateVisuals(_gameMap);
    }

    // Calculates the height and width of the map based on number of continents
    // and bounds on continent size
    void CalculateMapDimensions()
    {
        int medianContinentRadius = 
            (_parameters.GetMaxContinentRadius() - _parameters.GetMinContinentRadius()) / 2 
            + _parameters.GetMinContinentRadius();

        Debug.Log("Median continent radius: " + medianContinentRadius.ToString());

        int medianContinentDiameter = medianContinentRadius * 2;
        int mapWidth = medianContinentDiameter * _parameters.GetNumContinents();
        _parameters.SetMapWidth(mapWidth);

        int mapHeight = Mathf.FloorToInt(mapWidth * _parameters.GetWidthToHeightRatio());
        _parameters.SetMapHeight(mapHeight);

        Debug.Log("Map width: " + mapWidth.ToString());
        Debug.Log("Map height: " + mapHeight.ToString());
    }

    // Randomly generate the game map based on map generation parameters
    void GenerateMap()
    {
        InitializeEveryTileToSea();

    }   

    // Initialize every tile in _gameMap to sea
    void InitializeEveryTileToSea()
    {
        for(int row = 0; row < _parameters.GetMapHeight(); row++)
        {
            for(int col = 0;  col < _parameters.GetMapWidth(); col++)
            {
                Vector3Int coordinate = new Vector3Int(col, row, 0);
                Terrain seaTerrain = new Terrain(Terrain.TerrainType.sea);
                GameTile newTile = new GameTile(coordinate, seaTerrain);
                _gameMap.AddTile(coordinate, newTile);
            }
        }
    }

    // Generate continents without considering terrain
    void GenerateContinents()
    {
        
    }

    // Choose central tiles for each continent and return list of these coordinates
    Vector3Int[] ChooseContinentCentralTiles()
    {
        Vector3Int[] centralCoordinates = new Vector3Int[_parameters.GetNumContinents()];



        return centralCoordinates;
    }
}
