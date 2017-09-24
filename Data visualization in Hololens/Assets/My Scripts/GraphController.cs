using System.Collections;
using UnityEngine;


namespace Assets.My_Scripts
{
    public delegate void processAnimate(float valueInc);

    public class GraphController : MonoBehaviour
    {
        public static string urlCommon = "http://localhost:60077/api/";
        public static OverviewDataArray OverData = null;
        public static EntityDataArray NetDivData = null;
        public static TopVendorDataArray NetDivTopData = null;
        public static EntityDataArray NetBrandData = null;
        public static TopVendorDataArray NetBrandTopData = null;
        public static ECommerceDataArray EcomData = null;
        public static EntityDataArray SellDivData = null;
        public static TopVendorDataArray SellDivTopData = null;
        public static EntityDataArray SellBrandData = null;
        public static TopVendorDataArray SellBrandTopData = null;
        public static ChannelDataArray ChannelData = null;
        public static DivisionTopBrandDataArray ChannelTopData = null;
        public static VendorDataArray VendorData = null;
        public static DivisionTopBrandDataArray VendorTopData = null;
        public static GameObject CurrentActiveScene;

        public static GameObject CurrentActiveDashboardButton = null;
        public static GameObject CurrentActiveTimelineButton = null;
        public static GameObject CurrentActiveEntityButton = null;
        public static GameObject CurrentActiveVendorDivButton = null;
        public static GameObject CurrentNavTool = null;

        public GameObject graphName;
        public static GameObject GraphName;

        public GameObject offline;
        public static GameObject Offline;


        public static BarManager lastFocusedBar;
        public static BarManager lastSelectedBar;
        public static SubBaseLineManager lastSelectedSub;
        public static SupBaseLineManager lastSelectedSup;

        public Material[] normalMat;
        public Material[] focusMat;
        public Material[] selectMat;
        public static Material[] NormalMat;
        public static Material[] FocusMat;
        public static Material[] SelectMat;


        public static bool InsideGraphCollider = false;

        //X Axis Values needs to be in "REVERSE MANNER" 
        public static string[][] timelineName = {
            new string[]{ "2016","% Growth","2015" },
            new string[]{ "Q4", "Q3", "Q2", "Q1" },
            new string[]{ "Dec", "Nov", "Oct", "Sep", "Aug", "Jul", "Jun", "May", "Apr", "Mar", "Feb", "Jan" } };

        //X Axis Values needs to be in "REVERSE MANNER" 
        public static string[] overviewName = {"Growth", "Online", "Offline", "Total Sale" };

        //Z Axis Values needs to be in "FORWARD MANNER" 
        public static string[] ecommerceName = { "Shops", "Online", "Offline", "Other" };

        //Z Axis Values needs to be in "FORWARD MANNER" 
        public static string[] vendorNameShort = { "Stores", "Websites", "Online", "Offline", "O" };
        public static string[] vendorCatName = { "Our Stores", "Our Websites", "Online Websites", "Offline Store", "Other" };

        //Z Axis Values needs to be in "FORWARD MANNER" 
        public static string[] divisionName = { "DIV 1", "DIV 2", "DIV 3", "DIV 3", "Other" };
        public static string[] divisionNameShort = { "DIV 1", "DIV 2", "DIV 3", "DIV 3", "O" };

        //PopUp Information
        public static string[] vendorName = {"M Mart", "R3 Mall", "Flipkart", "Amazon", "ShopClues", "Jabong", "Snapdeal", "Myntra", "Alibaba.com", "JD.com", "PayTm.com", "Ebay.com", "Croma", "Vijya Salse", "HomeShop","Others","Others"};
        public static string[] brandName = { "Brand 1", "Brand 2", "Brand 3", "Brand 4", "Brand 5", "Brand 6", "Brand 7", "Brand 8", "Brand 9", "Brand 10", "Brand 11", "Brand 12", "Brand 13", "Brand 14", "Brand 15", "Brand 16", "Brand 17", "Brand 18", "Brand 19", "Brand 20", "Other" };

        public static float barNormalizedHeight = 1.0f;

        /*================================================================================================================
         * Start : Animation Code
         *===============================================================================================================*/
        public static IEnumerator animate(processAnimate processFunction, float totalTime, float startPoint, float endPoint)
        {
            float totalTimeInverse = 1 / totalTime;
            float curTime = 0;
            float diff = endPoint - startPoint;
            float valueInc = 0;
            float startTime = Time.time;

            while (curTime < totalTime)
            {
                curTime = Time.time - startTime;
                //for constant Speed
                valueInc = startPoint + (curTime * diff * totalTimeInverse);
                //for accelator Speed
                //valueInc = startPoint + (curTime * curTime * diff * totalTimeInverse * totalTimeInverse);
                if (valueInc < 0.001f)
                    valueInc = 0.001f;

                processFunction(valueInc);
                yield return null;
            }
        }//function : animate()

        /*================================================================================================================
         * End : Animation Code
         *===============================================================================================================*/



        public static TextMesh MyDebug;

        void Awake()
        {
            GraphName = graphName;
            Offline = offline;

            NormalMat = new Material[5];
            FocusMat = new Material[5];
            SelectMat = new Material[5];
            for (int i=0; i<5; i++)
            {
                NormalMat[i] = normalMat[i];
                FocusMat[i] = focusMat[i];
                SelectMat[i] = selectMat[i];
            }

            GameObject tmp = GameObject.FindGameObjectWithTag("MyDebug");
            if (tmp == null)
                return;

            MyDebug = tmp.GetComponent<TextMesh>();
            MyDebug.text = "Nothing";
        }

    }//class : GraphController
}//namespace