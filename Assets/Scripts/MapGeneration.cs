using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component for handling procedural map generation
// ------------------------------------------------------------------

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
        int mapWidth = Mathf.FloorToInt(_parameters.AverageContinentDiameter() * 
            _parameters.ContinentDiameterToGridCellSizeRatio() * 
            _parameters.NumContinents());
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
                Vector2Int coordinate = new Vector2Int(col, row);
                Terrain seaTerrain = new Terrain(Terrain.TerrainType.sea);
                GameTile newTile = new GameTile(coordinate, seaTerrain);
                _gameMap.AddTile(coordinate, newTile);
            }
        }
    }

    // Generate continents without considering terrain
    void GenerateContinents()
    {
        List<Vector2Int> centralCoordinates  = ChooseContinentCentralTiles();

        foreach(Vector2Int centralCoordinate in centralCoordinates)
        {
            _gameMap.ChangeTerrain(centralCoordinate, Terrain.TerrainType.land);
        }
    }

    // Choose central tiles for each continent and return list of these coordinates
    List<Vector2Int> ChooseContinentCentralTiles()
    {
        // Divide map into grid of square cells of size cellSize x cellSize and randomly 
        // choose a point in each cell
        int cellSize = Mathf.FloorToInt(
            _parameters.AverageContinentDiameter() * 
            _parameters.ContinentDiameterToGridCellSizeRatio());
        List<Vector2Int> cellPoints = ChoosePointInEachCell(cellSize, 
            out int gridWidth, 
            out int gridHeight);

        // Pick numContinents points out of the randomly chosen points to be the central 
        // tiles for each continent
        List<Vector2Int> centralCoordinates = SelectCentralTilesFromList(cellPoints, 
            gridWidth, 
            gridHeight, 
            _parameters.NumContinents());
        return centralCoordinates;
    }

    // Divides map into square cells of given size and randomly chooses a point in each
    // cell; returns the list of points; calculates width and height of grid
    List<Vector2Int> ChoosePointInEachCell(int cellSize, 
        out int gridWidth, 
        out int gridHeight)
    {
        // Iterate through the square cells and pick a random point in each one
        List<Vector2Int> cellPoints = new();

        gridWidth = 0;
        gridHeight = 0;

        for (int row = 0; row < _parameters.MapHeight(); row += cellSize)
        {
            gridHeight++;
            for (int col = 0; col < _parameters.MapWidth(); col += cellSize)
            {
                gridWidth++;

                // If cell doesn't fit evenly into map, decrease size of cell so it fits
                int exclusiveUpperRowBound = row + cellSize;
                if (exclusiveUpperRowBound >= _parameters.MapHeight())
                    exclusiveUpperRowBound = _parameters.MapHeight();

                int exclusiveUpperColBound = col + cellSize;
                if (exclusiveUpperColBound >= _parameters.MapWidth())
                    exclusiveUpperColBound = _parameters.MapWidth();

                int randRow = Random.Range(row, exclusiveUpperRowBound);
                int randCol = Random.Range(col, exclusiveUpperColBound);
                cellPoints.Add(new Vector2Int(randCol, randRow));
            }
        }

        return cellPoints;
    }

    // Randomly chooses n points out of given list; returns array of chosen points;
    // points are less likely to be chosen if tiles in adjacent grid cells have
    // already been chosen
    List<Vector2Int> SelectCentralTilesFromList(List<Vector2Int> points, 
        int gridWidth, 
        int gridHeight, 
        int n)
    {
        List<Vector2Int> chosenPoints = new();
        HashSet<Vector2Int> chosenGridCoordinates = new();

        for(int i = 0; i < n; i++)
        {
            // An index needs to randomly chosen numAdjacentCellsChosen times in a row
            // to be selected; this makes it much less likely that a cell adjacent to
            // several chosen cells will be chosen, which will ensure that the
            // continents aren't all clustered together
            int chosenIndex = Random.Range(0, points.Count); // dummy value to prevent compiler error
            bool pointSelected = false;

            while (!pointSelected)
            {
                chosenIndex = Random.Range(0, points.Count);
                int numAdjacentCellsChosen = AdjacentCellsChosen(chosenIndex,
                    chosenGridCoordinates,
                    gridWidth);

                if(numAdjacentCellsChosen == 0)
                    pointSelected = true;

                for (int j = 0; j < numAdjacentCellsChosen; j++)
                {
                    int confirmIndex = Random.Range(0, points.Count);
                    if (chosenIndex == confirmIndex)
                    {
                        pointSelected = true;
                        continue;
                    }  
                    else
                    {
                        pointSelected = false;
                        break;
                    }
                }
            }

            chosenPoints.Add(points[chosenIndex]);
            points.RemoveAt(chosenIndex);
            chosenGridCoordinates.Add(IndexToCoordinate(chosenIndex, 
                gridWidth));
        }

        return chosenPoints;
    }

    // Returns true if given point is less than
    // _parameters.MinDistanceBetweenCentralContinentTiles away from an already
    // chosen point
    bool TooCloseToChosenPoint(Vector2Int point, 
        List<Vector2Int> chosenPoints)
    {
        return false;
    }

    // Returns number of points in grid cells adjacent to given index that have
    // already been chosen
    int AdjacentCellsChosen(int cellIndex, 
        HashSet<Vector2Int> chosenGridCoordinates, 
        int gridWidth)
    {
        Vector2Int coordinate = IndexToCoordinate(cellIndex, gridWidth);

        // Calculates coordinates of all adjacent (including diagonal) grid cells
        // and adds them to array
        Vector2Int[] adjacentCoords = CalculateAdjacentCoordinates(coordinate);

        // Counts how many adjacent coordinates are in chosenGridCoordinates
        int numAdjacentCellsChosen = 0;
        foreach(Vector2Int coord in adjacentCoords)
        {
            if(chosenGridCoordinates.Contains(coord))
                numAdjacentCellsChosen++;
        }

        return numAdjacentCellsChosen;
    }

    // Converts given index of row-major array to corresponding 2D grid
    // coordinate; needs the number of columns in grid
    Vector2Int IndexToCoordinate(int index,
        int columnsInGrid)
    {
        int gridRow = index / columnsInGrid;
        int gridCol = index % columnsInGrid;
        return new Vector2Int(gridRow, gridCol);
    }

    // Given a 2D coordinate, return an array of the 8 adjacent coordinates 
    // (including diagonal)
    Vector2Int[] CalculateAdjacentCoordinates(Vector2Int coordinate)
    {
        Vector2Int[] adjacentCoords = new Vector2Int[8];

        Vector2Int southCell = coordinate + Vector2Int.down;
        Vector2Int southWestCell = coordinate + new Vector2Int(-1, -1);
        Vector2Int westCell = coordinate + Vector2Int.left;
        Vector2Int northWestCell = coordinate + new Vector2Int(-1, 1);
        Vector2Int northCell = coordinate + Vector2Int.up;
        Vector2Int northEastCell = coordinate + new Vector2Int(1, 1);
        Vector2Int eastCell = coordinate + Vector2Int.right;
        Vector2Int southEastCell = coordinate + new Vector2Int(1, -1);

        adjacentCoords[0] = southCell;
        adjacentCoords[1] = southWestCell;
        adjacentCoords[2] = westCell;
        adjacentCoords[3] = northWestCell;
        adjacentCoords[4] = northCell;
        adjacentCoords[5] = northEastCell;
        adjacentCoords[6] = eastCell;
        adjacentCoords[7] = southEastCell;

        return adjacentCoords;
    }
}
