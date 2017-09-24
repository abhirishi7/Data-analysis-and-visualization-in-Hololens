using UnityEngine;
using System.Collections;
using TMPro;


namespace Assets.My_Scripts
{

    public class BarManager : MonoBehaviour
    {
        public long value;
        public float shortValue;
        int totalLetter;
        float percentage = 0.0f;
        float valueAfterScale;
        public string unit = "M";

        Graph graph;
        int barID;
        public bool isGrowthValue;


        public GameObject BarMain;
        public GameObject BarChild;
        public GameObject BarValue;
        public GameObject BarUnit;
        public GameObject BarCanvas;
        public GameObject BarPopUpBoard;
        public GameObject BarPopUpBoardInfo;
        public GameObject BarPopUpStick;
        public static float infoPopUpMarginFromBar = 0.2f;
        public static float stickSize = 0.005f;
        Vector3 defaultPosPopUp;
        bool isPopUpInfoAvailable = true;

        private Material barFocusMat;
        private Material barNormalMat;
        private Material barSelectMat;
        private Material barGSelectMat;

        public Material growthIncMat;
        public Material growthDecMat;


        float bigFont = 0.7f;
        float smallFont;
        float heigthOnFocus = 0.06f;

        bool focused = false;
        bool selected = false;
        public bool gSelected = false;
        bool hide = false;

        public void setGraph(Graph Graph, int BarID)
        {
            graph = Graph;
            barID = BarID;
        }//function : setGraph(Graph Graph, int BarID, bool IsGrowthValue)

        public void setValue(long val)
        {
            value = val + (int)(val * Random.Range(-5.0f, 5.0f));
            if (value < 0)
                value = -value;
            if (!isGrowthValue && !unit.Equals("%"))
            {
                if (value > 3000000000)
                    value = (int)(value / Random.Range(1.5f, 4.0f));
            }
            else
            {
                if (value > 100)
                    value = value % 100;
            }
            //if (!isGrowthValue && !unit.Equals("%"))
            //    value = (int)(100000 * Random.Range(0.0f, 9999.0f));
            //else
            //    value = (int)Random.Range(0.0f, 100.0f);

            if (!isGrowthValue && !unit.Equals("%"))
            {
                if(DataManager.curTTLValue != 0)
                    percentage = (float)(value * 100) / DataManager.curTTLValue;

                BarPopUpBoardInfo.GetComponentInChildren<TextMeshPro>().text = "Value : \n" + value.ToString("N0") + "\n" +
                                                                           "% of Bussiness : \n" + percentage.ToString("F2") + "%\n"+
                                                                           "TTL : \n" + DataManager.curTTLValue.ToString("N0");
   
            }
            else
            {
                isPopUpInfoAvailable = false;
                BarPopUpBoardInfo.GetComponentInChildren<TextMeshPro>().text = "";
            }
        }//function : setValue(long val)

        public void setShortValue(int divide)
        {
            divide = 6;
            if (!isGrowthValue && unit != "%")
            {
                shortValue = value / Mathf.Pow(10, divide);
                if (divide == 6)
                    unit = "M";
                else if (divide == 3)
                    unit = "T";
                else if (divide == 2)
                    unit = "H";
            }
            else
            {
                shortValue = value;
                valueAfterScale /= Mathf.Pow(10, divide);
            }
        }//function : setShortValue(int divide)

        public void setUnit(string Unit)
        {
            this.unit = Unit;
        }//function : setUnit(string Unit)

        public void setValueAfterScale(float val)
        {
            valueAfterScale = val;
        }//function : setValueAfterScale(float val)

        public void setMat(Material nor, Material focus, Material sel)
        {
            barNormalMat = nor;
            barFocusMat = focus;
            barSelectMat = sel;
            barGSelectMat = sel;
            BarChild.GetComponent<MeshRenderer>().material = barNormalMat;
        }//function : setMat(Material nor, Material focus, Material sel)

