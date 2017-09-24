using Assets.My_Scripts.Tools;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace Assets.My_Scripts.Homeboard {
    public class EntityIconManager : GazeSelectionTarget {

        public bool IsEntityBrand = false;
        public bool IsEntityDivision = false;
        public Sprite DefaultSprite;
        public Sprite HighlightSprite;
        public Sprite SelectedSprite;
        public ToolSounds ToolSoundsInstance;

        public override void OnGazeSelect() {
            Highlight();
        }

        private void Highlight() {
            if (GraphController.CurrentActiveEntityButton.Equals(gameObject))
                return;

            //ToolSoundsInstance.PlayHighlightSound();
            gameObject.GetComponent<SpriteRenderer>().sprite = HighlightSprite;
        }

        public override void OnGazeDeselect() {
            RemoveHighlight();
        }

        private void RemoveHighlight() {
            if (GraphController.CurrentActiveEntityButton.Equals(gameObject))
                return;

            gameObject.GetComponent<SpriteRenderer>().sprite = DefaultSprite;
        }

        public override void OnTapped(InteractionSourceKind source, int tapCount, Ray ray) {
           OnSelect();
        }

        private void OnSelect() {

            if (GraphController.CurrentActiveEntityButton.Equals(gameObject))
                return;

            ToolSoundsInstance.PlaySelectSound();
            GraphController.CurrentActiveEntityButton.GetComponent<SpriteRenderer>().sprite =
              GraphController.CurrentActiveEntityButton.GetComponent<EntityIconManager>().DefaultSprite;

            GraphController.CurrentActiveEntityButton = gameObject;

            gameObject.GetComponent<SpriteRenderer>().sprite = SelectedSprite;
            HomeController.IsBrand = IsEntityBrand;
            HomeController.IsDivision = IsEntityDivision;
            GraphController.CurrentActiveScene.SetActive(false);
            HomeController.ModifyGraph();
        }
    }
}
