using UnityEngine;
using UnityEngine.Tilemaps;

// ------------------------------------------------------------------
// Utility functions usable in any Unity project
// ------------------------------------------------------------------

public static class UnityUtilities
{
    // Returns true if given layer is in layerMask, false if not
    public static bool LayerInMask(LayerMask layerMask, int layer)
    {
        return (layerMask == (layerMask | (1 << layer)));
    }

    // Returns whether difference between two given floats is <= given epsilon
    public static bool WithinEpsilon(float a, float b, float epsilon)
    {
        float diff = Mathf.Abs(a - b);
        return diff <= epsilon;
    }

    // Returns whether the given bool is the zero vector
    public static bool IsZeroVector(Vector3 vector)
    {
        return Mathf.Approximately(vector.magnitude, 0f);
    }

    // Returns angle in degrees around z axis of given vector; angle
    // will be in range [-180, 180]
    public static float AngleFromVector(Vector3 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }

    // Returns unit vector rotated by given angle around z axis
    public static Vector3 VectorFromAngle(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    // Returns Quaternion representing a rotation around z axis to
    // given direction vector
    // Set transform.rotation to returned Quaternion to rotate a
    // GameObject
    public static Quaternion VectorToRotation(Vector3 directionVector)
    {
        float angle = UnityUtilities.AngleFromVector(directionVector);
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Ensures that given angle (in degrees) is positive
    public static float EnsurePositiveAngleDeg(float angle)
    {
        bool anglePositive = (angle >= 0f);
        if (anglePositive)
            return angle;

        return angle + 360;
    }

    // Ensures that given angle (in radians) is positive
    public static float EnsurePositiveAngleRad(float angle)
    {
        bool anglePositive = (angle >= 0f);
        if (anglePositive)
            return angle;

        return angle + 2 * Mathf.PI;
    }

    // Returns world position of mouse cursor
    public static Vector3 MouseWorldPosition(Camera camera)
    {
        Vector3 mousePosScreen = Input.mousePosition;
        // Ensure that Z coordinate of mousePosScreen positive so movement
        // is detected in 3D
        mousePosScreen.z = camera.nearClipPlane;
        Debug.Log("Mouse position on screen: " + mousePosScreen.ToString());
        Vector3 pos = camera.ScreenToWorldPoint(Input.mousePosition);

        // Need to get rid of camera position's affect on pos
        return pos - camera.transform.position;
    }

    // Returns true if a tile exists in given tilemap at given position
    public static bool TileExists(Tilemap tilemap, 
        Vector3Int tilePosition)
    {
        return tilemap.GetTile(tilePosition) != null;
    }

    // Return child GameObject of given parent GameObject with given
    // name; returns null if not found
    public static GameObject FindChildGameObject(GameObject parent, 
        string childName)
    {
        Transform childTransform = parent.transform.Find(childName);
        if (!childTransform)
            return null;

        return childTransform.gameObject;
    }

    // Returns parent GameObject of given child GameObject; returns null
    // if there is no parent
    public static GameObject FindParentGameObject(GameObject child)
    {
        return child.transform.parent.gameObject;
    }

    // Returns a unit vector facing to the right relative to the given transform's rotation
    public static Vector3 RightVectorRelativeToRotation(Transform transform)
    {
        return transform.rotation * Vector3.right;
    }
}