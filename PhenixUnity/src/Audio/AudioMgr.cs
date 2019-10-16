using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.Audio
{    
    public abstract class AudioMgr
    {   
        Dictionary<int/*AudioType*/, AudioClip> _audios = new Dictionary<int, AudioClip>();

        protected AudioSource audioSource;

        protected void Add(int audioType, AudioClip clip)
        {
            _audios.Add(audioType, clip);
        }

        /// <summary>
        /// 简单播一发
        /// </summary>    
        public Coroutine PlayOneShot(int soundType, float delay = 0)
        {
            if (_audios.ContainsKey(soundType) == false)
            {
                return null;
            }
            return AudioTools.Instance.PlayOneShot(audioSource, _audios[soundType], delay);
        }

        /// <summary>
        /// 音量渐起
        /// </summary>    
        public void PlayIn(int soundType, float fadeInTime)
        {
            if (_audios.ContainsKey(soundType) == false)
            {
                return;
            }

            AudioTools.Instance.PlayIn(audioSource, _audios[soundType], fadeInTime);
        }

        /// <summary>
        /// 音量渐落
        /// </summary>    
        public void PlayOut(int soundType, float fadeOutTime)
        {
            if (_audios.ContainsKey(soundType) == false)
            {
                return;
            }

            AudioTools.Instance.PlayOut(audioSource, _audios[soundType], fadeOutTime);
        }

        /// <summary>
        /// 循环播放clip
        /// </summary>    
        public void PlayLoop(int soundType)
        {
            if (_audios.ContainsKey(soundType) == false)
            {
                return;
            }
            AudioTools.Instance.PlayLoop(audioSource, _audios[soundType]);
        }

        /// <summary>
        /// 在指定时长内循环播放clip
        /// </summary>    
        public void PlayLoop(int soundType, float time,
            float fadeInTime = 0, float fadeOutTime = 0)
        {
            if (_audios.ContainsKey(soundType) == false)
            {
                return;
            }
            AudioTools.Instance.PlayLoop(audioSource, _audios[soundType], time, fadeInTime, fadeOutTime);
        }

        /// <summary>
        /// 过渡播放(当前clip渐落之后，新clip渐起)
        /// </summary>    
        public void PlayCross(int soundType, float fadeOutTime, float fadeInTime)
        {
            if (_audios.ContainsKey(soundType) == false)
            {
                return;
            }
            AudioTools.Instance.PlayCross(audioSource, _audios[soundType], fadeOutTime, fadeInTime);
        }
    }
}