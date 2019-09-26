using UnityEngine;
using System.Collections;
using Phenix.Unity.Pattern;

namespace Phenix.Unity.Utilities
{
    public class AudioTools : AloneSingleton<AudioTools>
    {
        /// <summary>
        /// 简单播一发
        /// </summary>    
        public void PlayOneShot(AudioSource audioSource, AudioClip clip)
        {            
            audioSource.volume = 1;
            audioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// 音量渐起
        /// </summary>    
        public void PlayIn(AudioSource audioSource, AudioClip clip, float fadeInTime)
        {            
            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.volume = 0;

            StartCoroutine(PlayInImpl(audioSource, fadeInTime));
        }

        IEnumerator PlayInImpl(AudioSource audioSource, float fadeInTime)
        {
            audioSource.Play();
            while (fadeInTime > 0 && audioSource.volume < 1)
            {
                audioSource.volume += Time.deltaTime / fadeInTime;
                audioSource.volume = Mathf.Min(audioSource.volume, 1);
                yield return new WaitForEndOfFrame();
            }
            audioSource.volume = 1;
        }

        /// <summary>
        /// 音量渐落
        /// </summary>    
        public void PlayOut(AudioSource audioSource, AudioClip clip, float fadeOutTime)
        {
            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.volume = 1;

            StartCoroutine(PlayOutImpl(audioSource, fadeOutTime));
        }

        IEnumerator PlayOutImpl(AudioSource audioSource, float fadeOutTime)
        {
            audioSource.volume = 1;
            audioSource.Play();
            while (fadeOutTime > 0 && audioSource.volume > 0)
            {
                audioSource.volume -= Time.deltaTime / fadeOutTime;
                audioSource.volume = Mathf.Max(audioSource.volume, 0);
                yield return new WaitForEndOfFrame();
            }
            audioSource.volume = 0;
            audioSource.Stop();
            audioSource.volume = 1;
        }

        /// <summary>
        /// 循环播放clip
        /// </summary>    
        public void PlayLoop(AudioSource audioSource, AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.volume = 1;
            audioSource.loop = true;
            audioSource.Play();
        }

        /// <summary>
        /// 在指定时长内循环播放clip
        /// </summary>    
        public void PlayLoop(AudioSource audioSource, AudioClip clip, float time,
            float fadeInTime = 0, float fadeOutTime = 0)
        {
            if (time > 0 && time < fadeInTime + fadeOutTime)
            {
                return;
            }

            audioSource.clip = clip;
            audioSource.loop = true;

            StartCoroutine(PlayLoopImpl(audioSource, time, fadeInTime, fadeOutTime));
        }

        IEnumerator PlayLoopImpl(AudioSource audioSource, float time,
            float fadeInTime, float fadeOutTime)
        {
            yield return new WaitForEndOfFrame();

            audioSource.Play();
            audioSource.volume = 0;
            while (fadeInTime > 0 && audioSource.volume < 1)
            {
                audioSource.volume = Mathf.Min(1.0f, audioSource.volume + Time.deltaTime / fadeInTime);
                yield return new WaitForEndOfFrame();
            }
            audioSource.volume = 1;

            yield return new WaitForSeconds(time - fadeInTime - fadeOutTime);

            while (fadeOutTime > 0 && audioSource.volume > 0)
            {
                audioSource.volume = Mathf.Max(0.0f, audioSource.volume - Time.deltaTime / fadeInTime);
                yield return new WaitForEndOfFrame();
            }
            audioSource.volume = 0;
            audioSource.Stop();
            audioSource.volume = 1;
        }

        /// <summary>
        /// 过渡播放(当前clip渐落之后，新clip渐起)
        /// </summary>    
        public void PlayCross(AudioSource audioSource, AudioClip clip, float fadeOutTime, float fadeInTime)
        {
            StartCoroutine(PlayCrossImpl(audioSource, clip, fadeOutTime, fadeInTime));
        }

        IEnumerator PlayCrossImpl(AudioSource audioSource, AudioClip clip, float fadeOutTime, float fadeInTime)
        {
            if (audioSource.isPlaying)
            {
                while (fadeOutTime > 0 && audioSource.volume > 0)
                {
                    audioSource.volume -= Time.deltaTime / fadeOutTime;
                    audioSource.volume = Mathf.Max(audioSource.volume, 0);
                    yield return new WaitForEndOfFrame();
                }
                audioSource.Stop();
                audioSource.volume = 0;
            }

            yield return new WaitForEndOfFrame();

            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
                while (fadeInTime > 0 && audioSource.volume < 1)
                {
                    audioSource.volume += Time.deltaTime / fadeInTime;
                    audioSource.volume = Mathf.Min(audioSource.volume, 1);
                    yield return new WaitForEndOfFrame();
                }
                audioSource.volume = 1;
            }
        }
    }
}