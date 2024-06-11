using System;
using System.Collections;
using System.Collections.Generic;
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

    // Returns world position of mouse cursor; only works in 2D
    public static Vector3 MouseWorldPosition(Camera camera)
    {
        Vector3 mousePosScreen = Input.mousePosition;
        // Ensure that Z coordinate of mousePosScreen positive so movement
        // is detected in 3D
        mousePosScreen.z = camera.nearClipPlane;
        Vector3 pos = camera.ScreenToWorldPoint(Input.mousePosition);

        // Need to get rid of camera position's affect on pos
        return pos - camera.transform.position;
    }

    // Returns ray pointing towards the position of the mouse on the screen
    public static Ray RayTowardsMouse()
    {
        Vector3 mousePosScreen = Input.mousePosition;
        return Camera.main.ScreenPointToRay(mousePosScreen);
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

    // Returns first child GameObject of given parent GameObject; assumes
    // parent has at least one child
    public static GameObject GetFirstChildGameObject(GameObject parent)
    {
        Transform childTransform = parent.transform.GetChild(0);
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

    // Converts given index of row-major array to corresponding 2D grid
    // coordinate; needs the number of columns in grid
    public static Vector2Int IndexToCoordinate(int index,
        int columnsInGrid)
    {
        int gridCol = index % columnsInGrid;
        int gridRow = index / columnsInGrid;
        return new Vector2Int(gridCol, gridRow);
    }

    // Returns a normally distributed double using the given mean and 
    // standard deviation
    // Source: https://stackoverflow.com/a/218600
    public static float NormalDistribution(float mean, 
        float stdDev)
    {
        // u1 and u2 must be greater than 0
        float u1 = UnityEngine.Random.Range(float.Epsilon, 1.0f);
        float u2 = UnityEngine.Random.Range(float.Epsilon, 1.0f);

        // random normal(0,1)
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                     Mathf.Sin(2.0f * Mathf.PI * u2);
        // random normal(mean,stdDev^2)
        return mean + stdDev * randStdNormal; 
    }

    // Returns a normally distributed int using the given mean and
    // standard deviation
    public static int NormalDistributionInt(float mean, 
        float stdDev)
    {
        return (int)Mathf.Round(NormalDistribution(mean, stdDev));
    }

    // Returns a list containing the sequence of integers [a, a + n)
    public static List<int> SequentialList(int a, 
        int n)
    {
        List<int> sequentialList = new();
        for (int i = 0; i < n; i++)
        {
            sequentialList.Add(a + i);
        }
        return sequentialList;
    }

    // Returns a random index into the given list
    public static int RandomIndex<T>(List<T> list)
    {
        return UnityEngine.Random.Range(0, list.Count);
    }

    // Returns a random element from the given list
    public static T RandomElement<T>(List<T> list)
    {
        return list[RandomIndex(list)];
    }

    // Converts the given Vector3Int to a Vector3

}