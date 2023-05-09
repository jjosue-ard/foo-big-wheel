using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Datamodels
{
    [Serializable]
    public class SymbolWeightDataModel
    {
        public int id; // will help identify this symbol despite duplicate DisplayedTexts
        public string DisplayedText;
        public float Weight;
        public float PayValue;        
    }

 
}
