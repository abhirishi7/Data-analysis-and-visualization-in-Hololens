using System;
using System.Collections;
using UnityEngine;

namespace Assets.My_Scripts.Utility {
    public class TransitionUtility : MonoBehaviour {

        public static bool IsTransitionOver = false;

        public enum BlendMode {
            Opaque,
            Cutout,
            Fade,
            Transparent
        }


        public void GraphBaseFadeIn(Material baseMat, Material subLineZMat, Material subLineXMat) {
            StartCoroutine(BaseFadeIn(baseMat,subLineZMat,subLineXMat));
        }

        public IEnumerator BaseFadeIn(Material baseMat, Material subLineZMat, Material subLineXMat) {

            IsTransitionOver = false;
            var colorBase = baseMat.color;
            while (Math.Abs(baseMat.color.a) < 1f) {
                colorBase.a += 0.04f;
                baseMat.color = colorBase;
                yield return null;
            }
            SetMaterialRenderingMode(baseMat, BlendMode.Opaque);

            while (Math.Abs(subLineZMat.color.a) < 1f) {
                var colorSubLineZ = subLineZMat.color;
                colorSubLineZ.a += 0.04f;
                subLineZMat.color = colorSubLineZ;
                yield return null;
            }
            SetMaterialRenderingMode(subLineZMat, BlendMode.Opaque);

            while (Math.Abs(subLineXMat.color.a) < 1f) {
                var colorSubLineX = subLineXMat.color;
                colorSubLineX.a += 0.04f;
                subLineXMat.color = colorSubLineX;
                yield return null;
            }
            SetMaterialRenderingMode(subLineXMat, BlendMode.Opaque);
            IsTransitionOver = true;
        }

        public void GraphBaseReset(Material graphBaseMat, Material subLineZMat, Material subLineXMat) {
            var colorBase = graphBaseMat.color;
            colorBase.a = 0f;
            graphBaseMat.color = colorBase;
            var colorSubLineZ = subLineZMat.color;
            colorSubLineZ.a = 0f;
            subLineZMat.color = colorSubLineZ;
            var colorSubLineX = subLineXMat.color;
            colorSubLineX.a = 0f;
            subLineXMat.color = colorSubLineX;

            SetMaterialRenderingMode(subLineZMat, BlendMode.Fade);
            SetMaterialRenderingMode(subLineXMat, BlendMode.Fade);
            SetMaterialRenderingMode(graphBaseMat, BlendMode.Fade);
        }

        private static void SetMaterialRenderingMode(Material material, BlendMode blendMode) {
            switch (blendMode) {
                case BlendMode.Fade:
                    material.SetFloat("_Mode",2);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;

                case BlendMode.Opaque:
                    material.SetFloat("_Mode", 0);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
            }
        }

    }
}
