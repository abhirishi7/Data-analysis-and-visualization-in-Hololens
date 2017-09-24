using UnityEngine;
using System.Collections;



namespace Assets.My_Scripts
{

    public class DataManagerOverview : DataManager
    {

        public string urlOverview = "overview/get";
        bool fetchDataComplete =false;
        bool dataAssigned= false;
        string FileName = "DataManagerOverview";

        Graph graph;

        void Start()
        {
            graph = this.GetComponent<Graph>();
            StartCoroutine(fetchData());
        }//funciton : Start();

        public IEnumerator fetchData()
        {
            WWW www = new WWW(GraphController.urlCommon + urlOverview);
            string data="";
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


            GraphController.OverData = null;
            GraphController.OverData = JsonUtility.FromJson<OverviewDataArray>("{\"overview\":" + data + "}");

            if (GraphController.OverData == null)
                Debug.Log("Error : Data Could not be fatched in fetchData().");
            else
            {
                fetchDataComplete = true;
                if (!dataAssigned)
                    assignData();
            }

            //Checking Data is Coming or Not
            //Debug.Log("Data:" + GraphController.OverData.overview[0].DivisionName);

            yield return null;
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
            int zid = 0;
            for (int i = 0; i < graph.ZAxis[zid].totalSub; i++)
            {
                name = GraphController.OverData.overview[i].DivisionName;
                graph.ZAxis[zid].baseLine[i].GetComponent<SubBaseLineManager>().setName(GraphController.divisionName[i]);
                graph.ZAxis[zid].baseLine[i].GetComponent<SubBaseLineManager>().setPopUpInfo("");
            }

            int xid = 0;
            for (int i = 0; i < graph.XAxis[xid].totalSub; i++)
            {
                name = GraphController.overviewName[i];
                graph.XAxis[xid].baseLine[i].GetComponent<SubBaseLineManager>().setName(GraphController.overviewName[i]);
                graph.XAxis[xid].baseLine[i].GetComponent<SubBaseLineManager>().setPopUpInfo("");
            }

        }//function : assignNamesToBaselines()

        public void assignValuesToBars()
        {
            int bid = 0;
            for (int zid = 0; zid < graph.totalZAxis; zid++)
            {
                for (int xid = 0; xid < graph.totalXAxis; xid++)
                {
                    bid = (zid * graph.totalXAxis) + xid;

                    for (int i = 0; i < graph.ZAxis[zid].totalSub; i++)
                    {
                        if (graph.XAxis[xid].totalSub == 4)//NetSales, Indirect ,Direct, Growth
                        {
                            DataManager.curTTLValue = GraphController.OverData.overview[i].NetSalesTTL;
                            graph.Bar[bid][i, 0].GetComponent<BarManager>().setValue(GraphController.OverData.overview[i].NetSalesValue);

                            graph.Bar[bid][i, 1].GetComponent<BarManager>().setUnit("%");
                            graph.Bar[bid][i, 1].GetComponent<BarManager>().setValue(GraphController.OverData.overview[i].IndirectValue);
                            graph.Bar[bid][i, 2].GetComponent<BarManager>().setUnit("%");
                            graph.Bar[bid][i, 2].GetComponent<BarManager>().setValue(GraphController.OverData.overview[i].DirectValue);
                            graph.Bar[bid][i, 3].GetComponent<BarManager>().setUnit("%");
                            graph.Bar[bid][i, 3].GetComponent<BarManager>().setValue(GraphController.OverData.overview[i].NetSalesGrowth);
                        }//if Year
                    }//for Every Brand
                }//for xid
            }//for zid
        }//function : assignValuesToBar()

    }//class : DataManagerOverview
}//namespace