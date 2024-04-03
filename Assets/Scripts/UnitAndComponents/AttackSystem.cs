using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem
{
    public Unit unit;
    public AttackSystem(Unit u)
    {
        unit = u;
    }

    public IEnumerator Attack(Unit target)
    {
        yield return target.healthSystem.GetHit(unit.stats.damage);
        Debug.Log($"attacking for {unit.stats.damage} damage", unit);
        Debug.Log($"got hit, current health: {target.healthSystem.currentHealth}", target);

        yield return null;
    }
}
