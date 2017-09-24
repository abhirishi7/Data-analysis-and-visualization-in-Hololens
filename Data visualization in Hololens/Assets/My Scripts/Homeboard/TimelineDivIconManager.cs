using Assets.My_Scripts.Tools;
using UnityEngine;

namespace Assets.My_Scripts.Homeboard {
    public class TimelineDivIconManager : GazeSelectionTarget {

        public bool IsQuat = false;
        public Sprite DefaultSprite;
        public Sprite HighlightSprite;
        public Sprite SelectedSprite;
        public GameObject[] TimelineBarObject;
        public Sprite BarHighlightSprite;
        public Sprite BarDefaultSprite;
        public GameObject[] TimelineYearObject;
        public ToolSounds ToolSoundsInstance;

        public override void OnGazeSelect() {
            Highlight();
        }

        private void Highlight() {
            var timeIconManager = GraphController.CurrentActiveTimelineButton.GetComponent<TimelineIconManager>();
            if (IsQuat) {
                if (timeIconManager.IsQuat15 || timeIconManager.IsQuat16)
                    return;
            }

            if (!IsQuat) {
                if (timeIconManager.IsMonth15 || timeIconManager.IsMonth16)
                    return;
            }

            gameObject.GetComponent<SpriteRenderer>().sprite = HighlightSprite;
            foreach (var bar in TimelineBarObject) {
                bar.GetComponent<SpriteRenderer>().sprite = BarHighlightSprite;
            }
            foreach (var divYear in TimelineYearObject) {
                divYear.GetComponent<SpriteRenderer>().sprite = divYear.GetComponent<TimelineIconManager>().HighlightSprite;
            }

        }

        public override void OnGazeDeselect() {
            RemoveHighlight();
        }

        private void RemoveHighlight() {
            var timeIconManager = GraphController.CurrentActiveTimelineButton.GetComponent<TimelineIconManager>();
            if (IsQuat) {
                if (timeIconManager.IsQuat15 || timeIconManager.IsQuat16)
                    return;
            }

            if (!IsQuat) {
                if (timeIconManager.IsMonth15 || timeIconManager.IsMonth16)
                    return;
            }

            gameObject.GetComponent<SpriteRenderer>().sprite = DefaultSprite;
            foreach (var bar in TimelineBarObject) {
                bar.GetComponent<SpriteRenderer>().sprite = BarDefaultSprite;
            }
            foreach (var divYear in TimelineYearObject) {
                divYear.GetComponent<SpriteRenderer>().sprite = divYear.GetComponent<TimelineIconManager>().DefaultSprite;
            }
        }

        //void OnDisable() {
        //    gameObject.GetComponent<SpriteRenderer>().sprite = DefaultSprite;
        //    foreach (var bar in TimelineBarObject) {
        //        bar.GetComponent<SpriteRenderer>().sprite = BarDefaultSprite;
        //    }
        //    foreach (var divYear in TimelineYearObject) {
        //        divYear.GetComponent<SpriteRenderer>().sprite = divYear.GetComponent<TimelineIconManager>().DefaultSprite;
        //    }
        //}

        public void RemoveHighlightSprite() {
            gameObject.GetComponent<SpriteRenderer>().sprite = DefaultSprite;
            foreach (var bar in TimelineBarObject) {
                bar.GetComponent<SpriteRenderer>().sprite = BarDefaultSprite;
            }
        }
    }
}
