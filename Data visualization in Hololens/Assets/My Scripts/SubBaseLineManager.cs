using UnityEngine;
using TMPro;
using System.Collections;

namespace Assets.My_Scripts
{
    public class SubBaseLineManager : MonoBehaviour
    {
        [HideInInspector]
        public bool isZAxis;
        [HideInInspector]
        public Graph graph;
        [HideInInspector]
        public int subID;

        public GameObject baselineHeading1;
        public GameObject heading1;
        public GameObject baselineMesh;
        public GameObject baselineHeading2;
        public GameObject heading2;

        public GameObject baselineCanvas;
        public GameObject baseLinePopUp1;
        public GameObject baseLinePopUp1Board;
        public GameObject baseLinePopUp1BoardInfo;
        public GameObject baseLinePopUp1Stick;
        public GameObject baseLineDashLine1;
        public GameObject baseLinePopUp2;
        public GameObject baseLinePopUp2Board;
        public GameObject baseLinePopUp2BoardInfo;
        public GameObject baseLinePopUp2Stick;
        public GameObject baseLineDashLine2;
        public static float infoPopUpMarginFromBaseLine = 0.2f;
        public static float stickSize = 0.005f;
        public static float dashLineLength = 0.2f;
        Vector3 defaultPosPopUp1;
        Vector3 defaultPosPopUp2;
        bool isPopUpInfoAvailable = true;

        public Material BaseLineZMat;
        public Material BaseLineXMat;
        public Material BaseLineFocusMat;
        public Material BaseLineSelectMat;

        bool focused = false;
        bool selected = false;
        bool highlited = false;
        //bool dClicked = false;


        public void OnEnable()
        {
            StartCoroutine(animateBaseline());
        }//function : OnEnable()

        public void setChild(float baseWidth)
        {
            float halfWidth = baseWidth / 2;
            baselineMesh.transform.localScale = new Vector3(baselineMesh.transform.localScale.x,1.0f,baseWidth);
            baselineHeading1.transform.localPosition = new Vector3(0.0f, 0.0f, -halfWidth);
            baselineHeading2.transform.localPosition = new Vector3(0.0f, 0.0f, halfWidth);
            heading1.transform.localPosition = new Vector3(0.0f, 0.001f, -halfWidth);
            heading2.transform.localPosition = new Vector3(0.0f, 0.001f, halfWidth);

            //----------setting for PopUp
            baseLinePopUp1.GetComponent<RectTransform>().localPosition = defaultPosPopUp1;
            baseLinePopUp2.GetComponent<RectTransform>().localPosition = defaultPosPopUp2;
            baseLinePopUp1.GetComponent<RectTransform>().Translate(halfWidth, 0.0f, 0.0f);
            baseLinePopUp2.GetComponent<RectTransform>().Translate(halfWidth, 0.0f, 0.0f);

        }//function : setChild(float baseWidth)

        public void setChildBaseLineWidth(float baselineWidth)
        {
            baselineHeading1.transform.localScale = new Vector3(baselineWidth,1.0f,1.0f);
            baselineMesh.transform.localScale = new Vector3(baselineWidth, 1.0f, 1.0f);
            baselineHeading2.transform.localScale = new Vector3(baselineWidth, 1.0f, 1.0f);
            //Debug.Log("Sub Baseline Font Size:"+ ((baselineWidth * 10) - 0.2f));
            heading1.GetComponentInChildren<TextMeshPro>().fontSize = (baselineWidth * 10) - 0.2f;
            heading2.GetComponentInChildren<TextMeshPro>().fontSize = (baselineWidth * 10) - 0.2f;
        }//function : setChildBaseLineWidth()

        public void setName(string name)
        {
            heading1.GetComponentInChildren<TextMeshPro>().text = name;
            heading2.GetComponentInChildren<TextMeshPro>().text = name;
        }//function : setName(string name)

        public void setMat(Material nor, Material focus, Material sel)
        {
            //BaseLineZMat = nor;
            BaseLineFocusMat = focus;
            BaseLineSelectMat = sel;
            //if (isZAxis)
            //{
            //    baselineMesh.GetComponentInChildren<MeshRenderer>().material = BaseLineZMat;
            //    baselineHeading1.GetComponentInChildren<MeshRenderer>().material = BaseLineZMat;
            //    baselineHeading2.GetComponentInChildren<MeshRenderer>().material = BaseLineZMat;
            //}
        }//function : setMat(Material nor, Material focus, Material sel)

