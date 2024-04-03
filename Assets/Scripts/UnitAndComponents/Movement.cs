using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    Unit unit;
    int moveFrames = 40;
    int hopFrames = 20;
    public Movement(Unit _unit)
    {
        unit = _unit;
    }
    IEnumerator MoveAt(Vector2Int coords)
    {
        yield return null;
    }

    public IEnumerator MoveTo(Vector2Int coords)
    {
        //parabolic movement
        int frames = moveFrames;
        Vector3 start = GridMetrics.Position(unit.currentLocation.coords);
        Vector3 destination = GridMetrics.Position(coords);
        Vector3 displacement = GridMetrics.Displacement(coords, unit.currentLocation.coords);
        Vector3 direction = displacement.normalized;
        Quaternion startRot = GridMetrics.RotationFromHeading(unit.currentHeading);
        Quaternion endRot = GridMetrics.RotationFromHeading(direction);

        float span = displacement.magnitude;
        float dZ = span / (float)frames;

        for (int i = 0; i < frames; i++)
        {
            //y = -(z - Apex.z)^2 + height
            float z = dZ * (float)i; //z can be any horizontal direction
            float maxHeight = span * span / 4;
            float y = maxHeight - (z - span / 2) * (z - span / 2);    //y is always up

            Vector3 position = start + Vector3.up * y + direction * z;
            Quaternion rot = Quaternion.Lerp(startRot, endRot, (float)i / (float)frames);
            //Debug.Log($"z:{z}, y:{y}, start:{start.x},{start.y},{start.z}, currentPos:{position.x},{position.y},{position.z}");

            unit.transform.position = position;
            unit.transform.rotation = rot;
            yield return null;
        }
        unit.transform.position = destination;
        unit.transform.rotation = endRot;
        UpdateLocationTo(coords);
        UpdateHeadingTo(direction);
    }
    public IEnumerator HopTurnDegrees(float degrees)
    {
        int frames = hopFrames;
        float span = 1f;
        float dZ = span / (float)frames;
        Vector3 currentPos = unit.transform.position;
        //use degrees to get goal direction:
        Vector3 direction = Quaternion.AngleAxis(unit.stats.moveConeWidth, -Vector3.up) * unit.currentHeading;
        Quaternion startRot = GridMetrics.RotationFromHeading(unit.currentHeading);
        Quaternion endRot = GridMetrics.RotationFromHeading(direction);
        for (int i = 0; i < frames; i++)
        {
            float z = dZ * frames;
            float maxHeight = span * span / 4;
            float y = maxHeight - (z - span / 2) * (z - span / 2);
            Quaternion rot = Quaternion.Lerp(startRot, endRot, (float)i / (float)frames);
            unit.transform.position = new Vector3(currentPos.x, y, currentPos.z);
            unit.transform.rotation = rot;
            yield return null;
        }
        unit.transform.rotation = endRot;
        unit.transform.position = currentPos;
        UpdateHeadingTo(direction);
    }
    public IEnumerator HopTurnFace(Vector2Int coords) //rotation
    {
        int frames = hopFrames;
        float span = 1f;
        float dZ = span / (float)frames;
        Vector3 currentPos = unit.transform.position;
        Vector3 displacement = GridMetrics.Displacement(coords, unit.currentLocation.coords);
        Quaternion startRot = GridMetrics.RotationFromHeading(unit.currentHeading);
        Quaternion endRot = GridMetrics.RotationFromHeading(displacement);
        for (int i = 0; i < frames; i++)
        {
            float z = dZ * frames;
            float maxHeight = span * span / 4;
            float y = maxHeight - (z - span / 2) * (z - span / 2);
            Quaternion rot = Quaternion.Lerp(startRot, endRot, (float)i / (float)frames);
            unit.transform.position = new Vector3(currentPos.x, y, currentPos.z);
            unit.transform.rotation = rot;
            yield return null;
        }
        unit.transform.rotation = endRot;
        unit.transform.position = currentPos;
        UpdateHeadingTo(displacement.normalized);
    }

    void UpdateLocationTo(Vector2Int coords)
    {
        //old location's occupying unit = null
        unit.currentLocation.UpdateOccupyingUnitTo(null);
        //unit's location = new location
        unit.currentLocation = TopManager.Instance.gridManager.locations[coords];
        //new location's occupying unit
        unit.currentLocation.UpdateOccupyingUnitTo(unit);
    }

    void UpdateHeadingTo(Vector3 direction)
    {
        unit.currentHeading = direction;
    }

}
