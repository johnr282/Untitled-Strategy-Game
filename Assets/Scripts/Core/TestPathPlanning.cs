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
            List<HexCoordinateOffset> path = HexUtilities.FindShortestPath(_start, 
                goal, 
                _gameMap.CostByLand);
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