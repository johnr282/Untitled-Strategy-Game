using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component for storing map generation parameters
// ------------------------------------------------------------------

public class MapGenerationParameters : MonoBehaviour
{
    [SerializeField] float _widthToHeightRatio;
    public float WidthToHeightRatio { get => _widthToHeightRatio; }

    [SerializeField] int _numContinents;
    public int NumContinents { get => _numContinents; }

    [SerializeField] int _averageContinentDiameter;
    public int AverageContinentDiameter { get => _averageContinentDiameter; }

    // The higher these parameters, the farther the continents will be spread apart
    [SerializeField] float _continentDiameterToGridCellSizeRatio;
    public float ContinentDiameterToGridCellSizeRatio 
    { get => _continentDiameterToGridCellSizeRatio; }

    [SerializeField] int _minDistanceBetweenCentralContinentTiles;
    public int MinDistanceBetweenCentralContinentTiles 
    { get => _minDistanceBetweenCentralContinentTiles; }

    // MapHeight and MapWidth are calculated based on other parameters, so
    // they aren't serialized
    public int MapHeight { get; set; }
    public int MapWidth { get; set; }
}