using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public GameObject tilePrefab;
    public int gridSize;

    public void SpawnGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Vector3 position = new Vector3(i, -0.5f, j);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                Vector2Int coords = new Vector2Int(i, j);
                Location location = new Location(tile, tile.GetComponent<Renderer>(), coords);
                if (TopManager.Instance.gridManager is null) Debug.Log("Null GM");
                if (TopManager.Instance.gridManager.locations is null) Debug.Log("Null locations");
                TopManager.Instance.gridManager.locations.Add(coords, location);
            }
        }
    }
}
