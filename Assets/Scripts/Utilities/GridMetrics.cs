using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridMetrics
{
    public static Vector3 Displacement(Vector2Int destination, Vector2Int origin) => new Vector3(destination.x - origin.x, 0, destination.y - origin.y);

    public static Vector3 Position(Vector2Int coords) => new Vector3(coords.x, 0f, coords.y);
    public static Quaternion RotationFromHeading(Vector3 heading) => Quaternion.LookRotation(heading, Vector3.up);
}
