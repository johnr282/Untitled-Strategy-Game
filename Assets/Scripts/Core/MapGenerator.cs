using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

// ------------------------------------------------------------------
// Class handling procedural map generation
// ------------------------------------------------------------------

public class MapGenerator 
{
    public int MapSeed { get => _parameters.Seed; }

    MapGenerationParameters _parameters;

    public MapGenerator(MapGenerationParameters parametersIn)
    {
        _parameters = parametersIn;
    }

    public static int GenerateRandomSeed()
    {
        return Random.Range(int.MinValue, int.MaxValue);
    }

    // Struct representing a continent in the process of being generated
    struct ContinentInProgress
    {
        public Continent ContinentTiles { get; set; }
        public ContinentID ContinentID { get; }
        public HexCoordinateOffset CentralCoordinate { get; }
        public int TargetRadius { get; }
        public int CurrentRadius { get; set; }
        public float PerlinOffset { get; }

        public ContinentInProgress(ContinentID continentIDIn,
            HexCoordinateOffset centralCoordinateIn,
            int targetRadiusIn,
            float perlinOffsetIn)
        {
            ContinentTiles = new();
            ContinentID = continentIDIn;
            CentralCoordinate = centralCoordinateIn;
            TargetRadius = targetRadiusIn;
            CurrentRadius = 0;
            PerlinOffset = perlinOffsetIn;
        }

        // Returns whether this continent is finished generating
        public bool FinishedGenerating()
        {
            return CurrentRadius >= TargetRadius;
        }
    }

    // Randomly generate the game map based on map generation parameters
    // and given seed; called from GenerateMapCallback on clients, called
    // from playerManager on server
    public void GenerateMap()
    {
        Debug.Log("Generating map with seed " + _parameters.Seed.ToString());
        SeedRandomGeneration(_parameters.Seed);
        CalculateMapDimensions();
        InitializeEveryTileToSea();
        GenerateContinents();
    }

    // Initializes random generation with given seed
    void SeedRandomGeneration(int seed)
    {
        Random.InitState(seed);
    }

    // Calculates the height and width of the map based on number of continents
    // and bounds on continent size
    void CalculateMapDimensions()
    {
        float averageContinentDiameter = _parameters.AverageContinentRadius * 2;
        _parameters.MapWidth = Mathf.FloorToInt(averageContinentDiameter *
            _parameters.ContinentDiameterToGridCellSizeRatio *
            _parameters.NumContinents);

        _parameters.MapHeight = Mathf.FloorToInt(_parameters.MapWidth *
            _parameters.WidthToHeightRatio);

        Debug.Log("Map width: " + _parameters.MapWidth.ToString());
        Debug.Log("Map height: " + _parameters.MapHeight.ToString());
    }

    // Initialize every tile in GameMap to sea
    void InitializeEveryTileToSea()
    {
        for(int row = 0; row < _parameters.MapHeight; row++)
        {
            for(int col = 0;  col < _parameters.MapWidth; col++)
            {
                HexCoordinateOffset coordinate = new HexCoordinateOffset(col, row);
                GameTile seaTile = new(coordinate,
                    Terrain.sea,
                    new ContinentID(-1));
                GameMap.AddTile(coordinate, 
                    seaTile);
            }
        }
    }

    // Generate continents without considering terrain
    void GenerateContinents()
    {
        List<ContinentInProgress> continents = InitializeContinents();

        // Generate each continent one ring at a time
        while (!AllContinentsFinishedGenerating(continents))
        {
            for (int i = 0; i < _parameters.NumContinents; i++)
            {
                // Can't directly pass continents[i] in as a ref
                ContinentInProgress continent = continents[i];
                if (continent.FinishedGenerating())
                    continue;

                continent.CurrentRadius++;
                GenerateContinentRing(ref continent);
                continents[i] = continent;
            }
        }

        // Add generated continents to GameMap
        foreach (ContinentInProgress continent in continents)
        {
            GameMap.AddContinent(continent.ContinentID,
                continent.ContinentTiles);
        }
    }