        public void setBaselineScale(float length)
        {
            baselineHeading1.transform.localScale = new Vector3(baselineHeading1.transform.localScale.x, 1.0f, length);
            baselineHeading2.transform.localScale = new Vector3(baselineHeading2.transform.localScale.x, 1.0f, length);

            setPopUp(length);

        }//function : setBaselineScale(float length)

        public void setPopUp(float length)
        {
            baseLinePopUp1.GetComponent<RectTransform>().Translate(0.0f, 0.0f, -length, baselineHeading1.transform);
            baseLinePopUp2.GetComponent<RectTransform>().Translate(0.0f, 0.0f, length, baselineHeading2.transform);
            baseLinePopUp1.GetComponent<RectTransform>().Translate(dashLineLength, 0.0f, 0.0f, Space.Self);
            baseLinePopUp2.GetComponent<RectTransform>().Translate(dashLineLength, 0.0f, 0.0f, Space.Self);

            baseLineDashLine1.GetComponent<RectTransform>().sizeDelta = new Vector3(dashLineLength,stickSize);
            baseLineDashLine2.GetComponent<RectTransform>().sizeDelta = new Vector3(dashLineLength, stickSize);

            baseLinePopUp1Board.transform.localPosition = new Vector3(0.0f, infoPopUpMarginFromBaseLine - 0.01f, 0.0f);
            baseLinePopUp2Board.transform.localPosition = new Vector3(0.0f, infoPopUpMarginFromBaseLine - 0.01f, 0.0f);

            baseLinePopUp1Stick.transform.localScale = new Vector3(stickSize, infoPopUpMarginFromBaseLine, stickSize);
            baseLinePopUp2Stick.transform.localScale = new Vector3(stickSize, infoPopUpMarginFromBaseLine, stickSize);

            defaultPosPopUp1 = baseLinePopUp1.GetComponent<RectTransform>().localPosition;
            defaultPosPopUp2 = baseLinePopUp2.GetComponent<RectTransform>().localPosition;

            baseLinePopUp1.SetActive(false);
            baseLinePopUp2.SetActive(false);
            baselineCanvas.SetActive(false);

        }//function : setPopUp(float length)

        public void setPopUpInfo(string info)
        {
            if(info == "")
                isPopUpInfoAvailable = false;
            baseLinePopUp1BoardInfo.GetComponentInChildren<TextMeshPro>().text = info;
            baseLinePopUp2BoardInfo.GetComponentInChildren<TextMeshPro>().text = info;
        }

        public void Update()
        {
            if(selected && isPopUpInfoAvailable)
            {
                float disFromPopUp1 = (baseLinePopUp1.transform.position - Camera.main.transform.position).magnitude;
                float disFromPopUp2 = (baseLinePopUp2.transform.position - Camera.main.transform.position).magnitude;
                if (disFromPopUp1 < disFromPopUp2)
                {
                    if(!baseLinePopUp1.activeSelf)
                    {
                        baseLinePopUp1.SetActive(true);
                        StartCoroutine(initAnimatePopUp(true));
                        baseLinePopUp2.SetActive(false);
                    }
                }
                else
                {
                    if (!baseLinePopUp2.activeSelf)
                    {
                        baseLinePopUp1.SetActive(false);
                        StartCoroutine(initAnimatePopUp(false));
                        baseLinePopUp2.SetActive(true);
                    }
                }
            }//if selected
        }//function : Update





        /*========================================================================================
         * All Animation Things are implemeted Here... 
         =======================================================================================*/

        public void animateBaselineHeading(float valueInc)
        {
            baselineHeading1.transform.localScale = new Vector3(baselineHeading1.transform.localScale.x, baselineHeading1.transform.localScale.y, valueInc);
            baselineHeading2.transform.localScale = new Vector3(baselineHeading1.transform.localScale.x, baselineHeading1.transform.localScale.y, valueInc);
        }//function : animateBaselineHeading(float valueInc)

        public IEnumerator animateBaseline()
        {
            float b1z = baselineHeading1.transform.localScale.z;
            float b2z = baselineHeading2.transform.localScale.z;
            yield return GraphController.animate(new processAnimate(animateBaselineHeading), 0.5f, 0.0f, baselineHeading1.transform.localScale.z);
            baselineHeading1.transform.localScale = new Vector3(baselineHeading1.transform.localScale.x, baselineHeading1.transform.localScale.y, b1z);
            baselineHeading2.transform.localScale = new Vector3(baselineHeading1.transform.localScale.x, baselineHeading1.transform.localScale.y, b2z);
        }//function : animateBaseline()