        public IEnumerator Initialize()
        {
            if (!isGrowthValue)
            {
                valueAfterScale = shortValue / graph.barHeightScale[graph.curBid];
                if (valueAfterScale == 0.0f)
                    valueAfterScale = 0.00001f;
                BarMain.SetActive(true);

                yield return StartCoroutine(GraphController.animate(new processAnimate(animateBarScale), 0.6f, 0.0f, valueAfterScale + 0.1f));
                yield return StartCoroutine(GraphController.animate(new processAnimate(animateBarScale), 0.6f, valueAfterScale + 0.1f, valueAfterScale - 0.06f));
                yield return StartCoroutine(GraphController.animate(new processAnimate(animateBarScale), 0.6f, valueAfterScale - 0.06f, valueAfterScale));
                BarMain.transform.localScale = new Vector3(BarMain.transform.localScale.x, valueAfterScale, BarMain.transform.localScale.z);
            }
            else
            {
                if (shortValue < 0)
                {
                    shortValue = -shortValue;
                    BarMain.transform.localRotation = Quaternion.Euler(180.0f, 0.0f, 0.0f);

                    if (graph.graphName == "E Commerce")
                    {
                        isPopUpInfoAvailable = true;
                        BarPopUpBoardInfo.GetComponentInChildren<TextMeshPro>().text = "E-Commerce Decreased due to negative Twitter sentiment";
                    }
                    else if (graph.graphName == "Net Sales") {
                        isPopUpInfoAvailable = true;
                        BarPopUpBoardInfo.GetComponentInChildren<TextMeshPro>().text = "Net Sales Decreased due to devaluation of Yuan";
                    }
                }
                else
                {
                    if (graph.graphName == "E Commerce")
                    {
                        isPopUpInfoAvailable = true;
                        BarPopUpBoardInfo.GetComponentInChildren<TextMeshPro>().text = "E-Commerce Increased due to positive Twitter sentiment";
                    }
                }
                assignMaterialToGrowthBar();

                //Growth Bar Scale only depends on Z axis Width10
                float scaleX = BarMain.transform.localScale.x;
                BarMain.transform.localScale = new Vector3(scaleX, scaleX, scaleX);
                BarMain.SetActive(true);

                valueAfterScale /= graph.barHeightScale[graph.curBid];
                //valueAfterScale += scaleX;
                valueAfterScale /= 2;

                yield return StartCoroutine(GraphController.animate(new processAnimate(animateGrwothPosition), 1.8f, scaleX / 2.0f, valueAfterScale - (scaleX / 2.0f)));
                BarMain.transform.localPosition = new Vector3(0.0f, valueAfterScale - (scaleX / 2.0f), 0.0f);
            }


            totalLetter = ((int)shortValue).ToString().Length;
            BarValue.GetComponent<RectTransform>().sizeDelta = new Vector2((smallFont / 17.0f) * totalLetter, BarValue.GetComponent<RectTransform>().sizeDelta.y);
            BarValue.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, valueAfterScale, 0.0f);
            BarValue.GetComponent<TextMeshPro>().text = "" + (int)shortValue;
            BarUnit.GetComponent<TextMeshPro>().text = unit;
            BarValue.SetActive(true);

            //setting for PopUp
            BarPopUpBoard.transform.localPosition = defaultPosPopUp + new Vector3(0.0f, valueAfterScale, 0.0f);

            onUnFocus();
            yield return null;
        }//function : Initialize()

        public void OnDisable()
        {
            BarMain.transform.localScale = new Vector3(BarMain.transform.localScale.x, 0.0f, BarMain.transform.localScale.z);
            BarMain.SetActive(false);
            BarValue.SetActive(false);
        }//function : OnDisable()

        public void setChildScale(float x, float z)
        {
            BarMain.transform.localScale = new Vector3(x, 0.000001f, z);    //0.01 Minimum scale
            BarMain.SetActive(false);

            smallFont = x * 5.0f;
            BarValue.GetComponent<TextMeshPro>().fontSize = smallFont;
            BarUnit.GetComponent<TextMeshPro>().fontSize = smallFont/1.5f;
            BarValue.SetActive(false);

            setPopUp();
        }//function : setChildScale(float x, float z)