    // Returns a ContinentInProgress list and initializes the central coordinate
    // of each continent in the GameMap
    List<ContinentInProgress> InitializeContinents()
    {
        List<ContinentInProgress> continents = new();
        List<HexCoordinateOffset> centralCoordinates = ChooseContinentCentralTiles();

        for (int i = 0; i <  _parameters.NumContinents; i++)
        {
            ContinentID continentID = new(i);
            HexCoordinateOffset centralCoordinate = centralCoordinates[i];

            ContinentInProgress newContinent = new(continentID,
                centralCoordinate,
                GenerateRadius(),
                GeneratePerlinOffset());

            GameTile centralTile = new(centralCoordinate,
                    Terrain.land,
                    continentID);
            GameMap.SetTile(centralCoordinate, centralTile);
            newContinent.ContinentTiles.AddTile(centralTile);

            continents.Add(newContinent);
        }

        return continents;
    }

    // Returns whether all of the continents in the given list are finished
    // generating
    bool AllContinentsFinishedGenerating(List<ContinentInProgress> continents)
    {
        foreach (ContinentInProgress continent in continents)
        {
            if (!continent.FinishedGenerating())
                return false;
        }

        return true;
    }

    // Generates a continent radius
    int GenerateRadius()
    {
        int continentRadius = UnityUtilities.NormalDistributionInt(
                _parameters.AverageContinentRadius,
                _parameters.StdDevContinentRadius);

        if (continentRadius <= 0)
            continentRadius = 1;

        return continentRadius;
    }

    // Generates a perlin offset to use in continent generation
    float GeneratePerlinOffset()
    {
        return Random.Range(0.0f, _parameters.MaxPerlinOffset);
    }

