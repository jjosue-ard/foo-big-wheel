using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class ReadWriteFileScript
    {
        public static void WriteStringToFile(string content, string filePath)
        {
            Debug.Log("Attempting to write file to: " + filePath);

            byte[] contentBytes = System.Text.Encoding.ASCII.GetBytes(content);
            try
            {
                System.IO.File.WriteAllBytes(filePath, contentBytes);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);

            }
            Debug.Log("Completed writing file: " + filePath);
            Debug.Assert(System.IO.File.Exists(filePath));
        }


        public static void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }


        private static bool FileExists(string filePath)
        {
            bool result = true;
            FileInfo info = new FileInfo(filePath);
            if (info == null || info.Exists == false)
            {
                result = false;
            }
            else
            {
                Debug.LogWarning("file DOES NOT exist: " + filePath);
            }
            return result;
        }

        public static string EnsureReadFile(string filePath)
        {
            string result = null;
            if (FileExists(filePath))
            {
                result = File.ReadAllText(filePath);
            }
            else
            {
                Debug.LogWarning("File DOES NOT exist: " + filePath);
            }
            return result;

        }



    }
}