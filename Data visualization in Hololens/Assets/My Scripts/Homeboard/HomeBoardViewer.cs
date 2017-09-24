using Assets.My_Scripts.Dashboard;
using UnityEngine;

namespace Assets.My_Scripts.Homeboard {
    public class HomeBoardViewer : MonoBehaviour {

        public DashboardController DashboardController;
        public GameObject BrandDivGroup;
        public GameObject VendorDivGroup;
        public GameObject HomeIcon;
        public GameObject Divider;
        public GameObject TimelineGroup;
        public static bool IsVendorDivGroupActive = false;
        public static bool IsEcommerceActive = false;

        void OnEnable() {
            ShowAndHideIcons();
        }

        private void ShowAndHideIcons() {
            var indexOfObjectActive = GetActiveObjectIndex();
            switch (indexOfObjectActive) {
                case 0:  //// overview scene
                    IsVendorDivGroupActive = false;
                    IsEcommerceActive = false;
                    BrandDivGroup.SetActive(false);
                    HomeIcon.SetActive(true);
                    Divider.SetActive(false);
                    TimelineGroup.SetActive(false);
                    VendorDivGroup.SetActive(false);
                    break;

                case 2:  //// e-commerce scene
                    IsVendorDivGroupActive = false;
                    IsEcommerceActive = true;
                    BrandDivGroup.SetActive(false);
                    HomeIcon.SetActive(true);
                    Divider.SetActive(false);
                    TimelineGroup.SetActive(false);
                    VendorDivGroup.SetActive(false);
                    break;

                case 1:  //// netsales scene
                case 3:  //// sellout scene
                    IsVendorDivGroupActive = false;
                    IsEcommerceActive = false;
                    BrandDivGroup.SetActive(true);
                    HomeIcon.SetActive(true);
                    Divider.SetActive(true);
                    TimelineGroup.SetActive(true);
                    VendorDivGroup.SetActive(false);
                    break;

                case 4:  //// channel scene
                    IsVendorDivGroupActive = false;
                    IsEcommerceActive = false;
                    BrandDivGroup.SetActive(false);
                    HomeIcon.SetActive(true);
                    Divider.SetActive(false);
                    TimelineGroup.SetActive(true);
                    VendorDivGroup.SetActive(false);
                    break;

                case 5:  //// vendor scene
                    IsVendorDivGroupActive = true;
                    IsEcommerceActive = false;
                    BrandDivGroup.SetActive(false);
                    HomeIcon.SetActive(true);
                    Divider.SetActive(true);
                    TimelineGroup.SetActive(true);
                    VendorDivGroup.SetActive(true);
                    break;
            }
        }

        private int GetActiveObjectIndex() {
            var index = 0;
            foreach (var sceneObject in DashboardController.ObjectsInScene) {
                if (sceneObject.Equals(GraphController.CurrentActiveScene))
                    return index;
                index++;
            }
            return 0;
        }

        public void Reset() {
            ResetBrandDivGroup();
            ResetTimelineDivGroup();
            ResetVendorDivGroup();
        }

        private void ResetTimelineDivGroup() {
            foreach (Transform child in TimelineGroup.transform) {
                if (child.gameObject.GetComponent<TimelineIconManager>() != null) {
                    child.gameObject.GetComponent<SpriteRenderer>().sprite =
                        child.gameObject.GetComponent<TimelineIconManager>().DefaultSprite;
                } else if (child.gameObject.GetComponent<TimelineDivIconManager>() != null) {
                    child.gameObject.GetComponent<TimelineDivIconManager>().RemoveHighlightSprite();
                }
            }
        }

        private void ResetVendorDivGroup() {
            foreach (Transform child in VendorDivGroup.transform) {
                if (child.gameObject.GetComponent<VendorDivIconManager>() != null) {
                    child.gameObject.GetComponent<SpriteRenderer>().sprite =
                        child.gameObject.GetComponent<VendorDivIconManager>().DefaultSprite;
                }
            }
        }

        private void ResetBrandDivGroup() {
            foreach (Transform child in BrandDivGroup.transform) {
                if (child.gameObject.GetComponent<EntityIconManager>() != null) {
                    child.gameObject.GetComponent<SpriteRenderer>().sprite =
                        child.gameObject.GetComponent<EntityIconManager>().DefaultSprite;
                }
            }
        }


    }
}
