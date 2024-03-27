using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location
{
    //terrain (implement later)
    //Unit or null
    Unit occupyingUnit;
    GameObject tile;
    Renderer renderer;
    public Vector2Int coords;

    public Location(GameObject _tile, Renderer _rndr, Vector2Int _coords)
    {
        tile = _tile;
        coords = _coords;
        renderer = _rndr;
    }

    public float DistanceTo(Location l) => Vector2Int.Distance(coords, l.coords);
    public void HighlightTile()
    {
        renderer.material = TopManager.Instance.materialManager.fadedYellow;
    }
    public void HighlightTile(Material material)
    {
        renderer.material = material;
    }
    public void UnHighlightTile()
    {
        renderer.material = TopManager.Instance.materialManager.fadedGreen;
    }
}