        public void setPopUp()
        {
            BarPopUpBoard.transform.localPosition = new Vector3(0.0f, infoPopUpMarginFromBar - 0.01f, 0.0f);

            defaultPosPopUp = BarPopUpBoard.transform.localPosition;
            BarPopUpStick.transform.localScale = new Vector3(stickSize, infoPopUpMarginFromBar, stickSize);

            BarCanvas.SetActive(false);
        }//function : setPopUp()

        public void assignMaterialToGrowthBar()
        {
            if (value < 0)
                BarChild.GetComponent<Renderer>().material = growthDecMat;
            else
                BarChild.GetComponent<Renderer>().material = growthIncMat;
        }//function : assignMaterialToGrowthBar()

        public void setHideBar(bool mode,int divide)
        {
            if(mode)
                setShortValue(divide);
            else
                setShortValue(graph.dividePow[graph.curBid]);

            if (!isGrowthValue)
            {
                if (mode)
                    valueAfterScale = shortValue / Graph.barHeightScaleTMP;
                else
                    valueAfterScale = shortValue / graph.barHeightScale[graph.curBid];

                if (valueAfterScale == 0.0f)
                    valueAfterScale = 0.00001f;
                BarMain.transform.localScale = new Vector3(BarMain.transform.localScale.x, valueAfterScale, BarMain.transform.localScale.z);
            }
            else
            {
                //Growth Bar Scale only depends on Z axis Width
                float scaleX = BarMain.transform.localScale.x;

                if (mode)
                {

                    float value16 = graph.Bar[graph.curBid][(int)((barID % 10000) / 100), ((int)(barID % 100)) - 1].GetComponent<BarManager>().value;
                    Debug.Log(":" + value16);
                    float value15 = graph.Bar[graph.curBid][(int)((barID % 10000) / 100), ((int)(barID % 100)) + 1].GetComponent<BarManager>().value;
                    Debug.Log(":::" + value15);
                    float diff = (value16 - value15) / 2.0f;

                    valueAfterScale = value15 + diff;
                    valueAfterScale /= Graph.barHeightScaleTMP;
                }
                else
                {
                    Debug.Log((barID % 10000) / 100 + "::" + (barID % 100));
                    float value16 = graph.Bar[graph.curBid][(barID % 10000) / 100, ((int)(barID % 100)) - 1].GetComponent<BarManager>().value;
                    Debug.Log(":" + value16);
                    float value15 = graph.Bar[graph.curBid][(int)((barID % 10000) / 100), ((int)(barID % 100)) + 1].GetComponent<BarManager>().value;
                    Debug.Log(":::" + value15);
                    float diff = (value16 - value15) / 2.0f;

                    valueAfterScale = value15 + diff;
                    valueAfterScale /= graph.barHeightScale[graph.curBid];
                }
                valueAfterScale += scaleX;
                BarMain.transform.localPosition = new Vector3(0.0f, valueAfterScale - (scaleX / 2.0f), 0.0f);
            }

            totalLetter = ((int)shortValue).ToString().Length;
            BarValue.GetComponent<RectTransform>().sizeDelta = new Vector2((smallFont / 17.0f) * totalLetter, BarValue.GetComponent<RectTransform>().sizeDelta.y);
            BarValue.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, valueAfterScale, 0.0f);
            BarValue.GetComponent<TextMeshPro>().text = "" + (int)shortValue;
            BarUnit.GetComponent<TextMeshPro>().text = unit;

            BarPopUpBoard.transform.localPosition = defaultPosPopUp + new Vector3(0.0f, valueAfterScale, 0.0f);

