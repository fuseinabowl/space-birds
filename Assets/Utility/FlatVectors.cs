using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FlatVectors
{
    public static Vector2 ToGamePosition(this Vector3 original)
    {
        return new Vector2(original.x, original.z);
    }

    public static Vector3 ToWorldPosition(this Vector2 original)
    {
        return new Vector3(original.x, 0f, original.y);
    }
}
