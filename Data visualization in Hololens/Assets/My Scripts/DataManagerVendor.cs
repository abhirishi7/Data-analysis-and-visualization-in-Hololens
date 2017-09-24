using UnityEngine;
using System.Collections;
using System.IO;

namespace Assets.My_Scripts
{

    public class DataManagerVendor : DataManager
    {

        public string urlVendor = "vendor/get";
        public string urlVendorTop = "topbrand/getvendor";
        public int curDivision = 0; 
        bool fetchDataComplete = false;
        bool dataAssigned = false;
        string FileName = "DataManagerVendor";
        string FileNameTop = "DataManagerVendorTop";

        Graph graph;

        void Start()
        {
            graph = this.GetComponent<Graph>();
            StartCoroutine(fetchData());
        }//function : Start()

        public IEnumerator fetchData()
        {
            WWW www = new WWW(GraphController.urlCommon + urlVendor);
            string data = "";
            if (!offlineMode)
            {
                float endTime = Time.realtimeSinceStartup + DataManager.waitingTimeForOnline;
                while (Time.realtimeSinceStartup < endTime)
                {
                    if (www.isDone)
                        break;
                    else
                        yield return new WaitForSeconds(0.1f);
                }

                if (www.isDone)
                    data = www.text;
                else
                    offlineMode = true;
            }

            if (data.Equals(""))
                data = loadOfflineData(FileName);
            else
                saveOfflineData(FileName, www.text);

            GraphController.VendorData = null;
            GraphController.VendorData = JsonUtility.FromJson<VendorDataArray>("{\"venDiv\":" + data + "}");

            /*=====================================================
             * Start : Top Brand For Baseline PopUp Data Fetch
             ======================================================*/
            data = "";
            if (!offlineMode)
            {
                www = null;
                www = new WWW(GraphController.urlCommon + urlVendorTop);
                yield return www;
                data = www.text;
            }


            if (data.Equals(""))
                data = loadOfflineData(FileNameTop);
            else
                saveOfflineData(FileNameTop, www.text);

            GraphController.VendorTopData = null;
            GraphController.VendorTopData = JsonUtility.FromJson<DivisionTopBrandDataArray>("{\"divTop\":" + data + "}");
            /*=====================================================
             * End : Top Brand For Baseline PopUp Data Fetch
             ======================================================*/

            if (GraphController.VendorData == null)
                Debug.Log("Error : Data Could not be fatched in fetchData().");
            else
            {
                fetchDataComplete = true;
                if (!dataAssigned)
                    assignData();
            }

            //Checking Data is Coming or Not
            //Debug.Log("Data:" + GraphController.VendorData.venDiv[0].vendor[0].VendorName);
        }//function : fetchData()

        public override void assignData()
        {
            if (fetchDataComplete)
            {
                dataAssigned = true;
                if (graph.firstLoad)
                {
                    assignNamesToBaselines();
                    graph.firstLoad = false;
                }
                assignValuesToBars();
                StartCoroutine(graph.afterAssignedData());
            }
        }//function : assignData()

        public void assignNamesToBaselines()
        {
            string name = "";

            for (int zid = 0; zid < graph.totalZAxis; zid++)
            {
                for (int i = 0; i < graph.ZAxis[zid].totalSub; i++)
                {
                    name = GraphController.VendorData.venDiv[0].vendor[i].VendorName;
                    graph.ZAxis[zid].baseLine[i].GetComponent<SubBaseLineManager>().setName(GraphController.vendorName[i]);
                }
                if (graph.ZAxis[zid].showSup)
                {
                    for (int i = 0; i < graph.ZAxis[zid].totalSup; i++)
                    {
                        graph.ZAxis[zid].baseMainLine[i].GetComponent<SupBaseLineManager>().setName(GraphController.vendorNameShort[i]);
                        graph.ZAxis[zid].baseMainLine[i].GetComponent<SupBaseLineManager>().setPopUpInfo("");
                    }
                }
            }//for zid


            for (int xid = 0; xid < graph.totalXAxis; xid++)
            {
                for (int i = 0; i < graph.XAxis[xid].totalSub; i++)
                {
                    name = GraphController.timelineName[xid][i];
                    graph.XAxis[xid].baseLine[i].GetComponent<SubBaseLineManager>().setName(name);
                    graph.XAxis[xid].baseLine[i].GetComponent<SubBaseLineManager>().setPopUpInfo("");
                }
                if (graph.XAxis[xid].showSup)
                {
                    for (int i = 0; i < graph.XAxis[xid].totalSup; i++)
                    {
                        if(graph.ShowYear==15)
                            graph.XAxis[xid].baseMainLine[i].GetComponent<SupBaseLineManager>().setName(GraphController.timelineName[0][2]);
                        else
                            graph.XAxis[xid].baseMainLine[i].GetComponent<SupBaseLineManager>().setName(GraphController.timelineName[0][0]);

                        graph.XAxis[xid].baseMainLine[i].GetComponent<SupBaseLineManager>().setPopUpInfo("");
                    }
                }
            }//for xid
        }//function : assignNamesToBaselines()

