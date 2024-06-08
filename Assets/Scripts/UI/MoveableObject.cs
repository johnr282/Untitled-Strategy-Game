using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

// ------------------------------------------------------------------
// Component providing functionality to move this object around the
// tilemap
// ------------------------------------------------------------------

[RequireComponent(typeof(SpawnableObject))]
[RequireComponent(typeof(NetworkTransform))]
public class MoveableObject : NetworkBehaviour
{
    Queue<Vector3> _positionUpdates = new();

    NetworkTransform _networkTransform;
    SpawnableObject _spawnable;
    Tilemap _tilemap;

    void Start()
    {
        _networkTransform = GetComponent<NetworkTransform>();
        _spawnable = GetComponent<SpawnableObject>();
        _tilemap = ProjectUtilities.FindTilemap();    
    }

    public override void FixedUpdateNetwork()
    {
        if (_positionUpdates.Count != 0)
            transform.position = _positionUpdates.Dequeue();
    }

    // Move this object to given hex on the map
    // More accurately, queues a position update to occur in FixedUpdateNetwork
    // as NetworkTransform updates don't work unless they are done there
    public void MoveTo(HexCoordinateOffset hex)
    {
        Vector3 newPos = _tilemap.CellToWorld(hex.ConvertToVector3Int()) 
            + _spawnable.YOffset;
        _positionUpdates.Enqueue(newPos);
    }
}
