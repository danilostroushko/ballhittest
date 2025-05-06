using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 WithX(this Vector3 vec, float x)
    {
        vec.x = x;
        return vec;
    }

    public static Vector3 WithY(this Vector3 vec, float y)
    {
        vec.y = y;
        return vec;
    }

    public static Vector3 WithZ(this Vector3 vec, float z)
    {
        vec.z = z;
        return vec;
    }

    public static Vector3 AddY(this Vector3 vec, float y)
    {
        vec.y += y;
        return vec;
    }

    public static Vector3 RandomPointInBounds(Bounds bounds, Vector3 offset = default)
    {
        return offset + new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}