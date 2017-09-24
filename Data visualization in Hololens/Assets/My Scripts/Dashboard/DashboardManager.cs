using System.Collections;
using Assets.My_Scripts.Homeboard;
using Assets.My_Scripts.Tools;
using UnityEngine;
using UnityEngine.VR.WSA.Input;



namespace Assets.My_Scripts.Dashboard {
    public class DashboardManager : GazeSelectionTarget {

        public GameObject ObjectToActivate;
        public DashboardController DashboardController;
        public GameObject HomeboardGameObject;
        public GameObject DashboardGameObject;
        public Sprite DefaultSprite;
        public Sprite HighlightSprite;
        public Sprite SelectedSprite;
        public ToolSounds ToolSoundsInstance;

        public override void OnGazeSelect() {
            Highlight();
        }

        public override void OnGazeDeselect() {
            RemoveHighlight();
        }

        public override void OnTapped(InteractionSourceKind source, int tapCount, Ray ray) {
            StartCoroutine(OnSelect());
        }

        private void Highlight() {
            if (GraphController.CurrentActiveDashboardButton.Equals(gameObject))
                return;

            //ToolSoundsInstance.PlayHighlightSound();
            gameObject.GetComponent<SpriteRenderer>().sprite = HighlightSprite;
        }

        private void RemoveHighlight() {
            if (GraphController.CurrentActiveDashboardButton.Equals(gameObject))
                return;

            gameObject.GetComponent<SpriteRenderer>().sprite = DefaultSprite;
        }

        private IEnumerator OnSelect() {

            if (GraphController.CurrentActiveDashboardButton.Equals(gameObject)) {
                HomeboardGameObject.SetActive(true);
                DashboardGameObject.SetActive(false);
                yield break;
            }

            ToolSoundsInstance.PlaySelectSound();

            GraphController.CurrentActiveDashboardButton.GetComponent<SpriteRenderer>().sprite =
                GraphController.CurrentActiveDashboardButton.GetComponent<DashboardManager>().DefaultSprite;

            GraphController.CurrentActiveDashboardButton = gameObject;
            gameObject.GetComponent<SpriteRenderer>().sprite = SelectedSprite;

            yield return null;
           
            foreach (var sceneObject in DashboardController.ObjectsInScene) {
                sceneObject.SetActive(false);
            }
            GraphController.CurrentActiveScene = ObjectToActivate;
            GraphController.CurrentNavTool.GetComponent<Tool>().Unselect();
            GraphController.CurrentNavTool.GetComponent<Tool>().Select();
            ResetHomeController();
            HomeboardGameObject.SetActive(true);
            HomeController.ModifyGraph();
            DashboardGameObject.SetActive(false);
        }

        private void ResetHomeController() {

            DashboardController.HomeViewer.Reset();

            DashboardController.CurrActiveEntityObject.GetComponent<SpriteRenderer>().sprite =
                DashboardController.CurrActiveEntityObject.GetComponent<EntityIconManager>().SelectedSprite;

            DashboardController.CurrActiveTimelineObject.GetComponent<SpriteRenderer>().sprite =
                DashboardController.CurrActiveTimelineObject.GetComponent<TimelineIconManager>().SelectedSprite;

            DashboardController.CurrActiveVendorDivObject.GetComponent<SpriteRenderer>().sprite =
                DashboardController.CurrActiveVendorDivObject.GetComponent<VendorDivIconManager>().SelectedSprite;

            HomeController.IsBrand = false;
            HomeController.IsDivision = true;
            HomeController.IsMonth15 = false;
            HomeController.IsMonth16 = false;
            HomeController.IsQuat16 = false;
            HomeController.IsQuat15 = false;
            HomeController.IsYear = true;
            HomeController.IsAcd = false;
            HomeController.IsCpd = true;
            HomeController.IsLoreal = false;
            HomeController.IsLuxe = false;
            HomeController.IsPpd = false;

            GraphController.CurrentActiveEntityButton = DashboardController.CurrActiveEntityObject;
            GraphController.CurrentActiveTimelineButton = DashboardController.CurrActiveTimelineObject;
            GraphController.CurrentActiveVendorDivButton = DashboardController.CurrActiveVendorDivObject;
        }
    }
}
