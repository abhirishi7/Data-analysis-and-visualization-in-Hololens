using System;
using UnityEngine;
using UnityEngine.VR.WSA.Input;



namespace Assets.My_Scripts
{
    public class GazeGestureManager : MonoBehaviour
    {
        public static GazeGestureManager Instance { get; private set; }

        // Represents the hologram that is currently being gazed at.
        public event Action<InteractionSourceKind, Vector3, Ray> InputUpdated;

        public GestureRecognizer recognizer;

        // Use this for initialization
        void Start()
        {
            Instance = this;

            // Set up a GestureRecognizer to detect Select gestures.
            recognizer = new GestureRecognizer();
            recognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.DoubleTap | GestureSettings.NavigationY | GestureSettings.NavigationX);
            recognizer.NavigationUpdatedEvent += OnNavigationUpdated;
            recognizer.StartCapturingGestures();
            recognizer.TappedEvent += (source, tapCount, ray) =>
            {
                if (WorldCursor.focusedGO != null)
                {
                    GazeSelectionTarget focusedGST = WorldCursor.focusedGO.GetComponent<GazeSelectionTarget>();
                    if (focusedGST != null)
                        focusedGST.OnTapped(source, tapCount, ray);
                }
                //GraphController.CurrentNavTool.GetComponent<Tool>().Select();
            };
            GraphController.CurrentNavTool.GetComponent<Tool>().Select();
        }//function : Start()

        public void OnNavigationUpdated(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            InputUpdated(source,relativePosition,ray);
        }//function : OnNavigationUpdated(InteractionSourceKind source, Vector3 relativePosition, Ray ray)

    }//class : GazeGestureManager
}//namespace