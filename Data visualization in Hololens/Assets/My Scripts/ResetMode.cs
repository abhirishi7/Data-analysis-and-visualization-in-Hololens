using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

namespace Assets.My_Scripts
{
    public class ResetMode : GazeSelectionTarget
    {
        public Material DefaultMaterial;
        public Material HighlightMaterial;
        private MeshRenderer meshRenderer;


        // Use this for initialization
        void Start()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            if (meshRenderer == null)
            {
                Debug.LogWarning(gameObject.name + " Tool has no renderer.");
            }
        }

        public override void OnGazeSelect()
        {
            meshRenderer.material = HighlightMaterial;
        }

        public override void OnGazeDeselect()
        {
            meshRenderer.material = DefaultMaterial;
        }

        public override void OnTapped(InteractionSourceKind source, int tapCount, Ray ray)
        {
            //this.GetComponent<Image>().color = new Color(0.56f,0.0f, 0.0f);
            GraphController.CurrentActiveScene.GetComponent<Graph>().resetAll();
        }//function  : OnTapped(InteractionSourceKind source, int tapCount, Ray ray)
    }//class : ResetMode
}//namespce