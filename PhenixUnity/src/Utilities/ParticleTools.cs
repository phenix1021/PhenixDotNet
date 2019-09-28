using UnityEngine;
using System.Collections;
using Phenix.Unity.Pattern;
using Phenix.Unity.Extend;

namespace Phenix.Unity.Utilities
{
    public class ParticleTools : AloneSingleton<ParticleTools>
    {
        /// <summary>
        /// 播放粒子时，动态设定ParticleSystem的start rotation参数
        /// </summary>        
        public void Play(ParticleSystem particleSystem, float startRotationDegree, float delay)
        {
            if (particleSystem == null)
            {
                return;
            }
            var main = particleSystem.main;
            main.startRotation = new ParticleSystem.MinMaxCurve(startRotationDegree);
            StartCoroutine(PlayImpl(particleSystem, delay));
        }

        IEnumerator PlayImpl(ParticleSystem particleSystem, float delay)
        {
            yield return new WaitForSeconds(delay);
            particleSystem.Play();
            yield return new WaitForEndOfFrame();
            //particleSystem.Stop();
        }
    }
}