using System.Collections;
using System.IO;
using UnityEngine;
using Phenix.Unity.Pattern;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.Camera
{
    public class CameraEffect : AloneSingleton<CameraEffect>
    {
        /// <summary>
        /// 慢镜头特写
        /// </summary>        
        public Coroutine SlowMotion(UnityEngine.Camera camera, float timeScale, float fov, 
            float effectTime, float restoreDelayTime, float restoreTime)
        {
            if (camera == null)
            {
                return null;
            }
            StopCoroutine("SlowMotionImpl");
            return StartCoroutine(SlowMotionImpl(camera, timeScale, fov, effectTime, restoreDelayTime, restoreTime));
        }

        IEnumerator SlowMotionImpl(UnityEngine.Camera camera, float timeScale, float fov, 
            float effectTime, float restoreDelayTime, float restoreTime)
        {
            float curFovTime = 0;
            float fovTime = effectTime;
            float baseFov = camera.fieldOfView;
            float fovStart = camera.fieldOfView;
            float fovEnd = fov;
            bool  fovOK = false;

            float curSlowTime = 0;
            float slowTime = effectTime;
            float baseTimeScale = Time.timeScale;
            float timeScaleStart = Time.timeScale;
            float timeScaleEnd = timeScale;            
            bool slowOK = false;            

            yield return new WaitForEndOfFrame();

            bool inRestore = false; // 是否处于恢复过程

            while (fovOK == false || slowOK == false)
            {
                if (fovOK == false)
                {
                    curFovTime += Time.deltaTime;
                    if (curFovTime > fovTime)
                    {
                        curFovTime = fovTime;
                        fovOK = true;
                    }
                    camera.fieldOfView = MathTools.Hermite(fovStart, fovEnd, curFovTime / fovTime);
                }

                if (slowOK == false)
                {
                    curSlowTime += Time.deltaTime;
                    if (curSlowTime > slowTime)
                    {
                        curSlowTime = slowTime;
                        slowOK = true;
                    }
                    Time.timeScale = MathTools.Hermite(timeScaleStart, timeScaleEnd, curSlowTime / slowTime);                    
                }                

                yield return new WaitForEndOfFrame();

                if (fovOK && slowOK && inRestore == false)
                {
                    // 准备恢复
                    curFovTime = 0;
                    fovTime = restoreTime;                    
                    fovStart = fovEnd;
                    fovEnd = baseFov;
                    fovOK = false;

                    curSlowTime = 0;
                    slowTime = restoreTime;                    
                    timeScaleStart = timeScaleEnd;
                    timeScaleEnd = baseTimeScale;
                    slowOK = false;

                    inRestore = true;

                    yield return new WaitForSeconds(restoreDelayTime); // 延迟N秒开始恢复
                }
            }            
        }

        /// <summary>
        /// 镜头拍照
        /// </summary>        
        public Coroutine TakePicture(UnityEngine.Camera camera, string fileName)
        {
            if (camera == null)
            {
                return null;
            }
            StopCoroutine("TakePictureImpl");
            return StartCoroutine(TakePictureImpl(camera, fileName));
        }

        IEnumerator TakePictureImpl(UnityEngine.Camera camera, string fileName)
        {
            yield return new WaitForEndOfFrame();
            Texture2D tex = new Texture2D((int)camera.rect.width, (int)camera.rect.height, TextureFormat.ARGB32, false);
            tex.ReadPixels(camera.rect, 0, 0);
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            GameObject.Destroy(tex);
            File.WriteAllBytes(fileName + ".png", bytes);
        }
    }
}