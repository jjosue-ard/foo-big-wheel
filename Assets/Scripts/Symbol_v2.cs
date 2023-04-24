using Assets.Scripts.Datamodels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Symbol_v2 : MonoBehaviour
{
    public TextMeshProUGUI txtDisplay;

    private bool wheelIsMoving;    
    private float movementIncrementValue;
    private SymbolWeightDataModel symbolData;

    // Start is called before the first frame update
    public void Load(int i, SymbolWeightDataModel data)
    {
        wheelIsMoving = false;
        txtDisplay.text = data.DisplayedText;

        // deep copy
        symbolData = new SymbolWeightDataModel();
        DeepCopyData(data);
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
    
    private void DeepCopyData(SymbolWeightDataModel data)
    {
        symbolData.DisplayedText = data.DisplayedText;
        symbolData.PayValue = data.PayValue;
        symbolData.Weight = data.Weight;

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
