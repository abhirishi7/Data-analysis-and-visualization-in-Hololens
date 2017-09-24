using System.Collections;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace Assets.My_Scripts
{
    public class BarClick : GazeSelectionTarget
    {
        public BarManager BarParent;
        public static int tapCheck = 0;

        public override void OnGazeSelect()
        {
            BarParent.onFocus();
            //BarParent.onSelect();
        }//function : OnGazeSelect()

        public override void OnGazeDeselect()
        {
            BarParent.onUnFocus();
            //BarParent.onUnSelect();
        }//function : OnGazeDeSelect()

        public override void OnTapped(InteractionSourceKind source, int tapCount, Ray ray)
        {
            tapCheck = tapCount;
            if (tapCount == 2)
            {
                BarParent.onDClick();
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
            {
                BarParent.onSelect();
            }
            tapCheck = 0;
        }//function : waitForCheckDoubleClick()

    }//class : BarClick
}//namespace