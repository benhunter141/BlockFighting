using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    public void BeginEncounter()
    {
        StartCoroutine(Encounter());
    }

    IEnumerator Encounter()
    {
        Debug.Log("Encounter");
        while (!EncounterIsResolved())
        {
            Debug.Log("Processing Tick");
            yield return ProcessTick();
        }
    }

    IEnumerator ProcessTick()
    {
        foreach(var u in TopManager.Instance.unitManager.units)
        {
            yield return u.ExecuteTurn();
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
