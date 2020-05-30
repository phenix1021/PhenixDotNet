using UnityEngine;

namespace Phenix.Unity.Utilities
{
    public class AnimationTools
    {
        public static void PlayAnim(Animation animEngine, string anim, float fadeInTime,
            QueueMode queueMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopAll)
        {
            if (animEngine == null || string.IsNullOrEmpty(anim))
            {
                return;
            }

            if (animEngine.IsPlaying(anim))
            {
                // 如果自身正在被播放
                animEngine.CrossFadeQueued(anim, fadeInTime, queueMode, playMode);
            }
            else
            {
                animEngine.CrossFade(anim, fadeInTime, playMode);
            }
        }

        public static AnimationClip GetClip(Animator animator, string clipName)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip && clip.name == clipName)
                {
                    return clip;
                }
            }
            return null;
        }
    }
}