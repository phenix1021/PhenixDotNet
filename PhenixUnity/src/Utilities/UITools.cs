using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Phenix.Unity.Pattern;
using Phenix.Unity.Extend;
using System.Collections;
using UnityEngine.Events;

namespace Phenix.Unity.Utilities
{
    public class UITools : StandAloneSingleton<UITools>
    {
        // pointer是否在当前UI对象(即调用本函数的UI对象)上
        public bool IsPointerOverUIGameObject()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        // pointer事件透传（从当前UI对象（即调用本函数的UI对象）向所覆盖的其它对象发射射线透传eventData）
        public void PassPointerEvent<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> func, GameObject target = null) 
            where T : IEventSystemHandler
        {
            List<RaycastResult> rlt = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, rlt);
            foreach (var item in rlt)
            {
                if (target && target != item.gameObject)
                {
                    // 如果不是指定目标
                    continue;
                }

                if (item.gameObject == eventData.pointerCurrentRaycast.gameObject)
                {
                    // 过滤当前对象，否则会死循环
                    continue;
                }

                ExecuteEvents.Execute(item.gameObject, eventData, func);
            }
        }

        public void PassPointerEventClick(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.pointerClickHandler, target);
        }

        public void PassPointerEventDown(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.pointerDownHandler, target);
        }

        public void PassPointerEventUp(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.pointerUpHandler, target);
        }

        public void PassPointerEventEnter(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.pointerEnterHandler, target);
        }

        public void PassPointerEventExit(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.pointerExitHandler, target);
        }

        public void PassPointerEventBeginDrag(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.beginDragHandler, target);
        }

        public void PassPointerEventDrag(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.dragHandler, target);
        }

        public void PassPointerEventEndDrag(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.endDragHandler, target);
        }

        public void PassPointerEventDrop(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.dropHandler, target);
        }

        public void PassPointerEventScroll(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.scrollHandler, target);
        }

        // Texture随机渐隐
        public void GradientFadeRand(RawImage rawImage, int speed, UnityAction onDone = null)
        {
            Texture2D tex = new Texture2D(rawImage.mainTexture.width, rawImage.mainTexture.height, TextureFormat.ARGB32, false);
            tex.SetPixels((rawImage.mainTexture as Texture2D).GetPixels());
            tex.Apply();
            rawImage.texture = tex;

            StartCoroutine(GradientFadeRandImpl(tex, speed, onDone));
        }

        IEnumerator GradientFadeRandImpl(Texture2D tex, int speed, UnityAction onDone)
        {
            int batch = speed * 1000;

            Color[] colors = tex.GetPixels();
            List<int> idxList = new List<int>();
            for (int i = 0; i < colors.Length; i++)
            {
                idxList.Add(i);
            }

            idxList.Shuffle(); // 乱序

            for (int i = 0; i < idxList.Count; i++)
            {
                colors[idxList[i]].a = 0;
                if (i > 0 && (i % batch == 0))
                {
                    tex.SetPixels(colors);
                    tex.Apply();
                    yield return new WaitForEndOfFrame();
                }
            }

            tex.SetPixels(colors);
            tex.Apply();

            if (onDone != null)
            {
                onDone.Invoke();
            }
        }

        // Texture垂直渐隐
        public void GradientFadeV(RawImage rawImage, int speed, UnityAction onDone = null)
        {
            Texture2D tex = new Texture2D(rawImage.mainTexture.width, rawImage.mainTexture.height, TextureFormat.ARGB32, false);
            tex.SetPixels((rawImage.mainTexture as Texture2D).GetPixels());
            tex.Apply();
            rawImage.texture = tex;

            StartCoroutine(GradientFadeVImpl(tex, speed, onDone));
        }

        IEnumerator GradientFadeVImpl(Texture2D tex, int speed, UnityAction onDone)
        {
            int colCount = tex.width;
            int rowCount = tex.height;

            for (int row = 0; row < rowCount; ++row)
            {
                for (int col = 0; col < colCount; ++col)
                {
                    Color color = tex.GetPixel(col, row);
                    color.a = 0;

                    tex.SetPixel(col, row, color);
                }

                if (row > 0 && (row % speed == 0))
                {
                    tex.Apply();
                    yield return new WaitForEndOfFrame();
                }
            }

            tex.Apply();

            if (onDone != null)
            {
                onDone.Invoke();
            }
        }

        // Texture水平渐隐
        public void GradientFadeH(RawImage rawImage, int speed, UnityAction onDone = null)
        {
            Texture2D tex = new Texture2D(rawImage.mainTexture.width, rawImage.mainTexture.height, TextureFormat.ARGB32, false);
            tex.SetPixels((rawImage.mainTexture as Texture2D).GetPixels());
            tex.Apply();
            rawImage.texture = tex;

            StartCoroutine(GradientFadeHImpl(tex, speed, onDone));
        }

        IEnumerator GradientFadeHImpl(Texture2D tex, int speed, UnityAction onDone)
        {
            int colCount = tex.width;
            int rowCount = tex.height;

            for (int col = 0; col < colCount; ++col)
            {
                for (int row = 0; row < rowCount; ++row)
                {
                    Color color = tex.GetPixel(col, row);
                    color.a = 0;

                    tex.SetPixel(col, row, color);
                }

                if (col > 0 && (col % speed == 0))
                {
                    tex.Apply();
                    yield return new WaitForEndOfFrame();
                }
            }

            tex.Apply();

            if (onDone != null)
            {
                onDone.Invoke();
            }
        }

        // Texture垂直百叶窗渐隐
        public void GradientFadeVShutter(RawImage rawImage, float duration, int colBatch, UnityAction onDone = null)
        {
            Texture2D tex = new Texture2D(rawImage.mainTexture.width, rawImage.mainTexture.height, TextureFormat.ARGB32, false);
            tex.SetPixels((rawImage.mainTexture as Texture2D).GetPixels());
            tex.Apply();
            rawImage.texture = tex;

            StartCoroutine(GradientFadeVShutterImpl(tex, duration, colBatch, onDone));
        }

        IEnumerator GradientFadeVShutterImpl(Texture2D tex, float duration, int colBatch, UnityAction onDone)
        {
            int colCount = tex.width;
            int rowCount = tex.height;
            int batchCount = rowCount / colBatch;

            for (int i = 0; i < colBatch; ++i)
            {
                for (int j = 0; j <= batchCount; ++j)
                {
                    for (int row = 0; row < rowCount; ++row)
                    {
                        int col = j * colBatch + i;
                        if (col >= colCount)
                        {
                            continue;
                        }

                        Color color = tex.GetPixel(col, row);
                        color.a = 0;

                        tex.SetPixel(col, row, color);
                    }
                }

                tex.Apply();
                yield return new WaitForSeconds(duration / colBatch);
            }

            if (onDone != null)
            {
                onDone.Invoke();
            }
        }

        // Texture水平百叶窗渐隐
        public void GradientFadeHShutter(RawImage rawImage, float duration, int rowBatch, UnityAction onDone = null)
        {
            Texture2D tex = new Texture2D(rawImage.mainTexture.width, rawImage.mainTexture.height, TextureFormat.ARGB32, false);
            tex.SetPixels((rawImage.mainTexture as Texture2D).GetPixels());
            tex.Apply();
            rawImage.texture = tex;

            StartCoroutine(GradientFadeHShutterImpl(tex, duration, rowBatch, onDone));
        }

        IEnumerator GradientFadeHShutterImpl(Texture2D tex, float duration, int rowBatch, UnityAction onDone)
        {
            int colCount = tex.width;
            int rowCount = tex.height;
            int batchCount = rowCount / rowBatch;

            for (int i = 0; i < rowBatch; ++i)
            {
                for (int j = 0; j <= batchCount; ++j)
                {
                    for (int col = 0; col < colCount; ++col)
                    {
                        int row = j * rowBatch + i;
                        if (row >= rowCount)
                        {
                            continue;
                        }

                        Color color = tex.GetPixel(col, row);
                        color.a = 0;

                        tex.SetPixel(col, row, color);
                    }
                }

                tex.Apply();
                yield return new WaitForSeconds(duration / rowBatch);
            }

            if (onDone != null)
            {
                onDone.Invoke();
            }
        }
    }
}