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

    [SerializeField] UnitType _unitType;
    [SerializeField] bool _findAllPaths;

    Subscription<TileSelectedEvent> _tileSelectedSub;

    Unit _dummyUnit;
    HexCoordinateOffset _start;
    bool _startSelected = false;

    void Start()
    {
        _mapVisuals = ProjectUtilities.FindComponent<MapVisuals>("GameMap");

        _tileSelectedSub = EventBus.Subscribe<TileSelectedEvent>(OnTileSelected);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe(_tileSelectedSub);    
    }

    void OnTileSelected(TileSelectedEvent tileSelectedEvent)
    {
        if (_startSelected)
        {
            HexCoordinateOffset goal = tileSelectedEvent.Coordinate;
            Unit dummyUnit = new(_unitType,
                GameMap.GetTile(_start),
                new UnitID(0));
            FindPath(dummyUnit, goal);
            _startSelected = false;
        }
        else
        {
            _start = tileSelectedEvent.Coordinate;
            _startSelected = true;

            if (_findAllPaths)
            {
                Unit dummyUnit = new(_unitType,
                    GameMap.GetTile(_start),
                    new UnitID(0));
                FindAllPaths(dummyUnit);
                _startSelected = false;
            }
        }
    }

    public void FindPath(Unit unit, 
        HexCoordinateOffset goal)
    {
        HexCoordinateOffset start = unit.CurrentLocation.Hex;
        Debug.Log("Finding path between " + start.ToString() + " and " + goal.ToString() +
                ", distance of " + HexUtilities.DistanceBetween(start, goal).ToString());

        GameTile goalTile = GameMap.GetTile(goal);

        List<HexCoordinateOffset> path;
        try
        {
            path = GameMap.FindPath(unit, 
                goalTile);
        }
        catch (RuntimeException e)
        {
            Debug.LogError(e.Message);
            return;
        }

        Debug.Log("Path found");
        _mapVisuals.HighlightPath(path);
    }

    public void FindAllPaths(Unit unit)
    {


        Action<GameTile> findPathToTile = (tile) =>
        {
            if (GameMap.Traversable(unit, tile))
                FindPath(unit, tile.Hex);
        };

        GameMap.ExecuteOnAllTiles(findPathToTile);
    }
}