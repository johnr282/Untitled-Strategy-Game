using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPathPlanning : MonoBehaviour
{
    [SerializeField] MapVisuals _mapVisuals;
    [SerializeField] GameMap _gameMap;

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

            Debug.Log("Finding path between " + _start.ToString() + " and " + goal.ToString());

            Func<HexCoordinateOffset, HexCoordinateOffset, int> costFunc = (startHex, goalHex) 
                => _gameMap.CostToTraverse(UnitType.land, startHex, goalHex);
            Predicate<HexCoordinateOffset> traversableFunc = (hex)
                => _gameMap.Traversable(UnitType.land, hex);

            List<HexCoordinateOffset> path;
            try
            {
                path = HexUtilities.FindShortestPath(_start,
                    goal,
                    costFunc,
                    traversableFunc);

            }
            catch (RuntimeException e)
            {
                Debug.LogError(e.Message);
                _startSelected = false;
                return;
            }
            
            Debug.Log("Path found");

            List<Vector3Int> vectorPath = new()
            {
                _start.ConvertToVector3Int()
            };

            foreach (HexCoordinateOffset hex in path)
            {
                vectorPath.Add(hex.ConvertToVector3Int());
            }

            _mapVisuals.HighlightPath(vectorPath);
            _startSelected = false;
        }
        else
        {
            _start = HexUtilities.ConvertToHexCoordinateOffset(
                tileSelectedEvent.Coordinate);
            _startSelected = true;
        }
    }
}