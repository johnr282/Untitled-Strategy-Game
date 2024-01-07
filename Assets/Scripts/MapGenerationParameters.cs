using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerationParameters : MonoBehaviour
{
    [SerializeField] float _widthToHeightRatio;
    [SerializeField] int _numContinents;
    [SerializeField] int _minContinentRadius;
    [SerializeField] int _maxContinentRadius;

    // _mapHeight and _mapWidth are calculated based on numContinents, 
    // minContinentRadius, and maxContinent radius, so they aren't serialized
    int _mapHeight;
    int _mapWidth;

    public float GetWidthToHeightRatio() {  return _widthToHeightRatio; }

    public int GetNumContinents() {  return _numContinents; }

    public int GetMinContinentRadius() {  return _minContinentRadius; }

    public int GetMaxContinentRadius() { return _maxContinentRadius; }

    public int GetMapHeight() { return _mapHeight; }
    public void SetMapHeight(int mapHeight) { _mapHeight = mapHeight; }

    public int GetMapWidth() { return _mapWidth; }
    public void SetMapWidth(int mapWidth) { _mapWidth = mapWidth; }
}
