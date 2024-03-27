using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopManager : MonoBehaviour
{
    public static TopManager Instance { get; private set; }

    public UnitSpawner unitSpawner;
    public GridSpawner gridSpawner;

    public GridManager gridManager;
    public UnitManager unitManager;
    public TickManager tickManager;
    public MaterialManager materialManager;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        gridSpawner.SpawnGrid();
        unitSpawner.SpawnUnits();
        tickManager.BeginEncounter();
    }

}
