using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Targeting should always use v2Int coords for everything, not the transform position
/// </summary>
public class Targeting
{
    Unit unit;
    public Targeting(Unit _unit)
    {
        unit = _unit;
    }
    public List<Location> MoveRange() => MoveLocationsInFrontalCone(TilesInMoveRange(), unit.stats.moveConeWidth);
    public List<Location> Attackables(List<Location> moveRange) => AttackableMoves(moveRange);
    public bool CanTurnToAttack(out Unit target)
    {
        //can turn and get an enemy in range and in cone
        var enemiesInRange = TargetsInRange();
        target = null;
        float minDelta = 360f;
        foreach(var e in enemiesInRange)
        {
            //this is only called if e is outside of cone
            //check within full cone amount
            var displacement = GridMetrics.Displacement(e.currentLocation.coords, unit.currentLocation.coords);
            float angle = Vector3.Angle(displacement, unit.currentHeading);
            if (angle > unit.stats.attackConeWidth || angle > minDelta) continue;
            minDelta = angle;
            target = e;
        }
        if(target is null) return false;
        return true;
    }
    public Location BestMove(List<Location> moveTiles, List<Location> attackables) //can be current location if, for example, there is an attackable enemy in cone and in range
    {
        //Debug.Log("Best Move() Called");
        if (CanAttack()) return unit.currentLocation;
        if (attackables.Count != 0) return BestAttackTile(attackables);
        return BestMoveTile(moveTiles);
    }
    public List<Unit> TargetsInRange()
    {
        var list = new List<Unit>();
        foreach (var u in TopManager.Instance.unitManager.units)
        {
            if (u.allegiance == unit.allegiance) continue;
            float dist = Vector2Int.Distance(u.currentLocation.coords, unit.currentLocation.coords);
            if (dist < unit.stats.attackRange) list.Add(u);
        }
        return list;
    }
    public bool CanTurnAndFace(Unit target)
    {
        Vector3 displacement = GridMetrics.Displacement(target.currentLocation.coords, unit.currentLocation.coords);
        Vector3 heading = unit.currentHeading;
        float angle = Vector3.Angle(displacement, heading);
        if (angle < unit.stats.attackConeWidth) return true;
        return false;
    }
    Location BestAttackTile(List<Location> attackTiles)
    {
        //Debug.Log("calling best atk tile");
        //LOGIC FOR NOW: best atk tile is shortest distance from current position (conserve cT philosophy)
        if (attackTiles.Count == 0) throw new System.Exception("trying to find best of zero atk tiles");
        float minDist = float.MaxValue;
        Location bestTile = null;
        foreach (var tile in attackTiles)
        {
            float dist = Vector2Int.Distance(tile.coords, unit.currentLocation.coords);
            if (dist >= minDist) continue;
            minDist = dist;
            bestTile = tile;
        }
        return bestTile;
    }
    Location BestMoveTile(List<Location> moveTiles)
    {
        if (moveTiles.Count == 0) throw new System.Exception("move Tile count is zero...");
        //Debug.Log($"calling best move tile");
        //LOGIC FOR NOW: best non-atk move tile is closest to enemy IN-Cone
        //else if no moves have an In-Cone closestEnemy, stay put and turn to face
        float minDist = float.MaxValue;
        Location bestTile = null;
        //Best In-Cone Tile:
        foreach (var tile in moveTiles)
        {
            //minimize distance && check if in-cone
            Unit closest = ClosestEnemyFrom(tile.coords);
            float dist = Vector2Int.Distance(closest.currentLocation.coords, tile.coords);
            //direction of travel won't work if going to same tile
            Vector3 directionOfTravel = GridMetrics.Displacement(tile.coords, unit.currentLocation.coords);
            if (dist >= minDist || !TargetInFrontConeFrom(closest, tile.coords, directionOfTravel)) continue;
            bestTile = tile;
            minDist = dist;
        }
        if(bestTile is null)
        {
            Debug.Log($"No best tile due to obstruction. moveTile count:{moveTiles.Count}", unit.gameObject);
            float random = Random.Range(0f, moveTiles.Count);
            int r = Mathf.RoundToInt(random); //divid by zero...
            r %= moveTiles.Count; //div by zero here..?
            bestTile = moveTiles[r];
        }
        return bestTile;
    }
    float TargetDeflectionFromCentre(Unit target, Vector2Int coords, Vector3 facing) => Vector3.Angle(facing, GridMetrics.Displacement(target.currentLocation.coords, coords));
    bool TargetInFrontConeFrom(Unit target, Vector2Int coords, Vector3 facing) => TargetDeflectionFromCentre(target, coords, facing) < unit.stats.attackConeWidth / 2;
    public bool CanAttack() => AttackablesFrom(unit.currentLocation.coords, unit.currentHeading).Count != 0;
    List<Location> AttackableMoves(List<Location> potentialMoves)
    {
        var list = new List<Location>();
        foreach(var pM in potentialMoves)
        {
            //list only moves where you land within attack range AND target is within resulting attack cone
            Vector3 directionOfTravel = GridMetrics.Displacement(pM.coords, unit.currentLocation.coords);
            var attackables = AttackablesFrom(pM.coords, directionOfTravel);
            if (attackables.Count != 0) list.Add(pM);
        }
        return list;
    }
    bool TileIsWithinCone(Vector2Int coords, float coneWidth)
    {
        if (coords == unit.currentLocation.coords) return true;
        Vector3 displacement = new Vector3(coords.x - unit.currentLocation.coords.x, 0f, coords.y - unit.currentLocation.coords.y);
        float deltaTheta = Vector3.Angle(displacement, unit.currentHeading);
        if (deltaTheta <= coneWidth / 2) return true;
        else return false;
    }
    List<Location> MoveLocationsInFrontalCone(List<Location> tilesInRange, float coneWidth)
    {
        var locations = new List<Location>();
        foreach(var l in tilesInRange)
        {
            if (l.occupyingUnit is not null && l.occupyingUnit != unit) continue;
            if(TileIsWithinCone(l.coords, coneWidth)) locations.Add(l);
        }
        return locations;
    }
    List<Location> TilesInMoveRange()
    {
        var locations = new List<Location>();
        foreach(var l in TopManager.Instance.gridManager.locations.Values)
        {
            if (unit.currentLocation.DistanceTo(l) > unit.stats.moveRange) continue;
            locations.Add(l);
        }
        if (locations.Count == 0) throw new System.Exception("zero tiles in move range");
        return locations;
    }
    public Unit ClosestEnemy()
    {
        float maxDist = float.MaxValue;
        Unit target = unit;
        foreach (var u in TopManager.Instance.unitManager.units)
        {
            if (u == unit) continue;
            if (u.allegiance == unit.allegiance) continue;
            float distance = Vector2Int.Distance(u.currentLocation.coords, unit.currentLocation.coords);
            if (distance < maxDist)
            {
                maxDist = distance;
                target = u;
            }
        }
        if (target == unit) throw new System.Exception("targeting self");
        return target;
    }
    Unit ClosestEnemyFrom(Vector2Int coords)
    {
        float maxDist = float.MaxValue;
        Unit target = unit;
        foreach (var u in TopManager.Instance.unitManager.units)
        {
            if (u == unit) continue;
            if (u.allegiance == unit.allegiance) continue;
            float distance = Vector2Int.Distance(u.currentLocation.coords, coords);
            if (distance < maxDist)
            {
                maxDist = distance;
                target = u;
            }
        }
        if (target == unit) throw new System.Exception("targeting self");
        return target;
    }
    List<Unit> AttackablesFrom(Vector2Int coords, Vector3 facingDirection)
    {
        var list = new List<Unit>();
        foreach(var u in TopManager.Instance.unitManager.units)
        {
            //allegiance check
            if (u.allegiance == unit.allegiance) continue;
            //distance check
            float dist = Vector2Int.Distance(coords, u.currentLocation.coords);
            if (dist > unit.stats.attackRange) continue;
            //cone check
            float deltaTheta = Vector3.Angle(GridMetrics.Displacement(u.currentLocation.coords, coords), unit.currentHeading);
            if (deltaTheta > unit.stats.attackConeWidth / 2) continue;
            list.Add(u);
        }
        return list;
    }
    public Unit BestCurrentAttackTarget()
    {
        var attackables = AttackablesFrom(unit.currentLocation.coords, unit.currentHeading);
        if (attackables.Count == 0) throw new System.Exception("no attackables");
        float minDist = float.MaxValue;
        Unit target = null;
        foreach (Unit u in attackables)
        {
            float dist = Vector2Int.Distance(u.currentLocation.coords, unit.currentLocation.coords);
            if (dist > minDist) continue;
            minDist = dist;
            target = u;
        }
        return target;
    }

}
