using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPathPlanning : MonoBehaviour
{
    [SerializeField] MapVisuals _mapVisuals;
    [SerializeField] GameMap _gameMap;
    [SerializeField] UnitType _unitType;
    [SerializeField] bool _findAllPaths;

    Subscription<TileSelectedEvent> _tileSelectedSub;

    HexCoordinateOffset _start;
    bool _startSelected = false;

    void Start()
    {
        _tileSelectedSub = EventBus.Subscribe<TileSelectedEvent>(OnTileSelected);
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

        Func<HexCoordinateOffset, HexCoordinateOffset, int> costFunc = (startHex, goalHex)
            => _gameMap.CostToTraverse(_unitType, startHex, goalHex);
        Predicate<HexCoordinateOffset> traversableFunc = (hex)
            => _gameMap.Traversable(_unitType, hex);

        List<HexCoordinateOffset> path;
        try
        {
            path = HexUtilities.FindShortestPath(start,
                goal,
                costFunc,
                traversableFunc);

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

        foreach (HexCoordinateOffset hex in path)
        {
            vectorPath.Add(hex.ConvertToVector3Int());
        }
        _mapVisuals.HighlightPath(vectorPath);
    }

    public void FindAllPaths(HexCoordinateOffset start)
    {
        Action<GameTile> findPathToTile = (tile) =>
        {
            if (_gameMap.Traversable(_unitType, tile.Coordinate))
                FindPath(start, tile.Coordinate);
        };

        _gameMap.ExecuteOnAllTiles(findPathToTile);
    }
}