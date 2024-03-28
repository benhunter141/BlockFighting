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


    public IEnumerator ExecuteTurn() //logic of what to do should be here, not within components. Use component methods, but show logic here
    {
        yield return null;
        targeting.HighlightMoveRange();
        yield return new WaitForSeconds(1f);

        targeting.HighlightAttackables();
        yield return new WaitForSeconds(1f);

        targeting.HighlightBestMove();
        yield return new WaitForSeconds(1f);

        Location bestMove = targeting.BestMove();

        if(bestMove == currentLocation && targeting.CanAttack()) //atk if you can
        {
            Debug.Log($"can atk, hop turn face");
            yield return movement.HopTurnFace(targeting.BestCurrentAttackTarget().currentLocation.coords); //replace w atk later
        }
        else if (bestMove == currentLocation && targeting.CanTurnToAttack(out Unit target)) //turn if it leads to atk opportunity
        {
            Debug.Log($"can turn & get enemy in range and in cone", target.gameObject);
            yield return movement.HopTurnFace(target.currentLocation.coords);
        }
        else if(bestMove == currentLocation && targeting.CanTurnAndFace(targeting.ClosestEnemy())) //can turn and face closest (though OOR)
        {
            //1. turn & face closest or
            target = targeting.ClosestEnemy();
            Debug.Log($"can turn in place and get closest in cone");
            yield return movement.HopTurnFace(target.currentLocation.coords);
        }
        else if(bestMove == currentLocation) //can't turn and face, maximally turn and attempt facing but don't make it all the way
        {
            //2. full turn towards closest
            Debug.Log($"can turn in place, but can't get closest in cone");
            target = targeting.ClosestEnemy();
            //is target right or left?
            float signedAngle = 
                Vector3.SignedAngle(currentHeading, GridMetrics.Displacement(target.currentLocation.coords, currentLocation.coords), Vector3.up);
            Debug.Log($"signed angle: {signedAngle}");
            yield return movement.HopTurnDegrees(signedAngle);
        }
        else yield return movement.MoveTo(bestMove.coords);

        TopManager.Instance.gridManager.UnhighlightAllTiles();
        yield return new WaitForSeconds(0.5f);
        yield return null;
    }

    //Clean up below, organize into components

    //IEnumerator MoveToClosestEnemy()
    //{
    //    yield return MoveToDestination(ClosestEnemy().transform.position);
    //}



    private void Attack()
    {
        throw new NotImplementedException();
    }

    private bool CanAttack()
    {
        throw new NotImplementedException();
    }
}
