using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component for testing path planning
// ------------------------------------------------------------------

public class TestPathPlanning : MonoBehaviour
{
    MapVisuals _mapVisuals;
    GameMap _gameMap;

    [SerializeField] UnitType _unitType;
    [SerializeField] bool _findAllPaths;

    Subscription<TileSelectedEvent> _tileSelectedSub;

    Unit _dummyUnit;
    HexCoordinateOffset _start;
    bool _startSelected = false;

    void Start()
    {
        _mapVisuals = ProjectUtilities.FindComponent<MapVisuals>("GameMap");
        _gameMap = ProjectUtilities.FindGameMap();

        _tileSelectedSub = EventBus.Subscribe<TileSelectedEvent>(OnTileSelected);
        _dummyUnit = new Unit(_unitType,
            new GameTile(new HexCoordinateOffset(1, 1), Terrain.land), 
            0);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe(_tileSelectedSub);    
    }

    void OnTileSelected(TileSelectedEvent tileSelectedEvent)
    {
        if (_startSelected)
        {
            HexCoordinateOffset goal = HexUtilities.ConvertToHexCoordinateOffset(
                tileSelectedEvent.Coordinate);
            FindPath(_start, goal);
            _startSelected = false;
        }
        else
        {
            _start = HexUtilities.ConvertToHexCoordinateOffset(
                tileSelectedEvent.Coordinate);
            _startSelected = true;

            if (_findAllPaths)
            {
                FindAllPaths(_start);
                _startSelected = false;
            }
        }
    }

    public void FindPath(HexCoordinateOffset start, 
        HexCoordinateOffset goal)
    {
        Debug.Log("Finding path between " + start.ToString() + " and " + goal.ToString() +
                ", distance of " + HexUtilities.DistanceBetween(start, goal).ToString());

        _gameMap.FindTile(start, out GameTile startTile);
        _gameMap.FindTile(goal, out GameTile goalTile);

        List<GameTile> path;
        try
        {
            path = _gameMap.FindPath(_dummyUnit, 
                startTile, 
                goalTile);
        }
        catch (RuntimeException e)
        {
            Debug.LogError(e.Message);
            return;
        }

        Debug.Log("Path found");

        List<Vector3Int> vectorPath = new()
        {
            start.ConvertToVector3Int()
        };

        foreach (GameTile tile in path)
        {
            vectorPath.Add(tile.Hex.ConvertToVector3Int());
        }
        _mapVisuals.HighlightPath(vectorPath);
    }

    public void FindAllPaths(HexCoordinateOffset start)
    {
        Action<GameTile> findPathToTile = (tile) =>
        {
            if (_gameMap.Traversable(_dummyUnit, tile.Hex))
                FindPath(start, tile.Hex);
        };

        _gameMap.ExecuteOnAllTiles(findPathToTile);
    }
}