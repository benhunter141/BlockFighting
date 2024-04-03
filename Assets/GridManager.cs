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
            val.UnHighlight();
        }
    }
    //should have all the highlight stuff here
    public void Highlight(List<Location> tiles, Material material)
    {
        foreach(var t in tiles)
        {
            t.Highlight(material);
        }
    }

    public void Highlight(Location tile, Material material)
    {
        tile.Highlight(material);
    }
}
