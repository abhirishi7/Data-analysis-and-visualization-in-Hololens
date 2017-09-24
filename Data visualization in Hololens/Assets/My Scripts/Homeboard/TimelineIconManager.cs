using Assets.My_Scripts.Tools;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace Assets.My_Scripts.Homeboard {
    public class TimelineIconManager : GazeSelectionTarget {

        public bool IsYear = false;
        public bool IsQuat15 = false;
        public bool IsQuat16 = false;
        public bool IsMonth15 = false;
        public bool IsMonth16 = false;
        public Sprite DefaultSprite;
        public Sprite HighlightSprite;
        public Sprite SelectedSprite;

        public GameObject QuarterObject;
        public GameObject MonthObject;
        public Sprite MonthDefaultSprite;
        public Sprite QuatDefaultSprite;
        public Sprite QuatSelectedSprite;
        public Sprite MonthSelectedSprite;

        public ToolSounds ToolSoundsInstance;

        public override void OnGazeSelect() {
            Highlight();
        }

        private void Highlight() {
            if (GraphController.CurrentActiveTimelineButton.Equals(gameObject))
                return;

            gameObject.GetComponent<SpriteRenderer>().sprite = HighlightSprite;
        }

        public override void OnGazeDeselect() {
            RemoveHighlight();
        }

        private void RemoveHighlight() {
            if (GraphController.CurrentActiveTimelineButton.Equals(gameObject))
                return;

            gameObject.GetComponent<SpriteRenderer>().sprite = DefaultSprite;
        }

        public override void OnTapped(InteractionSourceKind source, int tapCount, Ray ray) {
            OnSelect();
        }

        private void OnSelect() {
            if(GraphController.CurrentActiveTimelineButton.Equals(gameObject))
                return;

            ToolSoundsInstance.PlaySelectSound();
            GraphController.CurrentActiveTimelineButton.GetComponent<SpriteRenderer>().sprite =
              GraphController.CurrentActiveTimelineButton.GetComponent<TimelineIconManager>().DefaultSprite;

            GraphController.CurrentActiveTimelineButton = gameObject;
            gameObject.GetComponent<SpriteRenderer>().sprite = SelectedSprite;

            if (IsMonth15 || IsMonth16) {
                MonthObject.GetComponent<SpriteRenderer>().sprite = MonthSelectedSprite;
                QuarterObject.GetComponent<SpriteRenderer>().sprite = QuatDefaultSprite;
            }
            else if (IsQuat16 || IsQuat15) {
                QuarterObject.GetComponent<SpriteRenderer>().sprite = QuatSelectedSprite;
                MonthObject.GetComponent<SpriteRenderer>().sprite = MonthDefaultSprite;
            }
            else if (IsYear) {
                QuarterObject.GetComponent<SpriteRenderer>().sprite = QuatDefaultSprite;
                MonthObject.GetComponent<SpriteRenderer>().sprite = MonthDefaultSprite;
            }

            HomeController.IsYear = IsYear;
            HomeController.IsQuat15 = IsQuat15;
            HomeController.IsQuat16 = IsQuat16;
            HomeController.IsMonth15 = IsMonth15;
            HomeController.IsMonth16 = IsMonth16;
            GraphController.CurrentActiveScene.SetActive(false);
            HomeController.ModifyGraph();
        }
    }
}
