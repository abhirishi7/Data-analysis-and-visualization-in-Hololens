using System.Collections;
using UnityEngine;




namespace Assets.My_Scripts
{

    public class WorldCursor : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        public Material ActiveMaterial;
        public Material DisActiveMaterial;

        public float maxGazeDistance;
        public LayerMask raycastLayerMask = Physics.DefaultRaycastLayers;
        GazeStabilizer gazeStabilizer;
        Vector3 gazeOrigin;
        Vector3 gazeDirection;


        public static GameObject focusedGO;
        GazeSelectionTarget focusedGST = null;
        GazeSelectionTarget lastFocusedGST = null;


        bool timeOver;
        void Start()
        {
            meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
            meshRenderer.enabled = true;
            lastFocusedGST = null;

            gazeStabilizer = GetComponent<GazeStabilizer>();

        }//function : Start()

        void Update()
        {
            gazeOrigin = Camera.main.transform.position;
            gazeDirection = Camera.main.transform.forward;

            gazeStabilizer.UpdateHeadStability(gazeOrigin, Camera.main.transform.rotation);
            gazeOrigin = gazeStabilizer.StableHeadPosition;

            updateRayCast(); 
        }//function : Update()

        public void updateRayCast()
        {
            Vector3 endPos;
            RaycastHit hitInfo;
            //bool hit = Physics.SphereCast(gazeOrigin, 0.02f,gazeDirection, out hitInfo, maxGazeDistance, raycastLayerMask);
            bool hit = Physics.Raycast(gazeOrigin, gazeDirection, out hitInfo, maxGazeDistance, raycastLayerMask);
            if (hit && hitInfo.collider!=null)
            {
                focusedGO = hitInfo.collider.gameObject;
                focusedGST = focusedGO.GetComponent<GazeSelectionTarget>();
                if (focusedGST == null)
                {
                    if (lastFocusedGST != null)
                        lastFocusedGST.OnGazeDeselect();
                    lastFocusedGST = focusedGST;
                }//When Gaze Focus On New Mesh with no GazeSelectionTarget
                else if(focusedGST != lastFocusedGST)
                {
                    if (lastFocusedGST != null)
                        lastFocusedGST.OnGazeDeselect();
                    lastFocusedGST = focusedGST;
                    focusedGST.OnGazeSelect();
                }//When Gaze Focus On New Mesh with GazeSelectionTarget

                meshRenderer.material = ActiveMaterial;

                this.transform.up = hitInfo.normal;
                //this.transform.position = focusedGo.transform.position;
                //this.transform.position = hitInfo.point
                endPos = hitInfo.point;
            }
            else
            {
                if (focusedGST != null)
                {
                    focusedGST.OnGazeDeselect();
                }//When Gaze Focus Go On NoMesh
                focusedGO = null;
                focusedGST = null;
                lastFocusedGST = null;

                meshRenderer.material = DisActiveMaterial;

                this.transform.up = gazeDirection;
                //this.transform.position = gazeOrigin + (gazeDirection * maxGazeDistance);
                endPos = gazeOrigin + (gazeDirection * maxGazeDistance);
            }

            float forwardDis;
            //forwardDis = ((transform.position - gazeOrigin).magnitude) - ((endPos - gazeOrigin).magnitude);
            //transform.position = Vector3.MoveTowards(this.transform.position,gazeOrigin, forwardDis);
            //Debug.Log("to cam pos: "+transform.position +"forward:"+forwardDis + "cam:"+gazeOrigin);
            forwardDis = (endPos - this.transform.position).magnitude;
            transform.position = Vector3.MoveTowards(this.transform.position, endPos, forwardDis/2.0f);
            //Debug.Log("to end pos: " + transform.position + "forward:"+forwardDis+" end:"+endPos);

        }//function : updateRaycast()

    }//class : WorldCursor
}//namespace