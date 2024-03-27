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

    //highlight tiles in move range for 1s
    public void HighlightBestMove()
    {
        BestMove().HighlightTile(TopManager.Instance.materialManager.deepRed);
    }

    public void HighlightMoveRange()
    {
        var locationsToHighlight = LocationsInFrontalCone(TilesInMoveRange(), unit.stats.moveConeWidth);
        HighlightTiles(locationsToHighlight, TopManager.Instance.materialManager.litGreen);
    }

    public void HighlightAttackables()
    {
        List<Location> potentialMoves = LocationsInFrontalCone(TilesInMoveRange(), unit.stats.moveConeWidth);
        List<Location> attackables = AttackableMoves(potentialMoves);
        HighlightTiles(attackables, TopManager.Instance.materialManager.highlightRed);
    }
    Location BestMove()
    {
        var moveTiles = LocationsInFrontalCone(TilesInMoveRange(), unit.stats.moveConeWidth);
        var attackTiles = AttackableMoves(moveTiles);
        if (attackTiles.Count != 0) return BestAttackTile(attackTiles);
        return BestMoveTile(moveTiles);
    }

    Location BestAttackTile(List<Location> attackTiles)
    {
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
            Vector3 directionOfTravel = GridMetrics.Displacement(tile.coords, unit.currentLocation.coords);
            if (dist >= minDist || !TargetInFrontConeFrom(closest, tile.coords, directionOfTravel)) continue;
            bestTile = tile;
            minDist = dist;
        }
        return bestTile;
    }
    float TargetDeflectionFromCentre(Unit target, Vector2Int coords, Vector3 facing) => Vector3.Angle(facing, GridMetrics.Displacement(target.currentLocation.coords, coords));
    bool TargetInFrontConeFrom(Unit target, Vector2Int coords, Vector3 facing) => TargetDeflectionFromCentre(target, coords, facing) < unit.stats.attackConeWidth;
    bool CanAttack() => AttackablesFrom(unit.currentLocation.coords, unit.currentHeading).Count != 0;
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
        if (coords == unit.currentLocation.coords) return false;
        Vector3 displacement = new Vector3(coords.x - unit.currentLocation.coords.x, 0f, coords.y - unit.currentLocation.coords.y);
        float deltaTheta = Vector3.Angle(displacement, unit.currentHeading);
        if (deltaTheta <= coneWidth / 2) return true;
        else return false;
    }

    List<Location> LocationsInFrontalCone(List<Location> tilesInRange, float coneWidth)
    {
        var locations = new List<Location>();
        foreach(var l in tilesInRange)
        {
            if(TileIsWithinCone(l.coords, coneWidth)) locations.Add(l);
        }
        return locations;
    }

    void HighlightTiles(List<Location> locations, Material material)
    {
        foreach(var l in locations)
        {
            l.HighlightTile(material);
        }
    }
    List<Location> TilesInMoveRange()
    {
        var locations = new List<Location>();
        foreach(var l in TopManager.Instance.gridManager.locations.Values)
        {
            if (unit.currentLocation.DistanceTo(l) > unit.stats.moveRange) continue;
            locations.Add(l);
        }
        return locations;
    }

    Unit ClosestEnemy()
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


    //ALL BELOW METHODS SHOULD BE CHANGED TO USE V2Int Coords


    //public bool CanAttack(out Unit target)
    //{
    //    float minDist = float.MaxValue;
    //    foreach(var u in TopManager.Instance.unitManager.units)
    //    {
    //        if (!TargetInRange(u)) continue;
    //        if (!TargetInFrontCone(u)) continue;
    //        float dist = TargetDistance(u);
    //        if (dist > minDist) continue;
    //        minDist = dist;
    //        target = u;
    //    }
    //    if(target is null)
    //}

    float TargetDistance(Unit u) => Vector3.Distance(unit.transform.position, u.transform.position);
    bool TargetInRange(Unit u) => TargetDistance(u) <= unit.stats.attackRange;


}
