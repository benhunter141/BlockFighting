using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBars
{
    public Transform healthBar, staminaBar;
    public StatusBars(Transform _healthBar, Transform _staminaBar)
    {
        healthBar = _healthBar;
        staminaBar = _staminaBar;
    }
    public void UpdateHealth(float health) //health is % health
    {
        if (health <= 0) return;
        var t = healthBar.localScale;
        t.y = health;
        healthBar.localScale = t;
    }

    public void UpdateStamina(float stamina)
    {
        var s = staminaBar.localScale;
        s.y = stamina;
        staminaBar.localScale = s;
    }
}
