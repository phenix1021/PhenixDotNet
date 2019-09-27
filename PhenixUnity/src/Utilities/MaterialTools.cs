using UnityEngine;
using System.Collections;
using Phenix.Unity.Pattern;
using Phenix.Unity.Extend;

namespace Phenix.Unity.Utilities
{
    public class MaterialTools : AloneSingleton<MaterialTools>
    {
        /// <summary>
        /// 颜色渐隐
        /// </summary>    
        public void FadeOut(Renderer renderer, string colorPropName, float fadeInTime,
            GameObject go, bool setActiveFalse = true)
        {
            if (renderer == null || go == null)
            {
                return;
            }
            StartCoroutine(FadeOutImpl(renderer, colorPropName, fadeInTime, go, setActiveFalse));
        }

        IEnumerator FadeOutImpl(Renderer renderer, string colorPropName, float fadeInTime,
            GameObject go, bool setActiveFalse)
        {
            Color color = Color.white;
            renderer.material.SetColor(colorPropName, color);

            while (fadeInTime > 0 && color.a > 0)
            {
                color.a -= Time.deltaTime / fadeInTime;
                color.a = Mathf.Max(color.a, 0);

                renderer.material.SetColor(colorPropName, color);
                yield return new WaitForEndOfFrame();
            }
            color.a = 0;
            renderer.material.SetColor(colorPropName, color);
            if (setActiveFalse)
            {
                go.SetActive(false);
            }
        }

        /// <summary>
        /// 颜色渐现
        /// </summary>    
        public void FadeIn(Renderer renderer, string colorPropName, float fadeInTime,
            GameObject go, bool setActiveTrue = true)
        {
            StartCoroutine(FadeInImpl(renderer, colorPropName, fadeInTime, go, setActiveTrue));
        }

        IEnumerator FadeInImpl(Renderer renderer, string colorPropName, float fadeInTime,
            GameObject go, bool setActiveTrue)
        {
            Color color = Color.black;
            renderer.material.SetColor(colorPropName, color);

            while (fadeInTime > 0 && color.a < 1)
            {
                color.a += Time.deltaTime / fadeInTime;
                color.a = Mathf.Min(color.a, 1);

                renderer.material.SetColor(colorPropName, color);
                yield return new WaitForEndOfFrame();
            }
            color.a = 1;
            renderer.material.SetColor(colorPropName, color);
            if (setActiveTrue)
            {
                go.SetActive(true);
            }
        }
    }
}