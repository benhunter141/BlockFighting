using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaSystem
{
    Unit unit;
    public int ClockTicks { get; private set; }

    public StaminaSystem(Unit u)
    {
        unit = u;
        ClockTicks = 0;
    }

    public void ReduceClockTicks(int cT)
    {
        ClockTicks -= cT;
    }

    public void ProcessTick()
    {
        ClockTicks += unit.stats.stamina;
    }
}
