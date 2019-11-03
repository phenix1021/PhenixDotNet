using UnityEngine;
using System.Collections;
using Phenix.Unity.Pattern;
using UnityEngine.Events;

namespace Phenix.Unity.Utilities
{
    public class MaterialTools : StandAloneSingleton<MaterialTools>
    {
        /// <summary>
        /// 颜色渐隐
        /// </summary>    
        public Coroutine FadeOut(Renderer renderer, string shaderColorProp, float fadeInTime,
            GameObject go, bool setActiveFalse = true, UnityAction onFinished = null)
        {
            if (renderer == null || go == null)
            {
                return null;
            }
            go.SetActive(true);
            return StartCoroutine(FadeOutImpl(renderer, shaderColorProp, fadeInTime, go, setActiveFalse, onFinished));
        }

        IEnumerator FadeOutImpl(Renderer renderer, string colorPropName, float fadeInTime,
            GameObject go, bool setActiveFalse, UnityAction onFinished)
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
            if (onFinished != null)
            {
                onFinished.Invoke();
            }
        }

        /// <summary>
        /// 颜色渐现
        /// </summary>    
        public Coroutine FadeIn(Renderer renderer, string colorPropName, float fadeInTime,
            GameObject go, bool setActiveTrue = true, UnityAction onFinished = null)
        {
            return StartCoroutine(FadeInImpl(renderer, colorPropName, fadeInTime, go, setActiveTrue, onFinished));
        }

        IEnumerator FadeInImpl(Renderer renderer, string colorPropName, float fadeInTime,
            GameObject go, bool setActiveTrue, UnityAction onFinished)
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
            if (onFinished != null)
            {
                onFinished.Invoke();
            }
        }
    }
}