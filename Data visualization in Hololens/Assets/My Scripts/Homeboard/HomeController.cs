using UnityEngine;

namespace Assets.My_Scripts.Homeboard {
    public class HomeController : MonoBehaviour {

        public static bool IsBrand = false;
        public static bool IsDivision = true;
        public static bool IsYear = true;
        public static bool IsQuat15 = false;
        public static bool IsQuat16 = false;
        public static bool IsMonth15 = false;
        public static bool IsMonth16 = false;
        public static bool IsCpd = true;
        public static bool IsLuxe = false;
        public static bool IsAcd = false;
        public static bool IsPpd = false;
        public static bool IsLoreal = false;

        public static void ModifyGraph() {
            var currentActiveObject = GraphController.CurrentActiveScene;
            ModifyCurrentObject(currentActiveObject);
        }

        private static void ModifyCurrentObject(GameObject currentActiveObject) {
            ModifyEntity(currentActiveObject);
            ModifyTimeline(currentActiveObject);
            currentActiveObject.SetActive(true); 
        }

        private static void ModifyTimeline(GameObject currentActiveObject) {
            if (IsYear)
                currentActiveObject.GetComponent<Graph>().curXAxis = 0;
            else if (IsQuat15) {
                currentActiveObject.GetComponent<Graph>().curXAxis = 1;
                currentActiveObject.GetComponent<Graph>().ShowYear = 15;
            }
            else if (IsQuat16) {
                currentActiveObject.GetComponent<Graph>().curXAxis = 1;
                currentActiveObject.GetComponent<Graph>().ShowYear = 16;
            }
            else if (IsMonth15) {
                currentActiveObject.GetComponent<Graph>().curXAxis = 2;
                currentActiveObject.GetComponent<Graph>().ShowYear = 15;
            }
            else if (IsMonth16) {
                currentActiveObject.GetComponent<Graph>().curXAxis = 2;
                currentActiveObject.GetComponent<Graph>().ShowYear = 16;
            }
        }

        private static void ModifyEntity(GameObject currentActiveObject) {
            if (HomeBoardViewer.IsEcommerceActive)
                return;

            if (HomeBoardViewer.IsVendorDivGroupActive)
                ModifyVendorDivGroup(currentActiveObject);
            else
                ModifyDivAndBrand(currentActiveObject);
        }

        private static void ModifyVendorDivGroup(GameObject currentActiveObject) {
            if (IsCpd)
                currentActiveObject.GetComponent<DataManagerVendor>().curDivision = 1;
            else if (IsLuxe)
                currentActiveObject.GetComponent<DataManagerVendor>().curDivision = 2;
            else if (IsAcd)
                currentActiveObject.GetComponent<DataManagerVendor>().curDivision = 3;
            else if (IsPpd)
                currentActiveObject.GetComponent<DataManagerVendor>().curDivision = 4;
            else if (IsLoreal)
                currentActiveObject.GetComponent<DataManagerVendor>().curDivision = 0;
            if(IsMonth15 || IsMonth16)
                currentActiveObject.GetComponent<SphereCollider>().radius = 1.03f;
            else {
                currentActiveObject.GetComponent<SphereCollider>().radius = 0.85f;
            }
        }

        private static void ModifyDivAndBrand(GameObject currentActiveObject) {
            if (IsDivision) {
                currentActiveObject.GetComponent<Graph>().curZAxis = 0;
                if (IsMonth15 || IsMonth16)
                    currentActiveObject.GetComponent<SphereCollider>().radius = 0.7f;
                else {
                    currentActiveObject.GetComponent<SphereCollider>().radius = 0.4f;
                }
            }
            else if (IsBrand) {
                currentActiveObject.GetComponent<Graph>().curZAxis = 1;
                if (IsMonth15 || IsMonth16)
                    currentActiveObject.GetComponent<SphereCollider>().radius = 1.2f;
                else {
                    currentActiveObject.GetComponent<SphereCollider>().radius = 1.05f;
                }
            }
        }
    }
}
