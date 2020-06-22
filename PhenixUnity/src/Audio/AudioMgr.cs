using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.Audio
{    
    public abstract class AudioMgr
    {   
        Dictionary<int/*AudioType*/, List<AudioClip>> _audios = new Dictionary<int, List<AudioClip>>();

        protected AudioSource audioSource;

        protected void Add(int audioType, AudioClip clip)
        {
            if (_audios.ContainsKey(audioType))
            {
                _audios[audioType].Add(clip);
                return;
            }

            _audios.Add(audioType, new List<AudioClip>() { clip });
        }

        AudioClip GetClip(int soundType)
        {
            if (_audios.ContainsKey(soundType) == false)
            {
                return null;
            }

            List<AudioClip> clips = _audios[soundType];
            return clips[Random.Range(0, clips.Count)];
        }

        /// <summary>
        /// 简单播一发
        /// </summary>    
        public Coroutine PlayOneShot(int soundType, float delay = 0)
        {            
            return AudioTools.Instance.PlayOneShot(audioSource, GetClip(soundType), delay);
        }

        /// <summary>
        /// 音量渐起
        /// </summary>    
        public void PlayIn(int soundType, float fadeInTime)
        {
            AudioTools.Instance.PlayIn(audioSource, GetClip(soundType), fadeInTime);
        }

        /// <summary>
        /// 音量渐落
        /// </summary>    
        public void PlayOut(int soundType, float fadeOutTime)
        {
            AudioTools.Instance.PlayOut(audioSource, GetClip(soundType), fadeOutTime);
        }

        /// <summary>
        /// 循环播放clip
        /// </summary>    
        public void PlayLoop(int soundType)
        {
            AudioTools.Instance.PlayLoop(audioSource, GetClip(soundType));
        }

        /// <summary>
        /// 在指定时长内循环播放clip
        /// </summary>    
        public void PlayLoop(int soundType, float time,
            float fadeInTime = 0, float fadeOutTime = 0)
        {
            AudioTools.Instance.PlayLoop(audioSource, GetClip(soundType), time, fadeInTime, fadeOutTime);
        }

        /// <summary>
        /// 过渡播放(当前clip渐落之后，新clip渐起)
        /// </summary>    
        public void PlayCross(int soundType, float fadeOutTime, float fadeInTime)
        {
            AudioTools.Instance.PlayCross(audioSource, GetClip(soundType), fadeOutTime, fadeInTime);
        }
    }
}