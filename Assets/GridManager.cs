using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Dictionary<Vector2Int, Location> locations;

    private void Awake()
    {
        locations = new Dictionary<Vector2Int, Location>();
    }
    public void UnhighlightAllTiles()
    {
        foreach(var val in locations.Values)
        {
            val.UnHighlightTile();
        }
    }
}