        public void assignValuesToBars()
        {
            int zid = graph.curZAxis;
            int xid = graph.curXAxis;
            int bid = (zid * graph.totalXAxis) + xid;

            if (graph.XAxis[xid].showSup)
            {
                for (int i = 0; i < graph.XAxis[xid].totalSup; i++)
                {
                    if (graph.ShowYear == 15)
                        graph.XAxis[xid].baseMainLine[i].GetComponent<SupBaseLineManager>().setName(GraphController.timelineName[0][2]);
                    else
                        graph.XAxis[xid].baseMainLine[i].GetComponent<SupBaseLineManager>().setName(GraphController.timelineName[0][0]);
                }
            }

            for (int i = 0; i < graph.ZAxis[zid].totalSub; i++)
            {
                if (graph.ShowYear == 15)
                    DataManager.curTTLValue = GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15TTL;
                else
                    DataManager.curTTLValue = GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16TTL;

                /*======================================================
                * Start : PopUp Info of Baseline is being set here
                *======================================================*/
                string info = "";
                if (graph.ShowYear == 15)
                    info = "Top 3 Brands [2015]\n" +
                            "- - - - - - - - - - - - - - - -\n" +
                            "1 : " + GraphController.brandName[GraphController.VendorTopData.divTop[curDivision].ven[i].pid15[0]] + "\n" +
                            "2 : " + GraphController.brandName[GraphController.VendorTopData.divTop[curDivision].ven[i].pid15[1]] + "\n" +
                            "3 : " + GraphController.brandName[GraphController.VendorTopData.divTop[curDivision].ven[i].pid15[2]] + "\n" +
                            "- - - - - - - - - - - - - - - -\n";
                else
                    info = "Top 3 Brands [2016]\n" +
                            "- - - - - - - - - - - - - - - -\n" +
                            "1 : " + GraphController.brandName[GraphController.VendorTopData.divTop[curDivision].ven[i].pid16[0]] + "\n" +
                            "2 : " + GraphController.brandName[GraphController.VendorTopData.divTop[curDivision].ven[i].pid16[1]] + "\n" +
                            "3 : " + GraphController.brandName[GraphController.VendorTopData.divTop[curDivision].ven[i].pid16[2]] + "\n" +
                            "- - - - - - - - - - - - - - - -\n";
                graph.ZAxis[zid].baseLine[i].GetComponent<SubBaseLineManager>().setPopUpInfo(info);
                /*======================================================
                 * End : PopUp Info of Baseline is being set here
                 *======================================================*/


                if (graph.XAxis[xid].totalSub == 3)//Year
                {
                    DataManager.curTTLValue = GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15TTL;
                    graph.Bar[bid][i, 0].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Year);

                    graph.Bar[bid][i, 1].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].VendorGrowth);
                    graph.Bar[bid][i, 1].GetComponent<BarManager>().setUnit("%");
                    float diff = (GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Year - GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Year) / 2.0f;
                    graph.Bar[bid][i, 1].GetComponent<BarManager>().setValueAfterScale(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Year + diff);

                    DataManager.curTTLValue = GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16TTL;
                    graph.Bar[bid][i, 2].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Year);
                }//if Year
                else if (graph.XAxis[xid].totalSub == 4)//Quat
                {
                    if (graph.ShowYear == 15)
                    {
                        graph.Bar[bid][i, 0].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Q1);
                        graph.Bar[bid][i, 1].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Q2);
                        graph.Bar[bid][i, 2].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Q3);
                        graph.Bar[bid][i, 3].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Q4);
                    }
                    else
                    {
                        graph.Bar[bid][i, 0].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Q1);
                        graph.Bar[bid][i, 1].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Q2);
                        graph.Bar[bid][i, 2].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Q3);
                        graph.Bar[bid][i, 3].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Q4);
                    }
                }//if Month
                if (graph.XAxis[xid].totalSub == 12)//Month
                {
                    if (graph.ShowYear == 15)
                    {
                        graph.Bar[bid][i, 0].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Jan);
                        graph.Bar[bid][i, 1].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Feb);
                        graph.Bar[bid][i, 2].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Mar);
                        graph.Bar[bid][i, 3].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Apr);
                        graph.Bar[bid][i, 4].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.May);
                        graph.Bar[bid][i, 5].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Jun);
                        graph.Bar[bid][i, 6].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Jul);
                        graph.Bar[bid][i, 7].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Aug);
                        graph.Bar[bid][i, 8].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Sep);
                        graph.Bar[bid][i, 9].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Oct);
                        graph.Bar[bid][i, 10].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Nov);
                        graph.Bar[bid][i, 11].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor15Value.Dec);
                    }
                    else
                    {
                        graph.Bar[bid][i, 0].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Jan);
                        graph.Bar[bid][i, 1].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Feb);
                        graph.Bar[bid][i, 2].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Mar);
                        graph.Bar[bid][i, 3].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Apr);
                        graph.Bar[bid][i, 4].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.May);
                        graph.Bar[bid][i, 5].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Jun);
                        graph.Bar[bid][i, 6].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Jul);
                        graph.Bar[bid][i, 7].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Aug);
                        graph.Bar[bid][i, 8].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Sep);
                        graph.Bar[bid][i, 9].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Oct);
                        graph.Bar[bid][i, 10].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Nov);
                        graph.Bar[bid][i, 11].GetComponent<BarManager>().setValue(GraphController.VendorData.venDiv[curDivision].vendor[i].Vendor16Value.Dec);
                    }
                }//if Month
            }//for Every Sub
        }//funciton : assignValuesToBar()

    }//class : DataManagerVendor
}//namespace