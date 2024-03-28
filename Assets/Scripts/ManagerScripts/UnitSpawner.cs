using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    //Spawn a few units on opposing teams, pass refs to unit manager
    public GameObject fighter, spearman;
    public Allegiance red, blue;

    public void SpawnUnits()
    {
        SpawnUnit(fighter, 2, 2, blue);
        SpawnUnit(fighter, 1, 0, blue);
        SpawnUnit(fighter, 2, 0, blue);
        SpawnUnit(fighter, 8, 6, red);
        SpawnUnit(fighter, 5, 9, red);
    }

    void SpawnUnit(GameObject unitPrefab, int x, int z, Allegiance allegiance)
    {
        GameObject obj = Instantiate(unitPrefab, new Vector3(x, 0f, z), allegiance.Rotation());
        GameObject body = obj.transform.GetChild(0).gameObject;
        body.GetComponent<Renderer>().material = allegiance.material;
        Unit unit = obj.GetComponent<Unit>();
        unit.allegiance = allegiance;
        Vector2Int coords = new Vector2Int(x, z);
        unit.currentLocation = TopManager.Instance.gridManager.locations[coords];
        TopManager.Instance.unitManager.units.Add(unit);

    }
}
