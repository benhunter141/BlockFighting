using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Allegiance allegiance;
    
    //Components
    Movement movement;
    Targeting targeting;
    Pathfinding pathfinding;

    public UnitStats stats;

    //currentStats
    public Location currentLocation;
    public Vector3 currentHeading;

    private void Awake()
    {
        movement = new Movement(this);
        targeting = new Targeting(this);
        pathfinding = new Pathfinding(this);
    }

    private void Start()
    {
        currentHeading = allegiance.forward;
    }


    public IEnumerator ExecuteTurn()
    {
        yield return null;
        targeting.HighlightMoveRange();
        yield return new WaitForSeconds(1f);
        targeting.HighlightAttackables();
        ////targeting.HighlightBestMove();
        yield return new WaitForSeconds(1f);
        TopManager.Instance.gridManager.UnhighlightAllTiles();
        yield return new WaitForSeconds(0.5f);
        yield return null;
    }

    //Clean up below, organize into components

    //IEnumerator MoveToClosestEnemy()
    //{
    //    yield return MoveToDestination(ClosestEnemy().transform.position);
    //}

    private IEnumerator MoveToDestination(Vector3 destination)
    {
        //parabolic movement
        int frames = 60;
        Vector3 start = transform.position;
        Vector3 direction = (destination - start).normalized;
        float span = (destination - start).magnitude;
        float dZ = span / (float)frames;
        Debug.Log($"Span:{span}, dZ:{dZ}");
        for(int i = 0; i < frames; i++)
        {
            //y = -(z - Apex.z)^2 + height
            float z = dZ * (float)i;                               //z can be any horizontal direction
            float y = 0.266f - (z - span / 2) * (z - span / 2); //y is always up

            Vector3 position = start + Vector3.up * y + direction * z;
            Debug.Log($"z:{z}, y:{y}, start:{start.x},{start.y},{start.z}, currentPos:{position.x},{position.y},{position.z}");

            transform.position = position;
            yield return null;
        }
        transform.position = destination;
    }

    private void Attack()
    {
        throw new NotImplementedException();
    }

    private bool CanAttack()
    {
        throw new NotImplementedException();
    }
}
