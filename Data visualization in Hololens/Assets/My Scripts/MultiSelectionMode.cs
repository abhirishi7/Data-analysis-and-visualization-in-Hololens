using UnityEngine;
using UnityEngine.VR.WSA.Input;
using UnityEngine.UI;

namespace Assets.My_Scripts
{
    public class MultiSelectionMode : GazeSelectionTarget
    {

        public Material DefaultMaterial;
        public Material HighlightMaterial;
        public Material SelectedMaterial;
        private MeshRenderer meshRenderer;

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
            if (!GraphController.CurrentActiveScene.GetComponent<Graph>().isMultiSelectionMode)
            {
                meshRenderer.material = HighlightMaterial;
            }
        }

        public override void OnGazeDeselect()
        {
            if (GraphController.CurrentActiveScene.GetComponent<Graph>().isMultiSelectionMode)
            {
                meshRenderer.material = SelectedMaterial;
            }
            else
            {
                meshRenderer.material = DefaultMaterial;
            }
        }

        public override void OnTapped(InteractionSourceKind source, int tapCount, Ray ray)
        {
            if (GraphController.CurrentActiveScene.GetComponent<Graph>().isMultiSelectionMode)
            {
                GraphController.CurrentActiveScene.GetComponent<Graph>().isMultiSelectionMode = false;
                meshRenderer.material = DefaultMaterial;
            }
            else
            {
                Graph.unSelectEverything();
                GraphController.CurrentActiveScene.GetComponent<Graph>().isMultiSelectionMode = true;
                meshRenderer.material = SelectedMaterial;
            }
        }//function  : OnTapped(InteractionSourceKind source, int tapCount, Ray ray)
    }//class : HideMode
}//namespace