using UnityEngine;
using System.Collections;
using System.IO;

namespace Assets.My_Scripts
{

    public class DataManagerSellOut : DataManager
    {

        public string urlDivision = "sellout/getdivision";
        public string urlBrand = "sellout/getbrand";
        public string urlDivisionTop = "topvendor/getselldiv";
        public string urlBrandTop = "topvendor/getsellbrand";
        bool fetchDataComplete = false;
        bool dataAssigned = false;
        string FileNameD = "DataManagerSellOutDiv";
        string FileNameB = "DataManagerSellOutBrand";
        string FileNameDTop = "DataManagerSellOutDivTop";
        string FileNameBTop = "DataManagerSellOutBrandTop";

        Graph graph;

        void Start()
        {
            graph = this.GetComponent<Graph>();
            StartCoroutine(fetchData());
        }//function : Start()

        public IEnumerator fetchData()
        {
            WWW www = new WWW(GraphController.urlCommon + urlDivision);
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
                data = loadOfflineData(FileNameD);
            else
                saveOfflineData(FileNameD, www.text);

            GraphController.SellDivData = null;
            GraphController.SellDivData = JsonUtility.FromJson<EntityDataArray>("{\"entity\":" + data + "}");


            data = "";
            if (!offlineMode)
            {
                www = null;
                www = new WWW(GraphController.urlCommon + urlBrand);
                yield return www;
                data = www.text;
            }

            if (data.Equals(""))
                data = loadOfflineData(FileNameB);
            else
                saveOfflineData(FileNameB, www.text);

            GraphController.SellBrandData = null;
            GraphController.SellBrandData = JsonUtility.FromJson<EntityDataArray>("{\"entity\":" + data + "}");

            /*=====================================================
             * Start : Top Vendor For Baseline PopUp Data Fetch
             ======================================================*/
            data = "";
            if (!offlineMode)
            {
                www = null;
                www = new WWW(GraphController.urlCommon + urlDivisionTop);
                yield return www;
                data = www.text;
            }


            if (data.Equals(""))
                data = loadOfflineData(FileNameDTop);
            else
                saveOfflineData(FileNameDTop, www.text);

            GraphController.SellDivTopData = null;
            GraphController.SellDivTopData = JsonUtility.FromJson<TopVendorDataArray>("{\"topVen\":" + data + "}");


            data = "";
            if (!offlineMode)
            {
                www = null;
                www = new WWW(GraphController.urlCommon + urlBrandTop);
                yield return www;
                data = www.text;
            }


            if (data.Equals(""))
                data = loadOfflineData(FileNameBTop);
            else
                saveOfflineData(FileNameBTop, www.text);

            GraphController.SellBrandTopData = null;
            GraphController.SellBrandTopData = JsonUtility.FromJson<TopVendorDataArray>("{\"topVen\":" + data + "}");

            /*=====================================================
             * End : Top Vendor For Baseline PopUp Data Fetch
             ======================================================*/


            if (GraphController.SellDivData == null || GraphController.SellBrandData == null)
                Debug.Log("Error : Data Could not be fatched in fetchData().");
            else
            {
                fetchDataComplete = true;
                if (!dataAssigned)
                    assignData();
            }

            //Checking Data is Coming or Not
            //Debug.Log("Data:"+GraphController.SellDivData.entity[0].EntityName);
            //Debug.Log("Data:"+GraphController.SellBrandData.entity[0].EntityName);
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
                    if (zid == 0)
                        graph.ZAxis[zid].baseLine[i].GetComponent<SubBaseLineManager>().setName(GraphController.divisionName[i]);
                    else
                        graph.ZAxis[zid].baseLine[i].GetComponent<SubBaseLineManager>().setName(GraphController.brandName[i]);
                }
                if (graph.ZAxis[zid].showSup)
                {
                    for (int i = 0; i < graph.ZAxis[zid].totalSup; i++)
                    {
                        graph.ZAxis[zid].baseMainLine[i].GetComponent<SupBaseLineManager>().setName(GraphController.divisionNameShort[i]);
                    }
                }
            }//for xid


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
                        if (graph.ShowYear == 15)
                            graph.XAxis[xid].baseMainLine[i].GetComponent<SupBaseLineManager>().setName(GraphController.timelineName[0][2]);
                        else
                            graph.XAxis[xid].baseMainLine[i].GetComponent<SupBaseLineManager>().setName(GraphController.timelineName[0][0]);