    // Choose central tiles for each continent and return list of these coordinates
    // Throws RuntimeException if selection process fails too many times
    List<HexCoordinateOffset> ChooseContinentCentralTiles()
    {
        // Divide map into grid of square cells of size cellSize x cellSize and randomly 
        // choose a point in each cell
        float averageContinentDiameter = _parameters.AverageContinentRadius * 2;
        int cellSize = Mathf.FloorToInt(
             averageContinentDiameter * 
            _parameters.ContinentDiameterToGridCellSizeRatio);

        //Debug.Log("Cell size: " + cellSize.ToString());

        ChoosePointInEachCell(cellSize, 
            out List<HexCoordinateOffset> randomPoints,
            out Dictionary<HexCoordinateOffset, Vector2Int> correspondingGridCells);

        // Pick numContinents points out of the randomly chosen points to be the central 
        // tiles for each continent; selection process can fail, hence the while loop
        List<HexCoordinateOffset> centralCoordinates;
        bool selectionFailed;
        int numAttempts = 0;
        do
        {
            numAttempts++;
            if (numAttempts > _parameters.MaxCentralTileSelectionAttempts)
                throw new RuntimeException(
                    "Exceeded maximum continent central tile selection attempts");

            centralCoordinates = SelectCentralTilesFromList(
                _parameters.NumContinents,
                randomPoints,
                correspondingGridCells);
            selectionFailed = (centralCoordinates.Count == 0);
        } while (selectionFailed);

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

    // Randomly chooses currentRadius points out of given list, returns list of chosen points;
    // points are less likely to be chosen if tiles in adjacent grid cells have
    // already been chosen
    // Returns empty list if selection process failed, which could happen if too
    // many of the randomly chosen points are too close together
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
            int chosenIndex = 0; // dummy value to prevent compiler error
            bool pointSelected = false;

            while (!pointSelected)
            {
                // If randomPoints is empty, selection process has failed and empty
                // list is returned
                if (randomPoints.Count == 0)
                    return new List<HexCoordinateOffset>();

                chosenIndex = Random.Range(0, randomPoints.Count);
                HexCoordinateOffset possiblePoint = randomPoints[chosenIndex];
                if (TooCloseToChosenPoint(possiblePoint, chosenPoints))
                {
                    randomPoints.RemoveAt(chosenIndex);
                    continue;
                }

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
        foreach (HexCoordinateOffset chosenPoint in chosenPoints) 
        {
            if (HexUtilities.DistanceBetween(point, chosenPoint) < 
                _parameters.MinDistanceBetweenCentralContinentTiles)
            {
                return true;
            }
        }
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

    // Generates the next ring of the continent with given central coordinate,
    // ID, and radius; updates continent as the ring is generated
    void GenerateContinentRing(ref ContinentInProgress continent)
    {
        List<HexCoordinateOffset> ring = 
            continent.CentralCoordinate.HexesExactlyNAway(continent.CurrentRadius);
        
        foreach (HexCoordinateOffset hex in ring)
        {
            if (IncludeTile(hex, 
                continent.ContinentID, 
                continent.PerlinOffset))
            {
                GameTile newTile = new(hex, Terrain.land, continent.ContinentID);
                GameMap.SetTile(hex, 
                    newTile);
                continent.ContinentTiles.AddTile(newTile);
            }
        }
    }

    // Returns whether given hex should be included in continent specified by
    // given continent ID
    bool IncludeTile(HexCoordinateOffset hex, 
        ContinentID continentID, 
        float offset)
    {
        GameTile tile;
        try
        {
            tile = GameMap.GetTile(hex);
        }
        catch (ArgumentException)
        {
            // Hex must be outside the map bounds
            return false;
        }

        if (tile.InContinent())
            return false;

        // If tile is an island, i.e. not adjacent to any other tiles from the 
        // same continent, don't include it
        List<GameTile> adjacentTiles = GameMap.Neighbors(tile);
        bool connectedToContinent = false;

        foreach (GameTile adjacentTile in adjacentTiles)
        {
            if (adjacentTile.ContinentID.ID == continentID.ID)
            {
                connectedToContinent = true;
                break;
            }
        }

        if (!connectedToContinent)
            return false;

        float perlinValue = CalculatePerlinValue(hex, offset);
        return  perlinValue > _parameters.LandGenerationThreshold;
    }

    // Returns Perlin noise value for given hex and offset
    float CalculatePerlinValue(HexCoordinateOffset hex, 
        float offset)
    {
        // Divide by col + 1 and row + 1 to ensure val isn't an integer, + 1
        // because col and row could be 0
        float x = (offset + hex.Col) / (hex.Col + 1) *
            _parameters.PerlinCoordinateScalingFactor;
        float y = (offset + hex.Row) / (hex.Row + 1) *
            _parameters.PerlinCoordinateScalingFactor;
        float val = Mathf.PerlinNoise(x, y);
        return Mathf.Clamp(val, 0.0f, 1.0f);
    }

    // Returns a list of currentRadius unique starting tiles; each tile is guaranteed to be
    // on land and on a different continent
    // Throws an ArgumentException if currentRadius is greater than the number of continents
    public List<GameTile> GenerateStartingTiles(int n)
    {
        if (n > GameMap.NumContinents)
            throw new ArgumentException("n cannot be greater than NumContinents");

        List<GameTile> startingTiles = new();
        List<int> availableContinents = GameMap.ContinentIDList();

        for (int i = 0; i < n; i++)
        {
            if (availableContinents.Count == 0)
                availableContinents = GameMap.ContinentIDList();

            int continentID = UnityUtilities.RandomElement(availableContinents);
            Continent continent = GameMap.GetContinent(continentID);
            availableContinents.Remove(continentID);

            GameTile startingTile = 
                UnityUtilities.RandomElement(continent.ContinentTiles);
            startingTiles.Add(startingTile);
        }

        return startingTiles;
    }
}