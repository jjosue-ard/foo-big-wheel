using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Datamodels
{
    [Serializable]
    public class SymbolWeightDataModel : MonoBehaviour
    {
        public string DisplayedText;
        public float Weight;
        public float PayValue;

        public SymbolWeightDataModel(SymbolWeightDataModel dataToCopy)
        {
            dataToCopy.DisplayedText = DisplayedText;
            dataToCopy.Weight = Weight;
            dataToCopy.PayValue = PayValue;
        }

    }

 
}