        public void animatePopUp1(float valueInc)
        {
            baseLinePopUp1Stick.transform.localScale = new Vector3(baseLinePopUp1Stick.transform.localScale.x, valueInc, baseLinePopUp1Stick.transform.localScale.z);
            baseLinePopUp1Board.transform.localPosition = new Vector3(baseLinePopUp1Board.transform.localPosition.x, valueInc - 0.01f, baseLinePopUp1Board.transform.localPosition.z);
        }//function : animatePopUp1(float valueInc)

        public void animatePopUp2(float valueInc)
        {
            baseLinePopUp2Stick.transform.localScale = new Vector3(baseLinePopUp2Stick.transform.localScale.x, valueInc, baseLinePopUp2Stick.transform.localScale.z);
            baseLinePopUp2Board.transform.localPosition = new Vector3(baseLinePopUp2Board.transform.localPosition.x, valueInc - 0.01f, baseLinePopUp2Board.transform.localPosition.z);
        }//function : animatePopUp2(float valueInc)

        public void animatePopUpScale1(float valueInc)
        {
            baseLinePopUp1BoardInfo.transform.localScale = new Vector3(valueInc, valueInc, 1.0f);
        }//function : animatePopUpScale1(float valueInc)

        public void animatePopUpScale2(float valueInc)
        {
            baseLinePopUp2BoardInfo.transform.localScale = new Vector3(valueInc, valueInc, 1.0f);
        }//function : animatePopUpScale2(float valueInc)

        public void animateDashline1Scale(float valueInc)
        {
            baseLineDashLine1.transform.localScale = new Vector3(valueInc, baseLineDashLine1.transform.localScale.y, baseLineDashLine1.transform.localScale.z);
            baseLineDashLine1.transform.localPosition = new Vector3(-dashLineLength * (1 - valueInc), baseLineDashLine1.transform.localPosition.y, baseLineDashLine1.transform.localPosition.z);
            baseLinePopUp1Board.transform.localPosition = new Vector3(-dashLineLength * (1 - valueInc), baseLinePopUp1Board.transform.localPosition.y, baseLinePopUp1Board.transform.localPosition.z);
        }//function : animatePopUpScale(float valueInc)

        public void animateDashline2Scale(float valueInc)
        {
            baseLineDashLine2.transform.localScale = new Vector3(valueInc, baseLineDashLine2.transform.localScale.y, baseLineDashLine2.transform.localScale.z);
            baseLineDashLine2.transform.localPosition = new Vector3(- dashLineLength * (1 - valueInc), baseLineDashLine2.transform.localPosition.y, baseLineDashLine2.transform.localPosition.z);
            baseLinePopUp2Board.transform.localPosition = new Vector3(- dashLineLength * (1 - valueInc), baseLinePopUp2Board.transform.localPosition.y, baseLinePopUp2Board.transform.localPosition.z);
        }//function : animatePopUpScale(float valueInc)

