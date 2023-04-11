using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol : MonoBehaviour
{
    private bool wheelIsMoving;
    private float topPos;
    private float bottomPos;

    private float movementIncrementValue;

    // Start is called before the first frame update
    public void Load(float _topPos, float _bottomPos)
    {
        wheelIsMoving = false;
        topPos = _topPos;
        bottomPos = _bottomPos;
    }

    public void StartMoving()
    {
        wheelIsMoving=true;
    }

    public void SetMovementIncrementValue(float newVal)
    {
        movementIncrementValue = newVal;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EnsureMoveSymbol(movementIncrementValue);
        EnsureSpawnSymbolsBackAtTopPosition(bottomPos);
    }

    private void EnsureMoveSymbol(float incrementVal)
    {
        if(wheelIsMoving)
        {
            Vector3 newPos = transform.position;
            newPos.y -= incrementVal;
            transform.position = newPos;
        }
    }

    private void EnsureSpawnSymbolsBackAtTopPosition(float bottomPos)
    {       
        float y_pos = transform.position.y;
        bool isWithinRangeOfRespawnTrigger = (y_pos - 0.001f) <= bottomPos;
        if (isWithinRangeOfRespawnTrigger)
        {
            SetSymbolToBeAtYPos(this.gameObject, topPos + movementIncrementValue);
        }
        
    }

    private void SetSymbolToBeAtYPos(GameObject symbol, float yPos)
    {
        Vector3 newPos = symbol.transform.position;
        newPos.y = yPos;
        symbol.transform.position = newPos;
    }
    
}
