using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

namespace Assets.My_Scripts
{
    public class SupBaseLineClick : GazeSelectionTarget
    {
        public SupBaseLineManager SupParent;
        public static int tapCheck = 0;

        public override void OnGazeSelect()
        {
            SupParent.onFocus();
            //SupParent.onSelect();
        }//function : OnGazeSelect()

        public override void OnGazeDeselect()
        {
            SupParent.onUnFocus();
            //SupParent.onUnSelect();
        }//function : OnGazeDeSelect()

        public override void OnTapped(InteractionSourceKind source, int tapCount, Ray ray)
        {
            tapCheck = tapCount;
            if (tapCount == 2)
            {
                SupParent.onDClick();
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
                SupParent.onSelect();
            tapCheck = 0;
        }//function : waitForCheckDoubleClick()

    }//class : SupBaseLineClick
}//namespace