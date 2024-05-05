using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component for storing and accessing map generation parameters
// ------------------------------------------------------------------

public class MapGenerationParameters : MonoBehaviour
{
    [SerializeField] int _seed;
    public int Seed 
    { 
        get => _seed; 
        set { _seed = value; } 
    }

    [SerializeField] bool _randomlyGenerateSeed;
    public bool RandomlyGenerateSeed { get => _randomlyGenerateSeed; }

    [SerializeField] float _widthToHeightRatio;
    public float WidthToHeightRatio { get => _widthToHeightRatio; }

    [SerializeField] int _numContinents;
    public int NumContinents { get => _numContinents; }

    [SerializeField] float _averageContinentRadius;
    public float AverageContinentRadius { get => _averageContinentRadius; }

    [SerializeField] float _stdDevContinentRadius;
    public float StdDevContinentRadius { get => _stdDevContinentRadius; }

    [SerializeField] float _perlinCoordinateScalingFactor;
    public float PerlinCoordinateScalingFactor { get => _perlinCoordinateScalingFactor; }

    [SerializeField] float _maxPerlinOffset;
    public float MaxPerlinOffset { get => _maxPerlinOffset; }

    // The higher these parameters, the farther the continents will be spread apart
    [SerializeField] float _continentDiameterToGridCellSizeRatio;
    public float ContinentDiameterToGridCellSizeRatio 
    { get => _continentDiameterToGridCellSizeRatio; }

    [SerializeField] int _minDistanceBetweenCentralContinentTiles;
    public int MinDistanceBetweenCentralContinentTiles 
    { get => _minDistanceBetweenCentralContinentTiles; }

    // Must be a value in between 0 and 1
    // Increasing this parameter causes each continent to have less contiguous land
    [SerializeField] float _landGenerationThreshold;
    public float LandGenerationThreshold { get => _landGenerationThreshold; }

    [SerializeField] int _maxCentralTileSelectionAttempts;
    public int MaxCentralTileSelectionAttempts
    { get => _maxCentralTileSelectionAttempts; }

    // MapHeight and MapWidth are calculated based on other parameters, so
    // they aren't serialized
    public int MapHeight { get; set; }
    public int MapWidth { get; set; }
}