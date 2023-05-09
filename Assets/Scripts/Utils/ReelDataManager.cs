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
            AttachIDsOnReelStripDataElements(reelStripData.SymbolTable);
        }


        public static ReelStripDataModel GetReelStripData()
        {
            return reelStripData;
        }

        // sets the id of each reelStripData element to be its index number
        // this will help identify each symbol, even though there duplicates of them on the reel
        private static void AttachIDsOnReelStripDataElements(List<SymbolWeightDataModel> symbolTable)
        {
            for (int i = 0; i < symbolTable.Count; i++)
            {
                symbolTable[i].id = i;
            }
        }

        private static ReelStripDataModel ReadDataFromFile()
        {
            ReelStripDataModel result = ReadWriteJSON.ReadReelDataFromFile("reelStripConfig.json");
            Debug.Assert(result.SymbolTable != null, "UH OH!! your data from reelStripConfig.json is NULL");
            return result;

        }


    }
}