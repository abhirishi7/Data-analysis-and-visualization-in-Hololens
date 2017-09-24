using Assets.My_Scripts.Tools;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace Assets.My_Scripts.Homeboard {
    public class VendorDivIconManager : GazeSelectionTarget {

        public bool IsCpd = false;
        public bool IsLuxe = false;
        public bool IsAcd = false;
        public bool IsPpd = false;
        public bool IsLoreal = false;
        public Sprite DefaultSprite;
        public Sprite HighlightSprite;
        public Sprite SelectedSprite;
        public ToolSounds ToolSoundsInstance;

        public override void OnGazeSelect() {
            Highlight();
        }

        private void Highlight() {
            if (GraphController.CurrentActiveVendorDivButton.Equals(gameObject))
                return;

            //ToolSoundsInstance.PlayHighlightSound();
            gameObject.GetComponent<SpriteRenderer>().sprite = HighlightSprite;
        }

        public override void OnGazeDeselect() {
            RemoveHighlight();
        }

        private void RemoveHighlight() {
            if (GraphController.CurrentActiveVendorDivButton.Equals(gameObject))
                return;

            gameObject.GetComponent<SpriteRenderer>().sprite = DefaultSprite;
        }

        public override void OnTapped(InteractionSourceKind source, int tapCount, Ray ray) {
            OnSelect();
        }

        private void OnSelect() {
            if (GraphController.CurrentActiveVendorDivButton.Equals(gameObject))
                return;

            ToolSoundsInstance.PlaySelectSound();
            GraphController.CurrentActiveVendorDivButton.GetComponent<SpriteRenderer>().sprite =
               GraphController.CurrentActiveVendorDivButton.GetComponent<VendorDivIconManager>().DefaultSprite;

            GraphController.CurrentActiveVendorDivButton = gameObject;
            gameObject.GetComponent<SpriteRenderer>().sprite = SelectedSprite;
            HomeController.IsAcd = IsAcd;
            HomeController.IsCpd = IsCpd;
            HomeController.IsLuxe = IsLuxe;
            HomeController.IsPpd = IsPpd;
            HomeController.IsLoreal = IsLoreal;
            GraphController.CurrentActiveScene.SetActive(false);
            HomeController.ModifyGraph();
        }
    }
}
