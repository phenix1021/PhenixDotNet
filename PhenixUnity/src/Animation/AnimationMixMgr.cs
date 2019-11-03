using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.Anim
{
    [System.Serializable]
    public class AnimationMixData
    {
        public AnimationClip baseClip;
        public Transform[] mixTransformList;
        public int layer = 10; // 必须大于非mix动画的layer
        public WrapMode wrapMode = WrapMode.Clamp;
        public string newAnimationName;
    }

    /// <summary>
    /// animation动画融合管理
    /// </summary>
    /// <usage>
    /// 添加到管理器的融合动画可以直接通过animation["新动画名"]访问
    /// </usage>
    [AddComponentMenu("Phenix/Animation/AnimationMixMgr")]
    public class AnimationMixMgr : MonoBehaviour
    {
        public List<AnimationMixData> mixDatum = new List<AnimationMixData>();

        [SerializeField]
        Animation _animation = null;

        // Use this for initialization
        void Start()
        {
            if (_animation == null)
            {
                return;
            }

            foreach (var mixData in mixDatum)
            {
                _animation.AddClip(mixData.baseClip, mixData.newAnimationName);

                AnimationState animState = _animation[mixData.newAnimationName];
                animState.layer = mixData.layer;
                animState.wrapMode = mixData.wrapMode;
                foreach (var mixTransform in mixData.mixTransformList)
                {
                    animState.AddMixingTransform(mixTransform);                    
                }
            }            
        }

        public void PlayAnim(Animation anim, string mixAnimName, float fadeInTime, 
            QueueMode queueMode = QueueMode.PlayNow)
        {
            foreach (var mixData in mixDatum)
            {
                if (mixData.newAnimationName == mixAnimName)
                {
                    AnimationTools.PlayAnim(anim, mixAnimName, fadeInTime, queueMode, 
                        PlayMode.StopSameLayer/*不可以是StopAll*/);
                }
            }
        }
    }
}