using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol_v2 : MonoBehaviour
{
    private bool wheelIsMoving;    
    private float movementIncrementValue;

    // Start is called before the first frame update
    public void Load()
    {
        wheelIsMoving = false;
   
    }

    public void StartMoving()
    {
        wheelIsMoving = true;
    }

    public void SetMovementIncrementValue(float newVal)
    {
        movementIncrementValue = newVal;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EnsureMoveSymbol(movementIncrementValue);        
    }

    private void EnsureMoveSymbol(float incrementVal)
    {
        if (wheelIsMoving)
        {
            Vector3 newPos = transform.position;
            newPos.y -= incrementVal;
            transform.position = newPos;
        }
    }


}