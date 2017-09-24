using Assets.My_Scripts.Tools;
using UnityEngine;

namespace Assets.My_Scripts.Utility {
    public class GraphCollider : MonoBehaviour {

        void OnTriggerEnter(Collider other) {
            if(other.GetComponent<ToolPanel>() != null)
                if (other.GetComponent<ToolPanel>().IsDummy)
                    GraphController.InsideGraphCollider = true;
        }

        void OnTriggerExit(Collider other) {
            if (other.GetComponent<ToolPanel>() != null)
                if (other.GetComponent<ToolPanel>().IsDummy)
                    GraphController.InsideGraphCollider = false;
        }

        void OnDisable() {
            GraphController.InsideGraphCollider = false;
        }

    }
}
