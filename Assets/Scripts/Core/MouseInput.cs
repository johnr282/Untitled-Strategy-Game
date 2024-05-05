using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Component handling selecting and hovering over objects with the mouse
// ------------------------------------------------------------------

public class MouseInput : MonoBehaviour
{
    void Update()
    {
        if (SelectionDetected())
            TrySelection();   
        else
            TryHover();
    }

    // Returns whether the player attempted to select something
    bool SelectionDetected()
    {
        int leftMouseButton = 0;
        return Input.GetMouseButtonDown(leftMouseButton);
    }

    // Attempts to select an object at the current mouse position; returns true
    // if successful, false if no selectable object was found
    bool TrySelection()
    {
        if (MouseRaycast(out GameObject collidedObject))
        {
            if (collidedObject.TryGetComponent(out SelectableObject selectedObject))
            {
                selectedObject.OnSelect();
                return true;
            }
        }

        return false;
    }

    // Attempts to hover over an object at the current mouse position; returns true
    // if successful, false if no hoverable object was found
    bool TryHover()
    {
        if (MouseRaycast(out GameObject collidedObject))
        {
            if (collidedObject.TryGetComponent(out HoverableObject hoveredObject))
            {
                hoveredObject.OnHover();
                return true;
            }
        }

        return false;
    }

    // Performs a raycast from current mouse position; returns true if cast hits
    // a valid collider and puts the first object the raycast collides with into
    // the output parameter collidedObject, returns false if cast doesn't hit
    // a valid collider
    bool MouseRaycast(out GameObject collidedObject)
    {
        collidedObject = null;
        Vector3 mousePosScreen = Input.mousePosition;
        Ray rayTowardsMousePos = Camera.main.ScreenPointToRay(mousePosScreen);

        if (!Physics.Raycast(rayTowardsMousePos, out RaycastHit hit) ||
            hit.collider == null)
        {
            return false;
        }

        collidedObject = hit.collider.gameObject;
        return true;
    }
}
