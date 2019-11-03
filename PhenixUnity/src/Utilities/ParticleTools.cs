using UnityEngine;
using System.Collections;
using Phenix.Unity.Pattern;
using UnityEngine.Events;

namespace Phenix.Unity.Utilities
{
    public class ParticleTools : StandAloneSingleton<ParticleTools>
    {
        /// <summary>
        /// 播放粒子时，动态设定ParticleSystem的start rotation参数
        /// </summary>        
        public Coroutine Play(ParticleSystem particleSystem, float startRotationRadian, float delay = 0)
        {
            if (particleSystem == null)
            {
                return null;
            }
            var main = particleSystem.main;
            main.startRotation = new ParticleSystem.MinMaxCurve(startRotationRadian);
            if (delay == 0)
            {
                particleSystem.gameObject.SetActive(true);
                particleSystem.Play();
                return null;
            }
            else
            {
                return StartCoroutine(PlayImpl(particleSystem, delay));
            }
        }

        IEnumerator PlayImpl(ParticleSystem particleSystem, float delay)
        {
            yield return new WaitForSeconds(delay);
            particleSystem.gameObject.SetActive(true);
            particleSystem.Play();            
        }

        public void Stop(ParticleSystem particleSystem, bool hide = true)
        {
            if (particleSystem == null)
            {
                return;
            }
            particleSystem.Stop();
            if (hide)
            {
                particleSystem.gameObject.SetActive(false);
            }            
        }
    }
}