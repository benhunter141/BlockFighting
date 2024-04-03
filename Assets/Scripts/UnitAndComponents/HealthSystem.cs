using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem
{
    Unit unit;
    public int maxHealth, currentHealth;
    public HealthSystem(int _maxHealth, Unit _unit)
    {
        maxHealth = _maxHealth;
        currentHealth = maxHealth;
        unit = _unit;
    }

    public IEnumerator GetHit(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            yield return Die();
        }
            float percentHealth = (float)currentHealth / maxHealth;
            Debug.Log($"Unit now at {percentHealth} percent health", unit);
            unit.statusBars.UpdateHealth(percentHealth);
    }

    IEnumerator Die()
    {
        TopManager.Instance.unitManager.units.Remove(unit);
        unit.currentLocation.occupyingUnit = null;
        unit.statusBars.UpdateStamina(0.01f);

        int frames = 15;
        float depth = -2f;
        float delta = depth / frames;
        for (int i = 0; i < frames; i++)
        {
            Vector3 pos = unit.transform.position;
            pos.y = i * delta;
            unit.transform.position = pos;
            yield return null;
        }
        yield return null;
    }
}
