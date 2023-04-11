using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Symbol[] Symbols;

    [Range(0.0f, 10.0f)]
    public float MOVEMENT_INCREMENT;


    // Start is called before the first frame update
    void Start()
    {
        InitSymbols();        
        SpinWheel();
    }

    private void FixedUpdate()
    {
        UpdateSpeedOfSymbols(MOVEMENT_INCREMENT);
    }

    private void UpdateSpeedOfSymbols(float newIncrementVal)
    {
        for(int i = 0; i < Symbols.Length; i++)
        {
            Symbols[i].SetMovementIncrementValue(newIncrementVal);
        }
    }

    private void InitSymbols()
    {
        float TOP_MOST_POS = Symbols[0].transform.position.y;
        float BOTTOM_MOST_POS = Symbols[Symbols.Length - 1].transform.position.y - 5f;
        for (int i = 0; i < Symbols.Length; i++)
        {
            Symbols[i].Load(TOP_MOST_POS, BOTTOM_MOST_POS);
        }
    }

    private void SpinWheel()
    {
        for (int i = 0; i < Symbols.Length; i++)
        {
            Symbols[i].StartMoving();
        }
    }

   

    

  

}