                        graph.XAxis[xid].baseMainLine[i].GetComponent<SupBaseLineManager>().setPopUpInfo("");
                    }
                }
            }//for xid
        }//funciton : assignNamesToBaselines()

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

            EntityDataArray Data;
            if (zid == 0)
                Data = GraphController.SellDivData;
            else
                Data = GraphController.SellBrandData;


            for (int i = 0; i < graph.ZAxis[zid].totalSub; i++)
            {
                if (graph.ShowYear == 15)
                    DataManager.curTTLValue = Data.entity[i].EntityTTL15;
                else
                    DataManager.curTTLValue = Data.entity[i].EntityTTL16;

                /*======================================================
                 * Start : PopUp Info of Baseline is being set here
                 *======================================================*/
                string info = "";
                if (i != graph.ZAxis[zid].totalSub - 1)
                {
                    if (zid == 0)
                    {
                        if (graph.ShowYear == 15)
                            info = "Top 3 Vendors [2015]\n" +
                                   "- - - - - - - - - - - - - - - -\n" +
                                   "1 : " + GraphController.vendorName[GraphController.SellDivTopData.topVen[i].vid15[0]] + "\n" +
                                   "2 : " + GraphController.vendorName[GraphController.SellDivTopData.topVen[i].vid15[1]] + "\n" +
                                   "3 : " + GraphController.vendorName[GraphController.SellDivTopData.topVen[i].vid15[2]] + "\n" +
                                   "- - - - - - - - - - - - - - - -\n";
                        else
                            info = "Top 3 Vendors [2016]\n" +
                                   "- - - - - - - - - - - - - - - -\n" +
                                   "1 : " + GraphController.vendorName[GraphController.SellDivTopData.topVen[i].vid16[0]] + "\n" +
                                   "2 : " + GraphController.vendorName[GraphController.SellDivTopData.topVen[i].vid16[1]] + "\n" +
                                   "3 : " + GraphController.vendorName[GraphController.SellDivTopData.topVen[i].vid16[2]] + "\n" +
                                   "- - - - - - - - - - - - - - - -\n";
                    }
                    else
                    {
                        if (graph.ShowYear == 15)
                            info = "Top 3 Vendors [2015]\n" +
                                   "- - - - - - - - - - - - - - - -\n" +
                                   "1 : " + GraphController.vendorName[GraphController.SellBrandTopData.topVen[i].vid15[0]] + "\n" +
                                   "2 : " + GraphController.vendorName[GraphController.SellBrandTopData.topVen[i].vid15[1]] + "\n" +
                                   "3 : " + GraphController.vendorName[GraphController.SellBrandTopData.topVen[i].vid15[2]] + "\n" +
                                   "- - - - - - - - - - - - - - - -\n";
                        else
                            info = "Top 3 Vendors [2016]\n" +
                                   "- - - - - - - - - - - - - - - -\n" +
                                   "1 : " + GraphController.vendorName[GraphController.SellBrandTopData.topVen[i].vid16[0]] + "\n" +
                                   "2 : " + GraphController.vendorName[GraphController.SellBrandTopData.topVen[i].vid16[1]] + "\n" +
                                   "3 : " + GraphController.vendorName[GraphController.SellBrandTopData.topVen[i].vid16[2]] + "\n" +
                                   "- - - - - - - - - - - - - - - -\n";
                    }
                    graph.ZAxis[zid].baseLine[i].GetComponent<SubBaseLineManager>().setPopUpInfo(info);
                }//if i not Last - For Loreal Group
                else
                {
                    graph.ZAxis[zid].baseLine[i].GetComponent<SubBaseLineManager>().setPopUpInfo("");
                }
                /*======================================================
                 * End : PopUp Info of Baseline is being set here
                 *======================================================*/


                if (graph.XAxis[xid].totalSub == 3)//Year
                {
                    DataManager.curTTLValue = Data.entity[i].EntityTTL15;
                    graph.Bar[bid][i, 0].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Year);

                    graph.Bar[bid][i, 1].GetComponent<BarManager>().setValue(Data.entity[i].EntityGrowth);
                    graph.Bar[bid][i, 1].GetComponent<BarManager>().setUnit("%");
                    float diff = (Data.entity[i].Entity16Value.Year - Data.entity[i].Entity15Value.Year) / 2.0f;
                    graph.Bar[bid][i, 1].GetComponent<BarManager>().setValueAfterScale(Data.entity[i].Entity15Value.Year + diff);

                    DataManager.curTTLValue = Data.entity[i].EntityTTL16;
                    graph.Bar[bid][i, 2].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Year);
                }//if Year
                else if (graph.XAxis[xid].totalSub == 4)//Quat
                {
                    if (graph.ShowYear == 15)
                    {
                        graph.Bar[bid][i, 0].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Q1);
                        graph.Bar[bid][i, 1].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Q2);
                        graph.Bar[bid][i, 2].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Q3);
                        graph.Bar[bid][i, 3].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Q4);
                    }
                    else
                    {
                        graph.Bar[bid][i, 0].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Q1);
                        graph.Bar[bid][i, 1].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Q2);
                        graph.Bar[bid][i, 2].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Q3);
                        graph.Bar[bid][i, 3].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Q4);
                    }
                }//if Month
                if (graph.XAxis[xid].totalSub == 12)//Month
                {
                    if (graph.ShowYear == 15)
                    {
                        graph.Bar[bid][i, 0].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Jan);
                        graph.Bar[bid][i, 1].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Feb);
                        graph.Bar[bid][i, 2].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Mar);
                        graph.Bar[bid][i, 3].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Apr);
                        graph.Bar[bid][i, 4].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.May);
                        graph.Bar[bid][i, 5].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Jun);
                        graph.Bar[bid][i, 6].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Jul);
                        graph.Bar[bid][i, 7].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Aug);
                        graph.Bar[bid][i, 8].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Sep);
                        graph.Bar[bid][i, 9].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Oct);
                        graph.Bar[bid][i, 10].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Nov);
                        graph.Bar[bid][i, 11].GetComponent<BarManager>().setValue(Data.entity[i].Entity15Value.Dec);
                    }
                    else
                    {
                        graph.Bar[bid][i, 0].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Jan);
                        graph.Bar[bid][i, 1].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Feb);
                        graph.Bar[bid][i, 2].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Mar);
                        graph.Bar[bid][i, 3].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Apr);
                        graph.Bar[bid][i, 4].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.May);
                        graph.Bar[bid][i, 5].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Jun);
                        graph.Bar[bid][i, 6].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Jul);
                        graph.Bar[bid][i, 7].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Aug);
                        graph.Bar[bid][i, 8].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Sep);
                        graph.Bar[bid][i, 9].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Oct);
                        graph.Bar[bid][i, 10].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Nov);
                        graph.Bar[bid][i, 11].GetComponent<BarManager>().setValue(Data.entity[i].Entity16Value.Dec);
                    }
                }//if Month
            }//for Every Sub

            if (graph.ZAxis[zid].showSup)
            {
                string info = "";
                for (int i = 0; i < graph.ZAxis[zid].totalSup; i++)
                {
                    if (i != graph.ZAxis[zid].totalSup - 1)
                    {
                        if (graph.ShowYear == 15)
                            info = "Top 3 Vendors [2015]\n" +
                                   "- - - - - - - - - - - - - - - -\n" +
                                   "1 : " + GraphController.vendorName[GraphController.SellDivTopData.topVen[i].vid15[0]] + "\n" +
                                   "2 : " + GraphController.vendorName[GraphController.SellDivTopData.topVen[i].vid15[1]] + "\n" +
                                   "3 : " + GraphController.vendorName[GraphController.SellDivTopData.topVen[i].vid15[2]] + "\n" +
                                   "- - - - - - - - - - - - - - - -\n";
                        else
                            info = "Top 3 Vendors [2016]\n" +
                                   "- - - - - - - - - - - - - - - -\n" +
                                   "1 : " + GraphController.vendorName[GraphController.SellDivTopData.topVen[i].vid16[0]] + "\n" +
                                   "2 : " + GraphController.vendorName[GraphController.SellDivTopData.topVen[i].vid16[1]] + "\n" +
                                   "3 : " + GraphController.vendorName[GraphController.SellDivTopData.topVen[i].vid16[2]] + "\n" +
                                   "- - - - - - - - - - - - - - - -\n";

                        graph.ZAxis[zid].baseMainLine[i].GetComponent<SupBaseLineManager>().setPopUpInfo(info);
                    }
                    else
                    {
                        graph.ZAxis[zid].baseMainLine[i].GetComponent<SupBaseLineManager>().setPopUpInfo("");
                    }
                }
            }//if ZAxis ShowSup For Assign PopUp Info

        }//funciton : assignValuesToBar

    }//class : DataManagerSellOut
}//namespace