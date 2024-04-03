using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    int tick = 0;
    public void BeginEncounter()
    {
        StartCoroutine(Encounter());
    }

    IEnumerator Encounter()
    {
        Debug.Log("Encounter");
        while (!EncounterIsResolved())
        {
            Debug.Log($"Processing Tick {tick}");
            tick++;
            yield return ProcessTick();
        }
    }

    IEnumerator ProcessTick()
    {
        for (int i = 0; i < TopManager.Instance.unitSpawner.id; i++)
        {
            //find the unit with the matching id
            var units = TopManager.Instance.unitManager.units;
            Unit unit = units.Find(x => x.id == i);
            if (unit is not null)
            {
                Debug.Log($"Found unit {i}");
                yield return unit.ExecuteTurn();
            }
            else
            {
                Debug.Log($"Did not find unit {i}");
            }
        }
    }

    private bool EncounterIsResolved()
    {
        //only one allegiance remains
        var units = TopManager.Instance.unitManager.units;
        bool red = false;
        bool blue = false;
        foreach(var u in units)
        {
            if (u.allegiance.name == "Red") red = true;
            if (u.allegiance.name == "Blue") blue = true;
        }
        if (red && blue) return false;
        else return true;
    }
}
