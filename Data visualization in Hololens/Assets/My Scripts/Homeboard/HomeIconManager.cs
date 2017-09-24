using Assets.My_Scripts.Tools;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace Assets.My_Scripts.Homeboard {
    public class HomeIconManager : GazeSelectionTarget {

        public GameObject DashboardGameObject;
        public GameObject HomeBoardGameObject;
        public Sprite DefaultSprite;
        public Sprite HighlightSprite;
        public ToolSounds ToolSoundsInstance;

        public override void OnGazeSelect() {
            Highlight();
        }

        private void Highlight() {
            ToolSoundsInstance.PlayHighlightSound();
            gameObject.GetComponent<SpriteRenderer>().sprite = HighlightSprite;
        }

        public override void OnGazeDeselect() {
            RemoveHighlight();
        }

        private void RemoveHighlight() {
            gameObject.GetComponent<SpriteRenderer>().sprite = DefaultSprite;
        }

        public override void OnTapped(InteractionSourceKind source, int tapCount, Ray ray) {
            OnSelect();
        }

        private void OnSelect() {
            DashboardGameObject.SetActive(true);
            HomeBoardGameObject.SetActive(false);
        }
    }
}
