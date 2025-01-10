using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using System;

// ------------------------------------------------------------------
// Component providing functionality to move this object around the
// tilemap
// ------------------------------------------------------------------

[RequireComponent(typeof(SpawnableObject))]
public class MoveableObject : MonoBehaviour
{
    [SerializeField] float _motionTimePerHex;


    SpawnableObject _spawnable;
    Tilemap _tilemap;

    void Start()
    {
        _spawnable = GetComponent<SpawnableObject>();
        _tilemap = ObjectFinder.FindTilemap();    
    }

    // Moves this object to given hex 
    public void MoveTo(HexCoordinateOffset hex)
    {
        Vector3 newPos = HexToWorld(hex);
        transform.DOMove(newPos, 1f);
    }

    // Moves this object along the given path, and executes optional
    // onCompleteCallback after movement is finished
    public void MoveAlongPath(List<HexCoordinateOffset> path,
        TweenCallback onCompleteCallback = null)
    {
        Sequence pathSequence = DOTween.Sequence();
        foreach (HexCoordinateOffset hex in path)
        {
            Vector3 hexPos = HexToWorld(hex);
            pathSequence.Append(transform.DOMove(hexPos, 
                _motionTimePerHex));
        }
        if (onCompleteCallback != null)
            pathSequence.OnComplete(onCompleteCallback);

        pathSequence.Play();
    }

    // Converts the given hex to world position
    Vector3 HexToWorld(HexCoordinateOffset hex)
    {
        return _tilemap.CellToWorld(hex.ConvertToVector3Int()) + 
            Vector3.zero; // TODO
    }
}
