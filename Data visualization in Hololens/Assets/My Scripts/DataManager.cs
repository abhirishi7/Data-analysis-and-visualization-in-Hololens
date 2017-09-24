using System;
using System.IO;
using UnityEngine;

namespace Assets.My_Scripts
{
    public class DataManager : MonoBehaviour
    {
        public static bool offlineMode=false;
        public static long curTTLValue;
        TextAsset reader;

        public static float waitingTimeForOnline = 1.0f;
        public virtual void assignData()
        {

        }//function : assignValuesToBars()

        public void saveOfflineData(string FileName, string wwwText)
        {
            #if WINDOWS_UWP
            #else
            string file = "Assets/Resources/" + FileName + ".txt";
            StreamWriter writer = new StreamWriter(new FileStream(file, FileMode.Create));
            writer.Write("" + wwwText);
            writer.Close();
            GraphController.Offline.SetActive(false);
            #endif
        }//function : saveOfflineData(string FileName, string wwwText)

        public string loadOfflineData(string FileName)
        {
            string data ="";
            reader = Resources.Load(FileName) as TextAsset;
            data = reader.text;
            GraphController.Offline.SetActive(true);
            return data;
        }//function : loadOfflineData(string FileName)

    }//class : DataManager
}//namespace