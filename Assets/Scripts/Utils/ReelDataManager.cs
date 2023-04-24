using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Datamodels;

namespace Assets.Scripts.Utils
{
    public class ReelDataManager : MonoBehaviour
    {
        private static ReelStripDataModel reelStripData;

        public static void Load()
        {
            reelStripData = ReadDataFromFile();
        }


        public static ReelStripDataModel GetReelStripData()
        {
            return reelStripData;
        }

        private static ReelStripDataModel ReadDataFromFile()
        {
            ReelStripDataModel result = ReadWriteJSON.ReadReelDataFromFile("reelStripConfig.json");
            Debug.Assert(result.SymbolTable != null, "UH OH!! your data from reelStripConfig.json is NULL");
            return result;

        }


    }
}