using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Written by Jehoshua Josue; 05/02/23
 * This script should be attached on an object that needs to move from its original position
 * onto a destination position.
 * 
 * The variables to tweak (to vary the movement) will be in MoveObjectWithEaseOut().
 * This was written so that the object's velocity mimics that of a Gaussian function (looks like a hill when graphed).
 * The object will ramp up to a max speed and then gradually slow down, eventually stopping once it reaches its destination
 */
public class MovementWithEaseOut : MonoBehaviour
{
    private Vector3 destinationPos; //drag an object on the Editor to this variable for the destination position

    private float startTime;
    private float duration;
    private int direction; // could be 1 if moving in the positive direction, or -1 if opposite direction
    private bool isMoving;
    private float totalDistanceToCover;
    private float MAX_SPEED = 5f;


    private GameObject reelStripParent; 
    private GameObject targetSymbolToStopOn;       

    // Start is called before the first frame update
    public void Load(Vector3 _destinationPos, GameObject _reelStripParent, GameObject targetSymbol)
    {
        isMoving = false;        
        destinationPos = _destinationPos;
        reelStripParent = _reelStripParent;        
        SetInitialConditions(targetSymbol);
    }

    public void StartMoving()
    {
        startTime = Time.time;
        isMoving = true;
        direction = DetermineDirection(transform.position, destinationPos);

    }

    private void SetInitialConditions(GameObject targetSymbol)
    {        
        targetSymbolToStopOn = targetSymbol;
        duration = 5.2f;
        totalDistanceToCover = GetDistanceToStoppingPoint();
    }

    private int DetermineDirection(Vector3 startPos, Vector3 destinationPos)
    {
        int direction = 1;
        float diff = Vector3.Distance(startPos, destinationPos);
        if (diff < 0)
        {
            direction = -1;
        }
        return direction;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving)
        {
            EnsureMoveObject(destinationPos, targetSymbolToStopOn);
        }
    }

    private float GetDistanceToStoppingPoint()
    {
        return Vector3.Distance(targetSymbolToStopOn.transform.position, destinationPos);
    }

    private void EnsureMoveObject(Vector3 targetPos, GameObject targetSymbol)
    {
        float distance = GetDistanceToStoppingPoint();
        Debug.Log("-- Distance of target symbol from destination: " + distance);
        float acceptableDegreeOfError = 1.01f;
        bool objectReachedDestination = distance <= acceptableDegreeOfError;

        Vector3 nextIncrementPos = targetSymbolToStopOn.transform.position;
        nextIncrementPos.y -= GetYIncrementWithEaseOut();
        float nextIncrementDistance = Vector3.Distance(nextIncrementPos, destinationPos);
        bool nextIncrementWillMakeObjectGoPastStoppingPoint = nextIncrementDistance > distance;
        if (objectReachedDestination || nextIncrementWillMakeObjectGoPastStoppingPoint)
        {
            //stop object
            isMoving = false;            
        }
        else
        {
            MoveObjectWithEaseOut();
        }

    }

    private float GetHybridYIncrement()
    {
        float yInc = RampUpFunction();
        if (GetProgress() >= 0.75f)
        {
            float tmp = 1 - GetProgress();
            yInc = tmp * MAX_SPEED;
        }
        yInc = Mathf.Clamp(yInc, 0.01f, MAX_SPEED);
        return yInc;
    }

    private float RampUpFunction()
    {
        float t = Time.time - startTime;
        float tmp = GetProgress();
        float e = (float)System.Math.E;

        float yInc = (float)System.Math.Log(t * 2);
        yInc = Mathf.Clamp(yInc, 0.1f, MAX_SPEED);
        return yInc;
    }

    private float GetProgress()
    {
        return (totalDistanceToCover - GetDistanceToStoppingPoint()) / totalDistanceToCover;
    }

    private float GetYIncrementWithEaseOut()
    {        
        float t = Time.time - startTime;
        float peakT = 0.1f; // time t wherein the y-increment will reach its peak value
        float heightOfGaussianCurve = MAX_SPEED;
        float top = Mathf.Pow((t - peakT), 2);
        float widthOfGaussianCurve = duration * 1f; // how wide is the "hill" curve going to be over time
        float bottom = Mathf.Pow(widthOfGaussianCurve, 2) * 2;
        float e = (float)System.Math.E;
        float yIncrement = Mathf.Pow(e, (top / bottom) * -1) * heightOfGaussianCurve;

        // ensure that the yIncrement doesn't drop below 0.1; otherwise it will take forever to reach its destination
        yIncrement = Mathf.Clamp(yIncrement, 0.1f, MAX_SPEED);


        if (GetDistanceToStoppingPoint() < 1.5f)
        {
            yIncrement = 0.05f;
        }
        return yIncrement;
    }

    /*
     * See Gaussian function for a visual graph of
     *  how the yIncrement value follows the shape of a hill
     *  as time progresses (time is the horizontal x-axis and y-increment is the y-axis)
     */
    private void MoveObjectWithEaseOut()
    {
        float yIncrement = GetHybridYIncrement();
        Vector3 newPos = transform.position;
        newPos.y -= yIncrement;
        Debug.Log("ypos: " + transform.position.y + ".. yIncrement: " + yIncrement);
        reelStripParent.transform.position = newPos;
    }
}
