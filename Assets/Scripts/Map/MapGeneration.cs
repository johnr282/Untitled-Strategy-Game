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

        //GenerateMap();
    }

    // Randomly generate the game map based on map generation parameters
    public void GenerateMap()
    {
        SeedRandomGeneration();
        CalculateMapDimensions();
        InitializeEveryTileToSea();
        GenerateContinents();
        _mapVisuals.GenerateVisuals(_gameMap, 
            _parameters.MapHeight,
            _parameters.MapWidth);
    }

    // Initializes random generation with a seed
    void SeedRandomGeneration()
    {
        // If _parameters.RandomlyGenerateSeed is false, simply use the 
        // serialized seed
        if (_parameters.RandomlyGenerateSeed)
            _parameters.Seed = Random.Range(int.MinValue, int.MaxValue);

        Debug.Log("Seed: " + _parameters.Seed.ToString());
        Random.InitState(_parameters.Seed);
    }

    // Calculates the height and width of the map based on number of continents
    // and bounds on continent size
    void CalculateMapDimensions()
    {
        int mapWidth = Mathf.FloorToInt(_parameters.AverageContinentDiameter *
            _parameters.ContinentDiameterToGridCellSizeRatio *
            _parameters.NumContinents);
        _parameters.MapWidth = mapWidth;

        int mapHeight = Mathf.FloorToInt(mapWidth * _parameters.WidthToHeightRatio);
        _parameters.MapHeight = mapHeight;

        Debug.Log("Map width: " + mapWidth.ToString());
        Debug.Log("Map height: " + mapHeight.ToString());
    }

    // Initialize every tile in _gameMap to sea
    void InitializeEveryTileToSea()
    {
        for(int row = 0; row < _parameters.MapHeight; row++)
        {
            for(int col = 0;  col < _parameters.MapWidth; col++)
            {
                HexCoordinateOffset coordinate = new HexCoordinateOffset(col, row);
                Terrain seaTerrain = new Terrain(Terrain.TerrainType.sea);
                GameTile newTile = new GameTile(coordinate, seaTerrain);
                _gameMap.AddTile(coordinate, newTile);
            }
        }
    }

    // Generate continents without considering terrain
    void GenerateContinents()
    {
        // Choose a central tile for each continent
        List<HexCoordinateOffset> centralCoordinates = ChooseContinentCentralTiles();

        for(int i = 0; i <  centralCoordinates.Count; i++)
        {
            _gameMap.ChangeTerrain(centralCoordinates[i], Terrain.TerrainType.land);
            GenerateContinent(centralCoordinates[i], i);
        }


    }

    // Choose central tiles for each continent and return list of these coordinates
    List<HexCoordinateOffset> ChooseContinentCentralTiles()
    {
        // Divide map into grid of square cells of size cellSize x cellSize and randomly 
        // choose a point in each cell
        int cellSize = Mathf.FloorToInt(
            _parameters.AverageContinentDiameter * 
            _parameters.ContinentDiameterToGridCellSizeRatio);

        //Debug.Log("Cell size: " + cellSize.ToString());

        ChoosePointInEachCell(cellSize, 
            out List<HexCoordinateOffset> randomPoints,
            out Dictionary<HexCoordinateOffset, Vector2Int> correspondingGridCells);

        // Pick numContinents points out of the randomly chosen points to be the central 
        // tiles for each continent
        List<HexCoordinateOffset> centralCoordinates = SelectCentralTilesFromList(
            _parameters.NumContinents,
            randomPoints,
            correspondingGridCells);
        return centralCoordinates;
    }

    // Divides map into square cells of given size and randomly chooses a point in each
    // cell; generates the list of points and creates a dictionary mapping each randomly
    // chosen point to the coordinate of the grid cell it was chosen from
    void ChoosePointInEachCell(int cellSize, 
        out List<HexCoordinateOffset> randomPoints, 
        out Dictionary<HexCoordinateOffset, Vector2Int> correspondingGridCells)
    {
        randomPoints = new();
        correspondingGridCells = new();

        // Keeps track of the coordinate of the current grid cell
        int gridRow = 0;
        int gridCol = 0;

        // Iterate through the square cells and pick a random point in each on
        for (int row = 0; row < _parameters.MapHeight; row += cellSize)
        {
            // Reset gridCol when moving to new row
            gridCol = 0;
            for (int col = 0; col < _parameters.MapWidth; col += cellSize)
            {
                // If cell doesn't fit evenly into map, decrease size of cell so it fits
                int exclusiveUpperRowBound = row + cellSize;
                if (exclusiveUpperRowBound >= _parameters.MapHeight)
                    exclusiveUpperRowBound = _parameters.MapHeight;

                int exclusiveUpperColBound = col + cellSize;
                if (exclusiveUpperColBound >= _parameters.MapWidth)
                    exclusiveUpperColBound = _parameters.MapWidth;

                int randRow = Random.Range(row, exclusiveUpperRowBound);
                int randCol = Random.Range(col, exclusiveUpperColBound);
                HexCoordinateOffset randPoint = new HexCoordinateOffset(randCol, randRow);
                randomPoints.Add(randPoint);
                correspondingGridCells[randPoint] = new Vector2Int(gridCol, gridRow);

                gridCol++;
            }
            gridRow++;
        }
    }

    // Randomly chooses n points out of given list, returns array of chosen points;
    // points are less likely to be chosen if tiles in adjacent grid cells have
    // already been chosen
    List<HexCoordinateOffset> SelectCentralTilesFromList(int n,
        List<HexCoordinateOffset> randomPoints,
        Dictionary<HexCoordinateOffset, Vector2Int> correspondingGridCells)
    {
        List<HexCoordinateOffset> chosenPoints = new();
        HashSet<Vector2Int> chosenGridCoordinates = new();

        for(int i = 0; i < n; i++)
        {
            // An index needs to randomly chosen numAdjacentCellsChosen times in a row
            // to be selected; this makes it much less likely that a cell adjacent to
            // several chosen cells will be chosen, which will ensure that the
            // continents aren't all clustered together
            int chosenIndex = Random.Range(0, randomPoints.Count); // dummy value to prevent compiler error
            bool pointSelected = false;

            while (!pointSelected)
            {
                chosenIndex = Random.Range(0, randomPoints.Count);
                HexCoordinateOffset possiblePoint = randomPoints[chosenIndex];
                Vector2Int correspondingGridCell = correspondingGridCells[possiblePoint];

                int numAdjacentCellsChosen = AdjacentCellsChosen(correspondingGridCell, 
                    chosenGridCoordinates);

                //Debug.Log("Tentative point: " + possiblePoint.ToString() +
                //    ", adjacent cells chosen: " + numAdjacentCellsChosen.ToString());

                if (numAdjacentCellsChosen == 0)
                    pointSelected = true;

                for (int j = 0; j < numAdjacentCellsChosen; j++)
                {
                    int confirmIndex = Random.Range(0, randomPoints.Count);
                    if (chosenIndex == confirmIndex)
                    {
                        //Debug.Log("Index chosen again");
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

            HexCoordinateOffset chosenPoint = randomPoints[chosenIndex];
            Vector2Int chosenGridCoordinate = correspondingGridCells[chosenPoint];

            //Debug.Log("Point chosen: " + chosenPoint.ToString() +
            //    ", adjacent cells chosen: " + AdjacentCellsChosen(chosenGridCoordinate,
            //    chosenGridCoordinates).ToString());

            chosenPoints.Add(chosenPoint);
            randomPoints.RemoveAt(chosenIndex);
            chosenGridCoordinates.Add(chosenGridCoordinate);
        }

        return chosenPoints;
    }

    // Returns true if given point is less than
    // _parameters.MinDistanceBetweenCentralContinentTiles away from an already
    // chosen point
    bool TooCloseToChosenPoint(HexCoordinateOffset point, 
        List<HexCoordinateOffset> chosenPoints)
    {
        return false;
    }

    // Returns number of points in grid cells adjacent to given grid cell that 
    // have already been chosen
    int AdjacentCellsChosen(Vector2Int gridCellCoordinate, 
        HashSet<Vector2Int> chosenGridCoordinates)
    {
        // Calculates coordinates of all adjacent (including diagonal) grid cells
        // and adds them to array
        Vector2Int[] adjacentCoords = CalculateAdjacentCoordinates(gridCellCoordinate);

        // Counts how many adjacent coordinates are in chosenGridCoordinates
        int numAdjacentCellsChosen = 0;
        foreach(Vector2Int coord in adjacentCoords)
        {
            if(chosenGridCoordinates.Contains(coord))
                numAdjacentCellsChosen++;
        }

        return numAdjacentCellsChosen;
    }

    // Given a 2D coordinate, returns an array of the 8 adjacent coordinates 
    // (including diagonal)
    Vector2Int[] CalculateAdjacentCoordinates(Vector2Int coordinate)
    {
        Vector2Int[] adjacentCoords = new Vector2Int[8];

        Vector2Int[] offsets = new Vector2Int[8]
        {
            Vector2Int.down,
            new Vector2Int(-1, -1),
            Vector2Int.left,
            new Vector2Int(-1, 1),
            Vector2Int.up,
            new Vector2Int(1, 1),
            Vector2Int.right,
            new Vector2Int(1, -1)
        };

        for(int i = 0; i < 8; i++)
        {
            adjacentCoords[i] = coordinate + offsets[i];
        }

        return adjacentCoords;
    }

    // Generates continent around given central coordinate with given ID
    void GenerateContinent(HexCoordinateOffset centralCoordinate, 
        int continentID)
    {
        _gameMap.SetContinentID(centralCoordinate, continentID);
    }
}