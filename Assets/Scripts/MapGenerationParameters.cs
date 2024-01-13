using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerationParameters : MonoBehaviour
{
    [SerializeField] float _widthToHeightRatio;
    [SerializeField] int _numContinents;
    [SerializeField] int _averageContinentDiameter;

    // The higher this parameter, the farther the continents will be spread apart
    [SerializeField] float _continentDiameterToGridCellSizeRatio;

    // _mapHeight and _mapWidth are calculated based on numContinents, 
    // minContinentRadius, and maxContinent radius, so they aren't serialized
    int _mapHeight;
    int _mapWidth;

    public float WidthToHeightRatio() {  return _widthToHeightRatio; }

    public int NumContinents() {  return _numContinents; }

    public int AverageContinentDiameter() {  return _averageContinentDiameter; }

    public float ContinentDiameterToGridCellSizeRatio() { return _continentDiameterToGridCellSizeRatio; }



    public int MapHeight() { return _mapHeight; }
    public void SetMapHeight(int mapHeight) { _mapHeight = mapHeight; }

    public int MapWidth() { return _mapWidth; }
    public void SetMapWidth(int mapWidth) { _mapWidth = mapWidth; }
}
