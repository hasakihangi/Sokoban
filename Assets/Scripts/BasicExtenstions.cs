using System;
using UnityEngine;

public static class BasicExtensions
{
    public static Vector2 ToVector2(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Vector3 ToVector3(this Vector2 vector, float height = 0f)
    {
        return new Vector3(vector.x, height, vector.y);
    }
}