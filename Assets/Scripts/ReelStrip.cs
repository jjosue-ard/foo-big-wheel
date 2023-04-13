using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelStrip : MonoBehaviour
{
    private bool isMoving;    
    private float movementIncrementValue;

    // Start is called before the first frame update
    public void Load()
    {
        isMoving = false;        
    }

    public void StartMoving()
    {
        Debug.Log("ReelStrip.StartMoving()");
        isMoving = true;
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
        if (isMoving)
        {
            Vector3 newPos = transform.position;
            newPos.y -= incrementVal;
            transform.position = newPos;
        }
    }
  
}
