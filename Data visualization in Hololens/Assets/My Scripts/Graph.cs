using Assets.My_Scripts.Utility;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.My_Scripts
{
    public class Graph : MonoBehaviour
    {
        public string graphName;
        public Material BaseMat;
        public GameObject BasePrefab;
        public GameObject SubBaseLineZPrefab;
        public GameObject SubBaseLineXPrefab;
        public GameObject SupBaseLinePrefab;
        public GameObject BarPrefab;
        public GameObject GrowthBarPrefab;
        GameObject TotalBase;
        GameObject BaseGo;

        public TransitionUtility TransitionUtil;

        public int ShowYear;
        public int curZAxis;
        public int curXAxis;
        [HideInInspector]
        public int curBid;

        public float supExtraWidth = 0.07f;

        [HideInInspector]
        public int totalZAxis= 2;
        [HideInInspector]
        public int totalXAxis = 3;

        public Axis[] ZAxis = new Axis[2];   //division , brand
        public Axis[] XAxis = new Axis[3];  //year, quat, month
        public int[] growthBarNum;

        public GameObject[][,] Bar;  //Bar[Z axis, X axis]
        [HideInInspector]
        public bool firstLoad =true;
        Vector3 lastScaleOfGraph;
        //static float sceneZPosition = 1.4f;
        //static float sceneXPosition = -0.2f;

        [HideInInspector]
        public bool isMultiSelectionMode = false;
        [HideInInspector]
        public int[] barHeightScale;
        public static int barHeightScaleTMP;
        [HideInInspector]
        public int[] dividePow;

        public void OnEnable()
        {
            //GraphController.MyDebug.text += "\nCreate Graph Start Time: " + Time.time + " " + Time.realtimeSinceStartup;

            GraphController.lastSelectedBar = null;
            GraphController.lastSelectedSub = null;
            GraphController.lastSelectedSup = null;

            //lastDClickedBar = null;
            //lastDClickedSub = null;
            //lastDClickedSup = null;

            if (firstLoad)
            {
                barHeightScale = new int[totalXAxis * totalZAxis];
                dividePow = new int[totalXAxis * totalZAxis];

                TotalBase = transform.GetChild(0).gameObject;

                totalZAxis = ZAxis.Length;
                totalXAxis = XAxis.Length;

                createBase();
                createGraph(ZAxis, totalZAxis, false);
                createGraph(XAxis, totalXAxis, true);
                scaleBaselineBasedOnText();
                createBarsOnGraph();
                //GraphController.MyDebug.text += "\nCreate Graph End Time: " + Time.time +" "+ Time.realtimeSinceStartup;

            }//if firstLoad

            TotalBase.SetActive(false);
            lastScaleOfGraph = this.transform.localScale;
            this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            GraphController.GraphName.GetComponent<TMPro.TextMeshPro>().text = graphName;

            curBid = (curZAxis * totalXAxis) + curXAxis;
            //GraphController.MyDebug.text += "\nAssign Data Start Time: " + Time.time +" "+ Time.realtimeSinceStartup;
            this.GetComponent<DataManager>().assignData();
            //GraphController.MyDebug.text += "\nAssign Data End Time: " + Time.time +" "+ Time.realtimeSinceStartup;
        }//function : OnEnable()

        public IEnumerator afterAssignedData()
        {
            SetAllRequiredObjectsDisActive();
            //TransitionUtil.GraphBaseReset(BaseMat, ZAxis[curZAxis].baseLine[0].GetComponent<SubBaseLineManager>().BaseLineZMat, ZAxis[curZAxis].baseLine[0].GetComponent<SubBaseLineManager>().BaseLineXMat);

            yield return StartCoroutine(showGraph());

            //yield return StartCoroutine(TransitionUtil.BaseFadeIn(BaseMat, ZAxis[curZAxis].baseLine[0].GetComponent<SubBaseLineManager>().BaseLineZMat, ZAxis[curZAxis].baseLine[0].GetComponent<SubBaseLineManager>().BaseLineXMat));
            SetAllRequiredObjectsActive();

            yield return StartCoroutine(initializeAllBars());
        }//function : afterAssignedData()

        private void SetAllRequiredObjectsActive()
        {
            for (int i = 0; i < ZAxis[curZAxis].totalSub; i++)
            {
                ZAxis[curZAxis].baseLine[i].GetComponent<SubBaseLineManager>().heading1.SetActive(true);
                ZAxis[curZAxis].baseLine[i].GetComponent<SubBaseLineManager>().heading2.SetActive(true);
            }
            for (int i = 0; i < ZAxis[curZAxis].totalSup; i++)
            {
                ZAxis[curZAxis].baseMainLine[i].GetComponent<SupBaseLineManager>().heading1.SetActive(true);
                ZAxis[curZAxis].baseMainLine[i].GetComponent<SupBaseLineManager>().heading2.SetActive(true);
            }

            for (int i = 0; i < XAxis[curXAxis].totalSub; i++)
            {
                XAxis[curXAxis].baseLine[i].GetComponent<SubBaseLineManager>().heading1.SetActive(true);
                XAxis[curXAxis].baseLine[i].GetComponent<SubBaseLineManager>().heading2.SetActive(true);
            }
            for (int i = 0; i < XAxis[curXAxis].totalSup; i++)
            {
                XAxis[curXAxis].baseMainLine[i].GetComponent<SupBaseLineManager>().heading1.SetActive(true);
                XAxis[curXAxis].baseMainLine[i].GetComponent<SupBaseLineManager>().heading2.SetActive(true);
            }
        }//function : SetAllRequiredObjectsActive()

        private void SetAllRequiredObjectsDisActive()
        {
            for (int i = 0; i < ZAxis[curZAxis].totalSub; i++)
            {
                ZAxis[curZAxis].baseLine[i].GetComponent<SubBaseLineManager>().heading1.SetActive(false);
                ZAxis[curZAxis].baseLine[i].GetComponent<SubBaseLineManager>().heading2.SetActive(false);
            }
            for (int i = 0; i < ZAxis[curZAxis].totalSup; i++)
            {
                ZAxis[curZAxis].baseMainLine[i].GetComponent<SupBaseLineManager>().heading1.SetActive(false);
                ZAxis[curZAxis].baseMainLine[i].GetComponent<SupBaseLineManager>().heading2.SetActive(false);
            }

            for (int i = 0; i < XAxis[curXAxis].totalSub; i++)
            {
                XAxis[curXAxis].baseLine[i].GetComponent<SubBaseLineManager>().heading1.SetActive(false);
                XAxis[curXAxis].baseLine[i].GetComponent<SubBaseLineManager>().heading2.SetActive(false);
            }
            for (int i = 0; i < XAxis[curXAxis].totalSup; i++)
            {
                XAxis[curXAxis].baseMainLine[i].GetComponent<SupBaseLineManager>().heading1.SetActive(false);
                XAxis[curXAxis].baseMainLine[i].GetComponent<SupBaseLineManager>().heading2.SetActive(false);
            }
        }//function : SetAllRequiredObjectsDisActive()

        public void OnDisable()
        {
            unSelectEverything();

            //GraphController.MyDebug.text += "\nDisable Graph Start Time: " + Time.time +" "+ Time.realtimeSinceStartup;
            for (int i = 0; i < ZAxis[curZAxis].totalSub; i++)
            {
                ZAxis[curZAxis].baseLine[i].SetActive(false);
            }
            for (int i = 0; i < ZAxis[curZAxis].totalSup; i++)
            {
                ZAxis[curZAxis].baseMainLine[i].SetActive(false);
            }
            for (int i = 0; i < XAxis[curXAxis].totalSub; i++)
            {
                XAxis[curXAxis].baseLine[i].SetActive(false);
            }
            for (int i = 0; i < XAxis[curXAxis].totalSup; i++)
            {
                XAxis[curXAxis].baseMainLine[i].SetActive(false);
            }

            //Disable all Active Bars
            for (int i = 0; i < ZAxis[curZAxis].totalSub; i++)
            {
                for (int j = 0; j < XAxis[curXAxis].totalSub; j++)
                {
                    Bar[curBid][i, j].gameObject.SetActive(false);
                }
            }

            this.gameObject.SetActive(false);
            //GraphController.MyDebug.text += "\nDisable Graph End Time: " + Time.time +" "+ Time.realtimeSinceStartup;
        }//function : OnDisable()

        public void createBase()
        {
            BaseGo = (GameObject)Instantiate(BasePrefab);
            BaseGo.transform.SetParent(TotalBase.transform, false);
            BaseGo.name = "Main Base";
            BaseGo.SetActive(false);
        }//function : createBase()

        public void createGraph(Axis[] Axis, int totalCount, bool rotateAxis)
        {
            for (int id = 0; id < totalCount; id++)
            {
                float startingGap = Axis[id].BaseLineGap + Axis[id].BaseLineGroupGap - Axis[id].BarWidth;
                Axis[id].baseWidth = (Axis[id].totalSub - 1) * Axis[id].BaseLineGap + Axis[id].BarWidth + (2 * startingGap - Axis[id].BarWidth);
                Axis[id].baseWidth += (Axis[id].totalSup - 1) * Axis[id].BaseLineGroupGap; //Adding Group Gap
                Axis[id].baseStartPoint = (-Axis[id].baseWidth / 2) + startingGap;

                float startPoint;
                if (Axis[id].showSup)
                {
                    int lastTotalBars = 0;
                    Axis[id].baseMainLine = new GameObject[Axis[id].totalSup];
                    for (int i = 0; i < Axis[id].totalSup; i++)
                    {
                        Axis[id].baseMainLine[i] = Instantiate(SupBaseLinePrefab);
                        Axis[id].baseMainLine[i].transform.SetParent(TotalBase.transform, false);

                        startPoint = Axis[id].baseStartPoint - Axis[id].BarWidth;
                        float scaleOfMainLine = ((Axis[id].totalSubInSup[i] - 1) * Axis[id].BaseLineGap) + (Axis[id].BarWidth * 2);
                        float tmp = startPoint + (lastTotalBars * Axis[id].BaseLineGap) + (i * Axis[id].BaseLineGroupGap) + scaleOfMainLine / 2;

                        if (rotateAxis) //X Axis Like Year, Quat, Month
                        {
                            Axis[id].baseMainLine[i].GetComponent<SupBaseLineManager>().isZAxis = false;
                            Axis[id].baseMainLine[i].name = "Base Main Line X " + id + " " + i;
                            Axis[id].baseMainLine[i].transform.Translate(0, 0, tmp);
                            Axis[id].baseMainLine[i].transform.Rotate(Vector3.up, 90);
                        }
                        else
                        {
                            Axis[id].baseMainLine[i].GetComponent<SupBaseLineManager>().isZAxis = true;
                            Axis[id].baseMainLine[i].name = "Base Main Line Z " + id + " " + i;
                            Axis[id].baseMainLine[i].transform.Translate(tmp, 0.0015f, 0);

                            Axis[id].baseMainLine[i].GetComponent<SupBaseLineManager>().setMat(GraphController.NormalMat[i], GraphController.FocusMat[i], GraphController.SelectMat[i]);
                        }

                        Axis[id].baseMainLine[i].GetComponent<SupBaseLineManager>().graph = this;
                        Axis[id].baseMainLine[i].GetComponent<SupBaseLineManager>().firstSubID = lastTotalBars;
                        Axis[id].baseMainLine[i].GetComponent<SupBaseLineManager>().lastSubID = lastTotalBars + Axis[id].totalSubInSup[i];
                        Axis[id].baseMainLine[i].GetComponent<SupBaseLineManager>().setChildBaseLineWidth(scaleOfMainLine);
                        Axis[id].baseMainLine[i].SetActive(false);

                        lastTotalBars += Axis[id].totalSubInSup[i];
                    }
                }

                startPoint = Axis[id].baseStartPoint;
                int groupEnd=0;
                int groupNo=0;
                if (Axis[id].showSup)
                {
                    groupEnd = Axis[id].totalSubInSup[0];
                    groupNo = 1;
                }
                Axis[id].baseLine = new GameObject[Axis[id].totalSub];
                for (int i = 0; i < Axis[id].totalSub; i++)
                {
                    if (i != 0)
                        startPoint += Axis[id].BaseLineGap;

                    if(Axis[id].showSup)
                    {
                        if (i == groupEnd)
                        {
                            startPoint += Axis[id].BaseLineGroupGap;
                            groupEnd += Axis[id].totalSubInSup[groupNo++];
                        }
                    }


                    if (rotateAxis) // X axis Like Year, Quat, Month 
                    {
                        Axis[id].baseLine[i] = Instantiate(SubBaseLineXPrefab);
                        Axis[id].baseLine[i].transform.SetParent(this.transform, false);
                        Axis[id].baseLine[i].GetComponent<SubBaseLineManager>().isZAxis = false;
                        Axis[id].baseLine[i].GetComponent<SubBaseLineManager>().subID = Axis[id].totalSub - 1 - i;
                        Axis[id].baseLine[i].name = "Base Line X " + id + " " + i;
                        Axis[id].baseLine[i].transform.Translate(startPoint,0.0f, 0.0f);
                    }
                    else
                    {
                        Axis[id].baseLine[i] = Instantiate(SubBaseLineZPrefab);
                        Axis[id].baseLine[i].transform.SetParent(this.transform, false);
                        Axis[id].baseLine[i].GetComponent<SubBaseLineManager>().isZAxis = true;
                        Axis[id].baseLine[i].GetComponent<SubBaseLineManager>().subID = i;
                        Axis[id].baseLine[i].name = "Base Line Z " + id + " " + i;
                        Axis[id].baseLine[i].transform.Translate(startPoint, 0.0f, 0.0f);

                        if (Axis[id].showSup)
                            Axis[id].baseLine[i].GetComponent<SubBaseLineManager>().setMat(GraphController.NormalMat[groupNo - 1], GraphController.FocusMat[groupNo - 1], GraphController.SelectMat[groupNo - 1]);
                        else
                            Axis[id].baseLine[i].GetComponent<SubBaseLineManager>().setMat(GraphController.NormalMat[i], GraphController.FocusMat[i], GraphController.SelectMat[i]);

                    }

                    Axis[id].baseLine[i].GetComponent<SubBaseLineManager>().graph = this;
                    Axis[id].baseLine[i].GetComponent<SubBaseLineManager>().setChildBaseLineWidth(Axis[id].BaseLineWidth);

                    if (Axis[id].showSup)
                        Axis[id].baseLine[i].transform.SetParent(Axis[id].baseMainLine[groupNo - 1].transform, true);
                    else
                        Axis[id].baseLine[i].transform.SetParent(TotalBase.transform, false);
                    Axis[id].baseLine[i].SetActive(false);
                }

            }//for Axis

        }//function : createGraph(Axis[] ,int totalCount,bool rotate)

        public void createBarsOnGraph()
        {
            Bar = new GameObject[totalXAxis * totalZAxis][,];
            for (int zid = 0; zid < totalZAxis; zid++)
            {
                for (int xid = 0; xid < totalXAxis; xid++)
                {
                    int bid = (zid * totalXAxis) + xid;

                    Bar[bid] = new GameObject[ZAxis[zid].totalSub, XAxis[xid].totalSub];

                    float xStartPoint = ZAxis[zid].baseStartPoint;
                    int xGroupNo = 0;
                    int xGroupEnd = 0;
                    if (ZAxis[zid].showSup)
                    {
                        xGroupNo = 1;
                        xGroupEnd = ZAxis[zid].totalSubInSup[0];
                    }

                    for (int i = 0; i < ZAxis[zid].totalSub; i++)
                    {
                        if (i != 0)
                            xStartPoint += ZAxis[zid].BaseLineGap;


                        if (ZAxis[zid].showSup && i == xGroupEnd)
                        {
                            xStartPoint += ZAxis[zid].BaseLineGroupGap;
                            xGroupEnd += ZAxis[zid].totalSubInSup[xGroupNo++];
                        }

                        float zStartPoint = XAxis[xid].baseStartPoint;
                        int zGroupNo = 0;
                        int zGroupEnd = 0;
                        if (XAxis[xid].showSup)
                        {
                            zGroupNo = 1;
                            zGroupEnd = XAxis[xid].totalSubInSup[0];
                        }
                        for (int j = 0; j < XAxis[xid].totalSub; j++)
                        {

                            if (j != 0)
                                zStartPoint += XAxis[xid].BaseLineGap;

                            if (XAxis[xid].showSup && j == zGroupEnd)
                            {
                                zStartPoint += XAxis[xid].BaseLineGroupGap;
                                zGroupEnd += XAxis[xid].totalSubInSup[zGroupNo++];
                            }

                            if (XAxis[xid].name.Equals("Year") && Array.IndexOf(growthBarNum, j) > -1)
                                Bar[bid][i, j] = Instantiate(GrowthBarPrefab);
                            else
                                Bar[bid][i, j] = Instantiate(BarPrefab);
                            Bar[bid][i, j].name = "Bar "+bid+":"+i+" "+ (XAxis[xid].totalSub - j - 1);
                            Bar[bid][i, j].transform.SetParent(this.transform, false);
                            //Bar[bid][i, j].transform.SetParent(Bars.transform, false);
                            Bar[bid][i, j].GetComponent<BarManager>().setGraph(this, (bid * 10000) + (i * 100) + (XAxis[xid].totalSub - j - 1));
                            Bar[bid][i, j].GetComponent<BarManager>().setChildScale(ZAxis[zid].BarWidth, XAxis[xid].BarWidth);
                            if (ZAxis[zid].showSup)
                                Bar[bid][i, j].GetComponent<BarManager>().setMat(GraphController.NormalMat[xGroupNo-1], GraphController.FocusMat[xGroupNo-1], GraphController.SelectMat[xGroupNo-1]);
                            else
                                Bar[bid][i, j].GetComponent<BarManager>().setMat(GraphController.NormalMat[i], GraphController.FocusMat[i], GraphController.SelectMat[i]);
                            Bar[bid][i, j].transform.localPosition = new Vector3(xStartPoint, 0.0215f, zStartPoint);
                            Bar[bid][i, j].transform.SetParent(ZAxis[zid].baseLine[i].transform, true);
                            Bar[bid][i, j].SetActive(false);

                        }//for j
                    }//for i
                }//for xid
            }//for zid
        }//function : createBarsOnGraph()

        public void scaleBaselineBasedOnText()
        {
            for (int zid = 0; zid < totalZAxis; zid++)
            {
                for (int i = 0; i < ZAxis[zid].totalSub; i++)
                {
                    ZAxis[zid].baseLine[i].GetComponent<SubBaseLineManager>().setBaselineScale(ZAxis[zid].baselineScale);
                }
                if(ZAxis[zid].showSup)
                {
                    float tmp = ZAxis[zid].baselineScale + supExtraWidth;
                    for (int i = 0; i < ZAxis[zid].totalSup; i++)
                    {
                        //baseMainLine will be a bigger than baseline
                        ZAxis[zid].baseMainLine[i].GetComponent<SupBaseLineManager>().setBaselineScale(tmp);
                    }
                }
            }//for zid
            for (int xid = 0; xid < totalXAxis; xid++)
            {
                for (int i = 0; i < XAxis[xid].totalSub; i++)
                {
                    XAxis[xid].baseLine[i].GetComponent<SubBaseLineManager>().setBaselineScale(XAxis[xid].baselineScale);
                }
                if (XAxis[xid].showSup)
                {
                    float tmp = XAxis[xid].baselineScale + supExtraWidth;
                    for (int i = 0; i < XAxis[xid].totalSup; i++)
                    {
                        //baseMainLine will be a bigger than baseline
                        XAxis[xid].baseMainLine[i].GetComponent<SupBaseLineManager>().setBaselineScale(tmp); 
                    }
                }
            }//for xid
        }//function : scaleBaselineBasedOnText() 

        public IEnumerator showGraph()
        {
            //set Base
            BaseGo.transform.localScale = new Vector3(ZAxis[curZAxis].baseWidth, 1, XAxis[curXAxis].baseWidth);


            //reset Rotation
            this.transform.rotation = Quaternion.Euler(0.0f,0.0f,0.0f);
            //set Graph Starting point At Camera
            this.transform.position = new Vector3(ZAxis[curZAxis].baseWidth / 2.0f, this.transform.position.y, XAxis[curXAxis].baseWidth / 2.0f);
            //Rotate Around Camera Pos
            this.transform.RotateAround(Vector3.zero, Vector3.up, -45);
            //translate to some point
            this.transform.Translate(ZAxis[curZAxis].sceneXPosition, 0.0f, ZAxis[curZAxis].sceneZPosition,Space.World);

            //Set ZAxis baselines
            for (int i = 0; i < ZAxis[curZAxis].totalSub; i++)
                ZAxis[curZAxis].baseLine[i].GetComponent<SubBaseLineManager>().setChild(XAxis[curXAxis].baseWidth);
            for (int i = 0; i < ZAxis[curZAxis].totalSup; i++)
                ZAxis[curZAxis].baseMainLine[i].GetComponent<SupBaseLineManager>().setChild(XAxis[curXAxis].baseWidth);


            //Set XAxis baselines
            for (int i = 0; i < XAxis[curXAxis].totalSub; i++)
                XAxis[curXAxis].baseLine[i].GetComponent<SubBaseLineManager>().setChild(ZAxis[curZAxis].baseWidth);
            for (int i = 0; i < XAxis[curXAxis].totalSup; i++)
                XAxis[curXAxis].baseMainLine[i].GetComponent<SupBaseLineManager>().setChild(ZAxis[curZAxis].baseWidth);
 

            //Show Base
            this.transform.localScale = lastScaleOfGraph;
            TotalBase.SetActive(true);
            BaseGo.SetActive(true);

            yield return new WaitForSeconds(0.8f);

            //Show XAxis baselines
            for (int i = 0; i < XAxis[curXAxis].totalSub; i++)
                XAxis[curXAxis].baseLine[i].SetActive(true);
            for (int i = 0; i < XAxis[curXAxis].totalSup; i++)
                XAxis[curXAxis].baseMainLine[i].SetActive(true);

            yield return new WaitForSeconds(0.6f);

            //Show ZAxis baselines
            for (int i = 0; i < ZAxis[curZAxis].totalSub; i++)
                ZAxis[curZAxis].baseLine[i].SetActive(true);
            for (int i = 0; i < ZAxis[curZAxis].totalSup; i++)
                ZAxis[curZAxis].baseMainLine[i].SetActive(true);

            yield return new WaitForSeconds(0.6f);


        }//function : showGraph()

        public IEnumerator initializeAllBars()
        {
            //Enable all Active Bars
            long max = 0;
            for (int i = 0; i < ZAxis[curZAxis].totalSub; i++)
            {
                for (int j = 0; j < XAxis[curXAxis].totalSub; j++)
                {
                    Bar[curBid][i, j].gameObject.SetActive(true);
                    if (Bar[curBid][i, j].GetComponent<BarManager>().value > max)
                    {
                        max = Bar[curBid][i, j].GetComponent<BarManager>().value;
                    }
                }
            }
            dividePow[curBid] = max.ToString().Length - 4;
            if (dividePow[curBid] == 4 || dividePow[curBid] == 5)
                dividePow[curBid] = 6;
            max = (long)(max / Mathf.Pow(10, dividePow[curBid]));
            barHeightScale[curBid] = (int)(max / GraphController.barNormalizedHeight);


            for (int i = 0; i < ZAxis[curZAxis].totalSub; i++)
            {
                for (int j = 0; j < XAxis[curXAxis].totalSub; j++)
                {
                    Bar[curBid][i, j].GetComponent<BarManager>().setShortValue(dividePow[curBid]);
                    StartCoroutine(Bar[curBid][i, j].GetComponent<BarManager>().Initialize());
                }
                yield return new WaitForSeconds(ZAxis[curZAxis].delayBetweenZAxis);
            }


        }//function : initializeAllBars()

        public void setHideMode()
        {
            long max = 0;
            for (int i = 0; i < ZAxis[curZAxis].totalSub; i++)
            {
                for (int j = 0; j < XAxis[curXAxis].totalSub; j++)
                {
                    if (!Bar[curBid][i, j].GetComponent<BarManager>().gSelected)
                    {
                        Bar[curBid][i, j].GetComponent<BarManager>().setActiveBar(false);
                    }
                    else
                    {
                        if (Bar[curBid][i, j].GetComponent<BarManager>().value > max)
                        {
                            max = Bar[curBid][i, j].GetComponent<BarManager>().value;
                        }
                    }

                }//for XAxis
            }//for ZAxis
            int divideTMP = max.ToString().Length - 4;
            if (divideTMP  == 4 || divideTMP == 5)
                divideTMP = 6;
            max = (long)(max / Mathf.Pow(10, divideTMP));
            barHeightScaleTMP = (int)(max / GraphController.barNormalizedHeight);


            for (int i = 0; i < ZAxis[curZAxis].totalSub; i++)
            {
                for (int j = 0; j < XAxis[curXAxis].totalSub; j++)
                {
                    if (Bar[curBid][i, j].GetComponent<BarManager>().gSelected)
                    {
                        Bar[curBid][i, j].GetComponent<BarManager>().setHideBar(true, divideTMP);
                    }
                }//for XAxis
            }//for ZAxis
        }//function : setHideMode()

        public void resetAll()
        {
            unSelectEverything();
            for (int i = 0; i < ZAxis[curZAxis].totalSub; i++)
            {
                for (int j = 0; j < XAxis[curXAxis].totalSub; j++)
                {
                    Bar[curBid][i, j].GetComponent<BarManager>().setHideBar(false, dividePow[curBid]);
                    Bar[curBid][i, j].GetComponent<BarManager>().setActiveBar(true);
                }
            }
            isMultiSelectionMode = false;
        }//function : resetAll()

        public static void unSelectEverything()
        {
            if (GraphController.lastSelectedBar != null)
            {
                GraphController.lastSelectedBar.onUnSelect();
            }
            if (GraphController.lastSelectedSub != null)
            {
                GraphController.lastSelectedSub.onUnSelect();
            }
            if (GraphController.lastSelectedSup != null)
            {
                GraphController.lastSelectedSup.onUnSelect();
            }
        }//function : unSelectEverything()

    }//class : Graph
}//namespace