using UnityEngine;
using Phenix.Unity.Pattern;
using System.Collections;
using UnityEngine.Events;

namespace Phenix.Unity.Utilities
{
    public class TimeTools : StandAloneSingleton<TimeTools>
    {     
        float _oriFixedDeltaTime;

        private void Awake()
        {
            _oriFixedDeltaTime = Time.fixedDeltaTime;       
        }

        /// <summary>
        /// 1. Update帧调用频率固定不变，不受Time.timeScale影响。Time.deltaTime值会与Time.timeScale同步变化。
        /// 近似可以认为Time.deltaTime（新） = Time.deltaTime（旧） * Time.timeScale，而实际update帧间间隔
        /// 时长 = Time.deltaTime / Time.timeScale。当Time.timeScale == 0时，update（以及lateUpdate）照常
        /// 触发，且此时Time.deltaTime == 0
        /// 2. fixedUpdate帧调用频率受Time.timeScale影响。但Time.fixedDeltaTime值不会与Time.timeScale同步变化。
        /// 实际FixedUpdate帧间间隔时长 = Time.fixedDeltaTime / Time.timeScale。当Time.timeScale == 0时，
        /// FixedUpdate不会触发。
        /// </summary>        
        public void ChangeTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
            // 同步修改Time.fixedDeltaTime，保证FixedUpdate和Update一样保持原有的函数触发频率
            Time.fixedDeltaTime = _oriFixedDeltaTime * Time.timeScale;
        }

        public Coroutine ChangeTimeScale(float timeScale, float time/*持续时间*/, UnityAction action)
        {
            ChangeTimeScale(timeScale);
            if (time <= 0)
            {                
                action.Invoke();
                return null;
            }            

            return StartCoroutine(ChangeTimeScaleImpl(time, action));
        }

        IEnumerator ChangeTimeScaleImpl(float time, UnityAction action)
        {            
            yield return new WaitForSecondsRealtime(time);
            action.Invoke();
        }

        public Coroutine ChangeTimeScale(float startScale, float endScale, float seconds)
        {
            if (seconds <= 0)
            {
                ChangeTimeScale(endScale);
                return null;
            }
            return StartCoroutine(ChangeTimeScaleImpl(startScale, endScale, seconds));
        }

        IEnumerator ChangeTimeScaleImpl(float startScale, float endScale, float seconds)
        {
            ChangeTimeScale(startScale);
            float startTimer = Time.realtimeSinceStartup;

            yield return new WaitForEndOfFrame();
            
            while (true)
            {
                float elapse = Time.realtimeSinceStartup - startTimer;
                if (elapse >= seconds)
                {
                    yield break;
                }
                float progress = Mathf.Clamp01(elapse / seconds);                
                ChangeTimeScale(Mathf.Lerp(startScale, endScale, progress));
                yield return new WaitForEndOfFrame();
            }            
        }
    }
}