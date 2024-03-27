using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridMetrics
{
    public static Vector3 Displacement(Vector2Int destination, Vector2Int origin) => new Vector3(destination.x - origin.x, 0, destination.y - origin.y);
}
