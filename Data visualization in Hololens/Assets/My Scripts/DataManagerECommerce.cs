using UnityEngine;
using System.Collections;
using System.IO;

namespace Assets.My_Scripts
{

    public class DataManagerECommerce : DataManager
    {

        public string urlEcom = "ecommerce/get";
        bool fetchDataComplete = false;
        bool dataAssigned = false;
        string FileName = "DataManagerECommerce";

        Graph graph;

        void Start()
        {
            graph = this.GetComponent<Graph>();
            StartCoroutine(fetchData());
        }//function : Start()

        public IEnumerator fetchData()
        {
            WWW www = new WWW(GraphController.urlCommon + urlEcom);

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

            GraphController.EcomData = null;
            GraphController.EcomData = JsonUtility.FromJson<ECommerceDataArray>("{\"ecommerce\":" + data + "}");

            if (GraphController.EcomData == null)
                Debug.Log("Error : Data Could not be fatched in fetchData().");
            else
            {
                fetchDataComplete = true;
                if (!dataAssigned)
                    assignData();
            }

            //Checking Data is Coming or Not
            //Debug.Log("Data:" + GraphController.EcomData.ecommerce[0].DivisionName);
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
                name = GraphController.ecommerceName[i % 4];
                graph.ZAxis[zid].baseLine[i].GetComponent<SubBaseLineManager>().setName(name);
                graph.ZAxis[zid].baseLine[i].GetComponent<SubBaseLineManager>().setPopUpInfo("");
            }
            if (graph.ZAxis[zid].showSup)
            {
                for (int i = 0; i < graph.ZAxis[zid].totalSup; i++)
                {
                    graph.ZAxis[zid].baseMainLine[i].GetComponent<SupBaseLineManager>().setName(GraphController.divisionName[i]);
                    graph.ZAxis[zid].baseMainLine[i].GetComponent<SupBaseLineManager>().setPopUpInfo("");
                }
            }


            int xid = 0;
            for (int i = 0; i < graph.XAxis[xid].totalSub; i++)
            {
                name = GraphController.timelineName[xid][i];
                graph.XAxis[xid].baseLine[i].GetComponent<SubBaseLineManager>().setName(name);
                graph.XAxis[xid].baseLine[i].GetComponent<SubBaseLineManager>().setPopUpInfo("");
            }
            if(graph.XAxis[xid].showSup)
            {
                for (int i = 0; i < graph.XAxis[xid].totalSup; i++)
                    graph.XAxis[xid].baseMainLine[i].GetComponent<SupBaseLineManager>().setPopUpInfo("");
            }
        }//function :assignNamesToBaselines()

        public void assignValuesToBars()
        {
            int zid = graph.curZAxis;
            int xid = graph.curXAxis;
            int bid = (zid * graph.totalXAxis) + xid;

            for (int i = 0; i < graph.ZAxis[zid].totalSub; i++)
            {
               
                if (graph.XAxis[xid].totalSub == 3)//Year
                {
                    DataManager.curTTLValue = GraphController.EcomData.ecommerce[i].ECommerce15TTL;
                    graph.Bar[bid][i, 0].GetComponent<BarManager>().setValue(GraphController.EcomData.ecommerce[i].Ecommerce15Value);

                    graph.Bar[bid][i, 1].GetComponent<BarManager>().setValue(GraphController.EcomData.ecommerce[i].EcommerceGrowth);
                    graph.Bar[bid][i, 1].GetComponent<BarManager>().setUnit("%");
                    float diff = (GraphController.EcomData.ecommerce[i].Ecommerce16Value - GraphController.EcomData.ecommerce[i].Ecommerce15Value) / 2.0f;
                    graph.Bar[bid][i, 1].GetComponent<BarManager>().setValueAfterScale(GraphController.EcomData.ecommerce[i].Ecommerce15Value + diff);

                    DataManager.curTTLValue = GraphController.EcomData.ecommerce[i].ECommerce16TTL;
                    graph.Bar[bid][i, 2].GetComponent<BarManager>().setValue(GraphController.EcomData.ecommerce[i].Ecommerce16Value);
                }//if Year
            }//for Every Sub
        }//funciton : assignValuesToBar()

    }//class : DataManagerECommerce
}//namespace