        public IEnumerator initAnimatePopUp(bool isbaselineHeading1)
        {
            if(isbaselineHeading1)
            {
                baseLinePopUp1Board.transform.localPosition = new Vector3(- dashLineLength, baseLinePopUp1Board.transform.localPosition.y, baseLinePopUp1Board.transform.localPosition.z);
                baseLineDashLine1.transform.localPosition = new Vector3(-dashLineLength, baseLineDashLine1.transform.localPosition.y, baseLineDashLine1.transform.localPosition.z);

                StartCoroutine(GraphController.animate(new processAnimate(animateDashline1Scale), 0.3f, 0.0f, 1.0f));
                yield return GraphController.animate(new processAnimate(animatePopUp1), 0.3f, 0.0f, infoPopUpMarginFromBaseLine);

                baseLineDashLine1.transform.localScale = new Vector3(1.0f, baseLineDashLine1.transform.localScale.y, baseLineDashLine1.transform.localScale.z);
                baseLineDashLine1.transform.localPosition = new Vector3(0.0f, baseLineDashLine1.transform.localPosition.y, baseLineDashLine1.transform.localPosition.z);
                baseLinePopUp1Board.transform.localPosition = new Vector3(0.0f, baseLinePopUp1Board.transform.localPosition.y, baseLinePopUp1Board.transform.localPosition.z);

                baseLinePopUp1Stick.transform.localScale = new Vector3(baseLinePopUp1Stick.transform.localScale.x, infoPopUpMarginFromBaseLine, baseLinePopUp1Stick.transform.localScale.z);
                baseLinePopUp1Board.transform.localPosition = new Vector3(baseLinePopUp1Board.transform.localPosition.x, infoPopUpMarginFromBaseLine - 0.01f, baseLinePopUp1Board.transform.localPosition.z);

                yield return GraphController.animate(new processAnimate(animatePopUpScale1), 0.08f, 1.0f, 1.2f);
                yield return GraphController.animate(new processAnimate(animatePopUpScale1), 0.08f, 1.2f, 1.0f);
                baseLinePopUp1BoardInfo.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else
            {
                baseLinePopUp2Board.transform.localPosition = new Vector3(-dashLineLength, baseLinePopUp2Board.transform.localPosition.y, baseLinePopUp2Board.transform.localPosition.z);
                baseLineDashLine2.transform.localPosition = new Vector3(-dashLineLength, baseLineDashLine2.transform.localPosition.y, baseLineDashLine2.transform.localPosition.z);

                StartCoroutine(GraphController.animate(new processAnimate(animateDashline2Scale), 0.3f, 0.0f, 1.0f));
                yield return GraphController.animate(new processAnimate(animatePopUp2), 0.3f, 0.0f, infoPopUpMarginFromBaseLine);

                baseLineDashLine2.transform.localScale = new Vector3(1.0f, baseLineDashLine2.transform.localScale.y, baseLineDashLine2.transform.localScale.z);
                baseLineDashLine2.transform.localPosition = new Vector3(0.0f, baseLineDashLine1.transform.localPosition.y, baseLineDashLine1.transform.localPosition.z);
                baseLinePopUp2Board.transform.localPosition = new Vector3(0.0f, baseLinePopUp2Board.transform.localPosition.y, baseLinePopUp2Board.transform.localPosition.z);

                baseLinePopUp2Stick.transform.localScale = new Vector3(baseLinePopUp2Stick.transform.localScale.x, infoPopUpMarginFromBaseLine, baseLinePopUp2Stick.transform.localScale.z);
                baseLinePopUp2Board.transform.localPosition = new Vector3(baseLinePopUp2Board.transform.localPosition.x, infoPopUpMarginFromBaseLine - 0.01f, baseLinePopUp2Board.transform.localPosition.z);

                yield return GraphController.animate(new processAnimate(animatePopUpScale2), 0.08f, 1.0f, 1.2f);
                yield return GraphController.animate(new processAnimate(animatePopUpScale2), 0.08f, 1.2f, 1.0f);
                baseLinePopUp1BoardInfo.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }//function : initAnimatePopUp(bool isbaselineHeading1)


        


        /*========================================================================================
         * All Events are implemeted Here... 
         =======================================================================================*/

        public void onFocus()
        {
            if (!focused && !selected && !highlited)
            {
                baselineMesh.GetComponentInChildren<MeshRenderer>().material = BaseLineFocusMat;
                baselineHeading1.GetComponentInChildren<MeshRenderer>().material = BaseLineFocusMat;
                baselineHeading2.GetComponentInChildren<MeshRenderer>().material = BaseLineFocusMat;
                focused = true;
            }// if !focused && !selected
        }//function : onFocus()

        public void onUnFocus()
        {
            if(focused && !selected && !highlited)
            {
                if (isZAxis)
                {
                    baselineMesh.GetComponentInChildren<MeshRenderer>().material = BaseLineZMat;
                    baselineHeading1.GetComponentInChildren<MeshRenderer>().material = BaseLineZMat;
                    baselineHeading2.GetComponentInChildren<MeshRenderer>().material = BaseLineZMat;
                }
                else
                {
                    baselineMesh.GetComponentInChildren<MeshRenderer>().material = BaseLineXMat;
                    baselineHeading1.GetComponentInChildren<MeshRenderer>().material = BaseLineXMat;
                    baselineHeading2.GetComponentInChildren<MeshRenderer>().material = BaseLineXMat;
                }
                focused = false;
            }//if focused
        }//function : onUnFocus()

        public void onSelect()
        {
            if (!selected)
            {
                if(!graph.isMultiSelectionMode)
                {
                    Graph.unSelectEverything();
                    baselineMesh.GetComponentInChildren<MeshRenderer>().material = BaseLineSelectMat;
                    baselineHeading1.GetComponentInChildren<MeshRenderer>().material = BaseLineSelectMat;
                    baselineHeading2.GetComponentInChildren<MeshRenderer>().material = BaseLineSelectMat;
                    if(isPopUpInfoAvailable)
                        baselineCanvas.SetActive(true);
                }

                if (isZAxis)
                {
                    for (int i = 0; i < graph.XAxis[graph.curXAxis].totalSub; i++)
                    {
                        graph.Bar[graph.curBid][subID, i].GetComponent<BarManager>().onGSelect();
                    }
                }
                else
                {
                    for (int i = 0; i < graph.ZAxis[graph.curZAxis].totalSub; i++)
                    {
                        graph.Bar[graph.curBid][i, subID].GetComponent<BarManager>().onGSelect();
                    }
                }

                if (!graph.isMultiSelectionMode)
                {
                    GraphController.lastSelectedSub = this;
                    selected = true;
                }
            }//if !selected
            else
            {
                onUnSelect();
            }
        }//function : onSelect()

        public void onUnSelect()
        {
            if (selected)
            {
                baseLinePopUp1.SetActive(false);
                baseLinePopUp2.SetActive(false);
                baselineCanvas.SetActive(false);

                if (isZAxis)
                {
                    baselineMesh.GetComponentInChildren<MeshRenderer>().material = BaseLineZMat;
                    baselineHeading1.GetComponentInChildren<MeshRenderer>().material = BaseLineZMat;
                    baselineHeading2.GetComponentInChildren<MeshRenderer>().material = BaseLineZMat;
                }
                else
                {
                    baselineMesh.GetComponentInChildren<MeshRenderer>().material = BaseLineXMat;
                    baselineHeading1.GetComponentInChildren<MeshRenderer>().material = BaseLineXMat;
                    baselineHeading2.GetComponentInChildren<MeshRenderer>().material = BaseLineXMat;
                }

                if (isZAxis)
                {
                    for (int i = 0; i < graph.XAxis[graph.curXAxis].totalSub; i++)
                    {
                        graph.Bar[graph.curBid][subID, i].GetComponent<BarManager>().onUnGSelect();
                    }
                }
                else
                {
                    for (int i = 0; i < graph.ZAxis[graph.curZAxis].totalSub; i++)
                    {
                        graph.Bar[graph.curBid][i, subID].GetComponent<BarManager>().onUnGSelect();
                    }
                }
                GraphController.lastSelectedSub = null;
                selected = false;
            }//if selected
        }//function : onUnSelect()

        public void onHighlite()
        {
            if(!highlited)
            {
                baselineMesh.GetComponentInChildren<MeshRenderer>().material = BaseLineSelectMat;
                baselineHeading1.GetComponentInChildren<MeshRenderer>().material = BaseLineSelectMat;
                baselineHeading2.GetComponentInChildren<MeshRenderer>().material = BaseLineSelectMat;
                highlited = true;
            }//if !highlited
        }//function : onHighlite()

        public void onUnHighlite()
        {
            if(highlited)
            {
                if (isZAxis)
                {
                    baselineMesh.GetComponentInChildren<MeshRenderer>().material = BaseLineZMat;
                    baselineHeading1.GetComponentInChildren<MeshRenderer>().material = BaseLineZMat;
                    baselineHeading2.GetComponentInChildren<MeshRenderer>().material = BaseLineZMat;
                }
                else
                {
                    baselineMesh.GetComponentInChildren<MeshRenderer>().material = BaseLineXMat;
                    baselineHeading1.GetComponentInChildren<MeshRenderer>().material = BaseLineXMat;
                    baselineHeading2.GetComponentInChildren<MeshRenderer>().material = BaseLineXMat;
                }
                highlited = false;
            }//if highlited
        }//function : onUnHighlite()

        public void onDClick()
        {
            //GraphController.MyDebug.text += "\n\n Sub BaseLine : onDClick";
        }//function : onDClick()

        public void onUnDClick()
        {
            //GraphController.MyDebug.text += "\n\n Sub BaseLine : onUnDClick";
        }//function : onUnDClick()

    }//class : SubBaseLineManager
}//namespace