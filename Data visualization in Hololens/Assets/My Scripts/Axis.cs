using UnityEngine;




namespace Assets.My_Scripts
{
    [System.Serializable]
    public class Axis  //Year Only, Division Only
    {
        public string name;
        public int totalSub;
        public bool showSup;
        public int totalSup;
        public int[] totalSubInSup = new int[6];
        public float BarWidth;
        public float BaseLineWidth;
        public float BaseLineGap;
        public float BaseLineGroupGap;
        public float baselineScale;   //Depends on No of Letters in Longest String
        [HideInInspector]
        public float baseWidth;
        [HideInInspector]
        public float baseStartPoint;
        [HideInInspector]
        public GameObject[] baseLine;
        [HideInInspector]
        public GameObject[] baseMainLine;

        public float delayBetweenZAxis = 0.3f;
        public float sceneZPosition;
        public float sceneXPosition;

        public Axis()
        {
            baseLine = new GameObject[totalSub];
            baseMainLine = new GameObject[totalSup];
        }//Constructor : Axis()

    }//class : Axis
}//namespace