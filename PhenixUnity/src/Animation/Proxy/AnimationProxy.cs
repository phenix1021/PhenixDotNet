using UnityEngine;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.Anim
{

    public class AnimationProxy : AnimProxy
    {
        public Animation animation;

        public override float GetLength(string clipName)
        {
            if (animation == null)
            {
                return 0;
            }
            return animation[clipName].length;
        }

        public override void Play(string clipName, float fadeTime)
        {
            if (animation == null)
            {
                return;
            }
            AnimationTools.PlayAnim(animation, clipName, fadeTime);
        }

        public override float GetSpeed(string clipName)
        {
            if (animation == null)
            {
                return 0;
            }
            return animation[clipName].speed;
        }

        public override void SetSpeed(string clipName, float val)
        {
            if (animation == null)
            {
                return;
            }
            animation[clipName].speed = val;
        }

        public override float GetTime(string clipName)
        {
            if (animation == null)
            {
                return 0;
            }
            return animation[clipName].time;
        }
    }
}