            if(mode)
            {
                onUnGSelect();
            }
            onUnFocus();
        }//funciton : setHideBar(bool mode)

        public void setActiveBar(bool mode)
        {
            hide = !mode;
            BarMain.SetActive(mode);
            BarValue.SetActive(mode);
        }//function : setActiveBar(bool mode)


        /*========================================================================================
         * All Animation Things are implemeted Here... 
         =======================================================================================*/

        public void animateBarScale(float valueInc)
        {
            BarMain.transform.localScale = new Vector3(BarMain.transform.localScale.x, valueInc, BarMain.transform.localScale.x);
        }//function : animateBarScale(float valueInc)

        public void animateGrwothPosition(float valueInc)
        {
            BarMain.transform.localPosition = new Vector3(BarMain.transform.localPosition.x, valueInc, BarMain.transform.localPosition.x);
        }//function : animateGrwothPosition(float valueInc)

        public void animatePopUp(float valueInc)
        {
            BarPopUpStick.transform.localScale = new Vector3(BarPopUpStick.transform.localScale.x, valueInc, BarPopUpStick.transform.localScale.z);
            BarPopUpBoard.transform.localPosition = new Vector3(BarPopUpBoard.transform.localPosition.x, valueAfterScale + valueInc - 0.01f, BarPopUpBoard.transform.localPosition.z);
        }//function : animatePopUp(float valueInc)

        public void animatePopUpScale(float valueInc)
        {
            BarPopUpBoardInfo.transform.localScale = new Vector3(valueInc, valueInc,1.0f);
        }//function : animatePopUpScale(float valueInc)

        public IEnumerator initAnimatePopUp()
        {
            yield return GraphController.animate(new processAnimate(animatePopUp), 0.3f, 0.0f, infoPopUpMarginFromBar);
            BarPopUpStick.transform.localScale = new Vector3(BarPopUpStick.transform.localScale.x, infoPopUpMarginFromBar, BarPopUpStick.transform.localScale.z);
            BarPopUpBoard.transform.localPosition = new Vector3(BarPopUpBoard.transform.localPosition.x, valueAfterScale + infoPopUpMarginFromBar - 0.01f, BarPopUpBoard.transform.localPosition.z);

            yield return GraphController.animate(new processAnimate(animatePopUpScale), 0.08f, 1.0f, 1.2f);
            yield return GraphController.animate(new processAnimate(animatePopUpScale), 0.08f, 1.2f, 1.0f);
            BarPopUpBoardInfo.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }//function : initAnimatePopUp()

        public void animateBarValue(float valueInc)
        {
            BarValue.GetComponent<TextMeshPro>().fontSize = valueInc;
            BarUnit.GetComponent<TextMeshPro>().fontSize = valueInc / 1.5f;
            BarValue.GetComponent<RectTransform>().sizeDelta = new Vector2((valueInc / 17.0f) * totalLetter, BarValue.GetComponent<RectTransform>().sizeDelta.y);
        }//function  :animateBarValue(float valueInc)

        public void animateBarValuePos(float valueInc)
        {
            BarValue.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, valueInc, 0.0f);
        }//funciton : animateBarValuePos(float valueInc)

        public IEnumerator initAnimateBarValue()
        {
            StartCoroutine(GraphController.animate(new processAnimate(animateBarValuePos), 0.08f, valueAfterScale, valueAfterScale + heigthOnFocus));
            yield return GraphController.animate(new processAnimate(animateBarValue), 0.16f, smallFont, bigFont);
            //yield return GraphController.animate(new processAnimate(animateBarValue), 0.16f, smallFont, bigFont + 0.2f);
            //yield return GraphController.animate(new processAnimate(animateBarValue), 0.08f, bigFont + 0.2f, bigFont);

            BarValue.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, valueAfterScale + heigthOnFocus, 0.0f);
            BarValue.GetComponent<TextMeshPro>().fontSize = bigFont;
            BarUnit.GetComponent<TextMeshPro>().fontSize = bigFont / 1.5f;
            BarValue.GetComponent<RectTransform>().sizeDelta = new Vector2((bigFont / 17.0f) * totalLetter, BarValue.GetComponent<RectTransform>().sizeDelta.y);

            if (!focused)
            {
                focused = true;
                onUnFocus();
            }
        }//function : initAnimateBarValue()



        /*========================================================================================
         * All Events are implemeted Here... 
         =======================================================================================*/

        public void onFocus()
        {
            if(!focused)
            {
                if(!selected && !gSelected)
                    BarChild.GetComponent<MeshRenderer>().material = barFocusMat;
                StartCoroutine(initAnimateBarValue());

                GraphController.lastFocusedBar = this;
                focused = true;
            }//if !focused && !selected
        }//function : onFocus()

        public void onUnFocus()
        {
            if(focused)
            {
                if (!selected && !gSelected)
                {
                    if (!isGrowthValue)
                        BarChild.GetComponent<MeshRenderer>().material = barNormalMat;
                    else
                        assignMaterialToGrowthBar();
                }

                BarValue.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, valueAfterScale, 0.0f);
                BarValue.GetComponent<TextMeshPro>().fontSize = smallFont;
                BarUnit.GetComponent<TextMeshPro>().fontSize = smallFont / 1.5f;
                BarValue.GetComponent<RectTransform>().sizeDelta = new Vector2((smallFont / 17.0f) * totalLetter, BarValue.GetComponent<RectTransform>().sizeDelta.y);
                focused = false;
            }//if focused
        }//function : onUnFocus()

        public void onSelect()
        {
            if(graph.isMultiSelectionMode)
            {
                onGSelect();
            }
            else
            {
                if (!selected)
                {
                    Graph.unSelectEverything();
                    BarChild.GetComponent<MeshRenderer>().material = barSelectMat;

                    if(isPopUpInfoAvailable)
                    {
                        BarCanvas.SetActive(true);
                        StartCoroutine(initAnimatePopUp());
                    }

                    int id;
                    id = (barID % 10000) / 100;
                    graph.ZAxis[graph.curZAxis].baseLine[id].GetComponent<SubBaseLineManager>().onHighlite();
                    id = barID % 100;
                    graph.XAxis[graph.curXAxis].baseLine[id].GetComponent<SubBaseLineManager>().onHighlite();

                    GraphController.lastSelectedBar = this;
                    selected = true;
                }//if !selected
                else
                {
                    onUnSelect();
                }//else !selected
            }//else GraphController.multiSelectionMode
        }//function : onSelect()

        public void onUnSelect()
        {
            if(selected)
            {
                if (!isGrowthValue)
                    BarChild.GetComponent<MeshRenderer>().material = barNormalMat;
                else
                    assignMaterialToGrowthBar();

                BarCanvas.SetActive(false);

                int id;
                id = (barID % 10000) / 100;
                graph.ZAxis[graph.curZAxis].baseLine[id].GetComponent<SubBaseLineManager>().onUnHighlite();
                id = barID % 100;
                graph.XAxis[graph.curXAxis].baseLine[id].GetComponent<SubBaseLineManager>().onUnHighlite();

                GraphController.lastSelectedBar = null;
                selected = false;
            }//if selected
        }//function : onUnSelect()

        public void onGSelect()
        {
            if (!gSelected)
            {
                if (!hide)
                {
                    BarChild.GetComponent<MeshRenderer>().material = barGSelectMat;
                    gSelected = true;
                }
            }
            else
            {
                onUnGSelect();
            }
        }//function : onGSelect()

        public void onUnGSelect()
        {
            if(gSelected)
            {
                if (!isGrowthValue)
                    BarChild.GetComponent<MeshRenderer>().material = barNormalMat;
                else
                    assignMaterialToGrowthBar();
                gSelected = false;
            }
        }//function : onUnGSelect()

        public void onDClick()
        {
            //if (!dClicked)
            //{
            //    if (Graph.lastDClickedBar != null)
            //    {
            //        Graph.lastDClickedBar.onUnDClickBar();
            //    }

            //    BarChild.GetComponent<MeshRenderer>().material = barDoubleClickMat;
            //    Graph.lastDClickedBar = this;
            //    dClicked = true;
            //}
            //else
            //{
            //    onUnDClickBar();
            //    Graph.lastDClickedBar = null;
            //}
        }//function : onDClick()

        public void onUnDClick()
        {
            //BarChild.GetComponent<MeshRenderer>().material = barNormalMat;
            //dClicked = false;
        }//function : onUnDClick()

    }//class : BarManager
}//namespace
