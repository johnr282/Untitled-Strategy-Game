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
    bool _highlightingPath = false;

    void Start()
    {
        _tileSelectedSub = EventBus.Subscribe<TileSelectedEvent>(OnTileSelected);
    }

    void OnTileSelected(TileSelectedEvent tileSelectedEvent)
    {
        if (_highlightingPath)
            return;

        if (_startSelected)
        {
            _highlightingPath = true;
            HexCoordinateOffset goal = HexUtilities.ConvertToHexCoordinateOffset(
                tileSelectedEvent.Coordinate);

            Debug.Log("Finding path between " + _start.ToString() + " and " + goal.ToString());
            List<HexCoordinateOffset> path = HexUtilities.FindShortestPath(_start, 
                goal, 
                _gameMap.CostByLand);

            Debug.Log("Path found");
            
            foreach (HexCoordinateOffset hex in path)
            {
                _mapVisuals.SelectTile(hex.ConvertToVector3Int());
            }

            _startSelected = false;
            _highlightingPath = false;
        }
        else
        {
            _start = HexUtilities.ConvertToHexCoordinateOffset(
                tileSelectedEvent.Coordinate);
            _startSelected = true;
        }
    }
}