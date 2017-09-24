using Assets.My_Scripts.Homeboard;
using UnityEngine;

namespace Assets.My_Scripts.Dashboard {
    public class DashboardController : MonoBehaviour {

        public GameObject[] ObjectsInScene;
        public GameObject DefaultActiveObject;
        public GameObject CurrActiveDashboardObject;
        public GameObject CurrActiveEntityObject;
        public GameObject CurrActiveTimelineObject;
        public GameObject CurrActiveVendorDivObject;
        public HomeBoardViewer HomeViewer;

        void Awake() {
            GraphController.CurrentActiveScene = DefaultActiveObject;
            GraphController.CurrentActiveDashboardButton = CurrActiveDashboardObject;
            GraphController.CurrentActiveEntityButton = CurrActiveEntityObject;
            GraphController.CurrentActiveTimelineButton = CurrActiveTimelineObject;
            GraphController.CurrentActiveVendorDivButton = CurrActiveVendorDivObject;
        }

        void OnEnable() {
            
        }
    }
}
