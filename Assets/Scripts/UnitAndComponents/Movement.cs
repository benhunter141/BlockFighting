using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    Unit unit;
    public Movement(Unit _unit)
    {
        unit = _unit;
    }
    IEnumerator MoveAt(Vector2Int coords)
    {
        yield return null;
    }
}
