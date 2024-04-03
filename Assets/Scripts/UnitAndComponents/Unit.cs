using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Allegiance allegiance;
    
    //Components
    public Movement movement;
    Targeting targeting;
    Pathfinding pathfinding;
    public HealthSystem healthSystem;
    public AttackSystem attackSystem;
    public StatusBars statusBars;
    public Transform healthBar, staminaBar;
    public UnitStats stats;

    //currentStats
    public Location currentLocation;
    public Vector3 currentHeading;
    public int id;

    private void Awake()
    {
        movement = new Movement(this);
        targeting = new Targeting(this);
        pathfinding = new Pathfinding(this);
        statusBars = new StatusBars(healthBar, staminaBar);
        attackSystem = new AttackSystem(this);
        healthSystem = new HealthSystem(stats.maxHealth, this);
    }

    private void Start()
    {
        currentHeading = allegiance.forward;
    }


    public IEnumerator ExecuteTurn() //logic of what to do should be here, not within components. Use component methods, but show logic here
    {
        yield return null;

        //cache moverange
        //cache attackables
        
        List<Location> moveTiles = targeting.MoveRange();
        if (moveTiles.Count == 0) throw new Exception("moveTiles.Count is zero wtf...");
        List<Location> attackables = targeting.Attackables(moveTiles);
        Location bestMove = targeting.BestMove(moveTiles, attackables);
        //Debug.Log($"move/atkbles counts: {moveTiles.Count}, {attackables.Count}");

        TopManager.Instance.gridManager.Highlight(moveTiles, TopManager.Instance.materialManager.litGreen);
        yield return new WaitForSeconds(0.05f);

        TopManager.Instance.gridManager.Highlight(attackables, TopManager.Instance.materialManager.fadedYellow);
        yield return new WaitForSeconds(0.05f);

        TopManager.Instance.gridManager.Highlight(bestMove, TopManager.Instance.materialManager.highlightRed);
        //Debug.Log($"Best move is {bestMove.coords.x},{bestMove.coords.y}", gameObject);
        yield return new WaitForSeconds(0.1f);

        if (bestMove == currentLocation && targeting.CanAttack()) //ATTACK!!
        {
            //Debug.Log($"can atk, hop turn face", gameObject);
            yield return movement.HopTurnFace(targeting.BestCurrentAttackTarget().currentLocation.coords); //replace w atk later
            yield return attackSystem.Attack(targeting.BestCurrentAttackTarget());
        }
        else if (bestMove == currentLocation && targeting.CanTurnToAttack(out Unit target)) //turn if it leads to atk opportunity
        {
            //Debug.Log($"can turn & get enemy in range and in cone", gameObject);
            yield return movement.HopTurnFace(target.currentLocation.coords);
        }
        else if (bestMove == currentLocation && targeting.CanTurnAndFace(targeting.ClosestEnemy())) //can turn and face closest (though OOR)
        {
            //1. turn & face closest or
            target = targeting.ClosestEnemy();
            //Debug.Log($"can turn in place and get closest in cone", gameObject);
            yield return movement.HopTurnFace(target.currentLocation.coords);
        }
        else if (bestMove == currentLocation) //can't turn and face, maximally turn and attempt facing but don't make it all the way
        {
            //2. full turn towards closest
            //Debug.Log($"can turn in place, but can't get closest in cone", gameObject);
            target = targeting.ClosestEnemy();
            //is target right or left?
            float signedAngle =
                Vector3.SignedAngle(currentHeading, GridMetrics.Displacement(target.currentLocation.coords, currentLocation.coords), Vector3.up);
            //Debug.Log($"signed angle: {signedAngle}");
            yield return movement.HopTurnDegrees(signedAngle);
        }
        else
        {
            //Debug.Log("Best move is not the current location!");
            yield return movement.MoveTo(bestMove.coords);
        }

        TopManager.Instance.gridManager.UnhighlightAllTiles();
        yield return new WaitForSeconds(0.5f);
        yield return null;
    }
}
