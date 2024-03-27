using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Helpers
{

    static Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(100);

    static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
    static WaitForSeconds _shortDelay = new WaitForSeconds(0.2f);
    static WaitForSeconds _oneSecond = new WaitForSeconds(1f);

    public static void PrintList(List<float> nums)
    {
        for(int i = 0; i < nums.Count; i++)
        {
            Debug.Log($"item {i} in list: {nums[i]}.");
        }
    }
    public static bool IsFacing(GameObject _unit, Vector3 position)
    {
        float facingThreshhold = 5; //within 5 degrees returns true
        //BUG & CHANGE: ALLOW FOR HANDLE TO BE DIFFERENT HEIGHTS:
        //WILL STILL BEND DOWN... AVOID THIS OR CREATE NEW ABILITY TO PICK UP AT HEIGHT
        //OR GENERALIZE PICKUP TO ALLOW DIFFERENT HEIGHTS
        //WEAPON LOCATION DICTATES PRESET HAND POSITION
        //OR WEAPON LOCATION DICTATES HAND POSITION ON SPHERE AROUND SHOULDER
        if (Mathf.Abs(position.y - _unit.transform.position.y) < 0.5) position.y = _unit.transform.position.y;
        Vector3 displacementToTarget = position - _unit.transform.position;
        float signedAngle = Vector3.SignedAngle(_unit.transform.forward, displacementToTarget, Vector3.up);
        //Debug.Log("abs signedAngle needs to be under 5. Is: " + signedAngle);
        //Debug.DrawLine(_unit.transform.position, position, Color.green, 0.2f);
        //Debug.DrawRay(_unit.transform.position, _unit.transform.forward, Color.red, 0.2f);
        if (Mathf.Abs(signedAngle) < facingThreshhold) return true;
            else return false;
    }
    public static WaitForSeconds OneSecond
    {
        get { return _oneSecond; }
    }
    public static WaitForEndOfFrame EndOfFrame
    {
        get { return _endOfFrame; }
    }
    public static WaitForSeconds ShortDelay
    {
        get { return _shortDelay; }
    }

    static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
    public static WaitForFixedUpdate FixedUpdate
    {
        get { return _fixedUpdate; }
    }

    public static WaitForSeconds Get(float seconds) //caches whatever you do.. eg. call Helpers.WaitForSeconds ??? 
    {
        if (!_timeInterval.ContainsKey(seconds))
            _timeInterval.Add(seconds, new WaitForSeconds(seconds));
        return _timeInterval[seconds];
    }
    public static IEnumerator PauseForDuration(float duration) //DOES THIS WORK??
    {
        float pauseFrames = duration / Time.fixedDeltaTime;
        for (int i = 0; i < pauseFrames; i++)
        {
            yield return Helpers.FixedUpdate;
        }
    }

    public static void SetLayerRecursively(GameObject go, int layerNumber)
    {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    public static Vector2 PositionOnCircle(float theta, float radius)
    {
        float xCoord = Mathf.Cos(theta) * radius;
        float yCoord = Mathf.Sin(theta) * radius;
        return new Vector2(xCoord, yCoord);
    }
    public static Vector2 PositionOnCircle(float theta)
    {
        float xCoord = Mathf.Cos(theta);
        float yCoord = Mathf.Sin(theta);
        return new Vector2(xCoord, yCoord);
    }

    public static Vector3 PositionOnCircle(float theta, float radius, Vector3 xDirection, Vector3 yDirection)
    {
        Vector2 positionOn2DCircle = PositionOnCircle(theta, radius);
        Vector3 xComponent = xDirection * positionOn2DCircle.x;
        Vector3 yComponent = yDirection * positionOn2DCircle.y;
        return xComponent + yComponent;
    }

    public static Vector3 PositionOnCircle(float theta, Vector3 axis, Vector3 startingDisplacementFromCentre)
    {
        Vector3 xVector = startingDisplacementFromCentre; //this is (1,0) on the unit circle
        Vector3 yVector = Vector3.Cross(xVector, axis); //this is (0,1) on the unit circle
        Vector3 xComponent = xVector * Mathf.Cos(theta);
        Vector3 yComponent = yVector * Mathf.Sin(theta);
        return xComponent + yComponent;
    }

    public static Vector3 FlatForward(Vector3 forward) => new Vector3(forward.x, 0, forward.z);
    

}