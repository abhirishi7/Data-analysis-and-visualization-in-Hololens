using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

namespace Assets.My_Scripts
{
    public class SubBaseLineClick : GazeSelectionTarget
    {
        public SubBaseLineManager SubParent;
        public static int tapCheck = 0;

        public override void OnGazeSelect()
        {
            SubParent.onFocus();
            //SubParent.onSelect();
        }//function : OnGazeSelect()

        public override void OnGazeDeselect()
        {
            SubParent.onUnFocus();
            //SubParent.onUnSelect();
        }//function : OnGazeDeSelect()

        public override void OnTapped(InteractionSourceKind source, int tapCount, Ray ray)
        {
            tapCheck = tapCount;
            if (tapCount == 2)
            {
                SubParent.onDClick();
                tapCheck = 0;
            }
            else if (tapCount == 1)
            {
                StartCoroutine(waitForCheckDoubleClick());
            }
        }//function : OnTapped(InteractionSourceKind source, int tapCount, Ray ray)

        public IEnumerator waitForCheckDoubleClick()
        {
            yield return new WaitForSeconds(0.25f);
            if (tapCheck == 1)
                SubParent.onSelect();
            tapCheck = 0;
        }//function : waitForCheckDoubleClick()

    }//class : SubBaseLineClick
}//namespace