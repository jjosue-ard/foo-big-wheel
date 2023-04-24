// Get Newtonsoft's JsonDotNet from Unity Assetstore
using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using Assets.Scripts.Datamodels;
/*
*
* NOTE: make sure to add this into Packages/manifest.json --> "com.unity.nuget.newtonsoft-json": "2.0.0"
*/
namespace Assets.Scripts.Utils
{
    public class ReadWriteJSON
    {

        public static ReelStripDataModel ReadReelDataFromFile(string fileName)
        {
            ReelStripDataModel result;            
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            string fileContent = ReadWriteFileScript.EnsureReadFile(filePath);

            result = JsonConvert.DeserializeObject<ReelStripDataModel>(fileContent);

            return result;
        }
    }
}