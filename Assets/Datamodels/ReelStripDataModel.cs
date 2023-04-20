using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Datamodels
{
    [Serializable]
    public class ReelStripDataModel : MonoBehaviour
    {
        public int SymbolCountPerReelStrip;
        public List<SymbolWeightDataModel> SymbolTable;
    }
}
