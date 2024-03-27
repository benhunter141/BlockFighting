using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Allegiance", menuName = "ScriptableObjects/Allegiance", order = 1)]
public class Allegiance : ScriptableObject
{
    public Material material;
    public Vector3 euler;
    public Vector3 forward;

    public Quaternion Rotation() => Quaternion.Euler(euler);

}

