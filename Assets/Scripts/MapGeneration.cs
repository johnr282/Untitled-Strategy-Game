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
        _mapVisuals.InitializeVisuals(_parameters.MapHeight(), _parameters.MapWidth());

        GenerateMap();

        // Updates map visuals based off terrain of tiles in _hexMap
        _mapVisuals.UpdateVisuals(_gameMap);
    }

    // Calculates the height and width of the map based on number of continents
    // and bounds on continent size
    void CalculateMapDimensions()
    {
        int mapWidth = _parameters.AverageContinentDiameter() * _parameters.NumContinents();
        _parameters.SetMapWidth(mapWidth);

        int mapHeight = Mathf.FloorToInt(mapWidth * _parameters.WidthToHeightRatio());
        _parameters.SetMapHeight(mapHeight);

        Debug.Log("Map width: " + mapWidth.ToString());
        Debug.Log("Map height: " + mapHeight.ToString());
    }

    // Randomly generate the game map based on map generation parameters
    void GenerateMap()
    {
        InitializeEveryTileToSea();
        GenerateContinents();
    }   

    // Initialize every tile in _gameMap to sea
    void InitializeEveryTileToSea()
    {
        for(int row = 0; row < _parameters.MapHeight(); row++)
        {
            for(int col = 0;  col < _parameters.MapWidth(); col++)
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
        Vector3Int[] centralCoordinates  = ChooseContinentCentralTiles();

        foreach(Vector3Int centralCoordinate in centralCoordinates)
        {
            _gameMap.ChangeTerrain(centralCoordinate, Terrain.TerrainType.land);
        }
    }

    // Choose central tiles for each continent and return list of these coordinates
    Vector3Int[] ChooseContinentCentralTiles()
    {
        // Divide map into square cells of size cellSize x cellSize and randomly 
        // choose a point in each cell
        int cellSize = _parameters.AverageContinentDiameter();
        List<Vector3Int> cellPoints = ChoosePointInEachCell(cellSize);

        // Pick numContinents points out of the randomly chosen points to be the central 
        // tiles for each continent
        Vector3Int[] centralCoordinates = ChooseNFromList(cellPoints, _parameters.NumContinents());
        return centralCoordinates;
    }

    // Divides map into square cells of given size and randomly chooses a point in each
    // cell; returns the list of points
    List<Vector3Int> ChoosePointInEachCell(int cellSize)
    {
        // Iterate through the square cells and pick a random point in each one
        List<Vector3Int> cellPoints = new List<Vector3Int>();

        for (int row = 0; row < _parameters.MapHeight(); row += cellSize)
        {
            for (int col = 0; col < _parameters.MapWidth(); col += cellSize)
            {
                // If cell doesn't fit evenly into map, decrease size of cell so it fits
                int exclusiveUpperRowBound = row + cellSize;
                if (exclusiveUpperRowBound >= _parameters.MapHeight())
                    exclusiveUpperRowBound = _parameters.MapHeight();

                int exclusiveUpperColBound = col + cellSize;
                if (exclusiveUpperColBound >= _parameters.MapWidth())
                    exclusiveUpperColBound = _parameters.MapWidth();

                int randRow = Random.Range(row, exclusiveUpperRowBound);
                int randCol = Random.Range(col, exclusiveUpperColBound);
                cellPoints.Add(new Vector3Int(randCol, randRow, 0));
            }
        }

        return cellPoints;
    }

    // Randomly chooses n points out of given list; returns array of chosen points
    Vector3Int[] ChooseNFromList(List<Vector3Int> list, int n)
    {
        Vector3Int[] chosenPoints = new Vector3Int[n];

        for(int i = 0; i < n; i++)
        {
            int randIndex = Random.Range(0, list.Count);
            chosenPoints[i] = list[randIndex];
            list.RemoveAt(randIndex);
        }

        return chosenPoints;
    }
}
