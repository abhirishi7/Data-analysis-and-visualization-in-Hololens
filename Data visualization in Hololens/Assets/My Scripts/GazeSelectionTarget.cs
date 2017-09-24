using UnityEngine;
using UnityEngine.VR.WSA.Input;



namespace Assets.My_Scripts {
    public class GazeSelectionTarget : MonoBehaviour {

        public virtual void OnGazeSelect() {
        }

        public virtual void OnGazeDeselect() {
        }

        public virtual bool OnNavigationStarted(InteractionSourceKind source, Vector3 relativePosition, Ray ray) {
            return false;
        }

        public virtual bool OnNavigationUpdated(InteractionSourceKind source, Vector3 relativePosition, Ray ray) {
            return false;
        }

        public virtual bool OnNavigationCompleted(InteractionSourceKind source, Vector3 relativePosition, Ray ray) {
            return false;
        }

        public virtual bool OnNavigationCanceled(InteractionSourceKind source, Vector3 relativePosition, Ray ray) {
            return false;
        }

        public virtual void OnTapped(InteractionSourceKind source, int tapCount, Ray ray) {
        }
    }
}
