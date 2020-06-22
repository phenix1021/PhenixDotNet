using UnityEngine;
using System.Collections;
using Phenix.Unity.Pattern;

namespace Phenix.Unity.Utilities
{
    public class AudioTools : StandAloneSingleton<AudioTools>
    {
        /// <summary>
        /// 简单播一发
        /// </summary>    
        public Coroutine PlayOneShot(AudioSource audioSource, AudioClip clip, float delay = 0)
        {
            if (audioSource == null || clip == null)
            {
                return null;
            }

            audioSource.volume = 1;
            if (delay == 0)
            {
                audioSource.PlayOneShot(clip);
                return null;
            }
            else
            {
                return StartCoroutine(PlayOneShotImpl(audioSource, clip, delay));
            }
        }

        IEnumerator PlayOneShotImpl(AudioSource audioSource, AudioClip clip, float delay)
        {
            yield return new WaitForSeconds(delay);
            audioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// 音量渐起
        /// </summary>    
        public Coroutine PlayIn(AudioSource audioSource, AudioClip clip, float fadeInTime)
        {
            if (audioSource == null || clip == null)
            {
                return null;
            }

            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.volume = 0;

            return StartCoroutine(PlayInImpl(audioSource, fadeInTime));
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
        public Coroutine PlayOut(AudioSource audioSource, AudioClip clip, float fadeOutTime)
        {
            if (audioSource == null || clip == null)
            {
                return null;
            }

            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.volume = 1;

            return StartCoroutine(PlayOutImpl(audioSource, fadeOutTime));
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
            if (audioSource == null || clip == null)
            {
                return;
            }

            audioSource.clip = clip;
            audioSource.volume = 1;
            audioSource.loop = true;
            audioSource.Play();
        }

        /// <summary>
        /// 在指定时长内循环播放clip
        /// </summary>    
        public Coroutine PlayLoop(AudioSource audioSource, AudioClip clip, float time,
            float fadeInTime = 0, float fadeOutTime = 0)
        {
            if (audioSource == null || clip == null)
            {
                return null;
            }

            if (time > 0 && time < fadeInTime + fadeOutTime)
            {
                return null;
            }

            audioSource.clip = clip;
            audioSource.loop = true;

            return StartCoroutine(PlayLoopImpl(audioSource, time, fadeInTime, fadeOutTime));
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
        public Coroutine PlayCross(AudioSource audioSource, AudioClip clip, float fadeOutTime, float fadeInTime)
        {
            if (audioSource == null || clip == null)
            {
                return null;
            }

            return StartCoroutine(PlayCrossImpl(audioSource, clip, fadeOutTime, fadeInTime));
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

        /// <summary>
        /// 动态设置声源（如子弹击中物体时在hitPoint动态添加临时声源，播放后删除）
        /// </summary>        
        public void PlaySoundAtPosition(AudioClip sound, Vector3 pos, float volume, 
            float minDistance = 1f)
        {
            if (sound == null)
                return;

            GameObject emptyGO = new GameObject("Sound" + sound.name);            
            emptyGO.transform.position = pos;
            AudioSource source = emptyGO.AddComponent<AudioSource>();
            source.clip = sound;
            source.volume = volume;
            source.minDistance = minDistance;
            source.Play();
            Destroy(emptyGO, sound.length);
        }
    }
}