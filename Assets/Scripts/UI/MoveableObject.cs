using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Component providing functionality to move this object around the
// tilemap
// ------------------------------------------------------------------

public class MoveableObject : MonoBehaviour
{
    Tilemap _tilemap;

    void Start()
    {
        _tilemap = ProjectUtilities.FindTilemap();    
    }

    // Move this object to given position on the tilemap
    public void MoveTo(Vector3Int tilePos)
    {
        transform.position = _tilemap.CellToWorld(tilePos);
    }
}
