using UnityEngine;

namespace Phenix.Unity.Anim
{
    public abstract class AnimProxy : MonoBehaviour
    {
        // 播放
        public abstract void Play(string clipName, float fadeTime);
        // clip总时长（秒）
        public abstract float GetLength(string clipName);
        // 获得播放速度
        public abstract float GetSpeed(string clipName);
        // 设置播放速度
        public abstract void SetSpeed(string clipName, float val);
        // 已播放时长（秒）
        public abstract float GetTime(string clipName);
        // 获得实际总时长
        public virtual float GetRealLength(string clipName)
        {
            float speed = GetSpeed(clipName);
            if (speed == 0)
            {
                return 0;
            }

            return GetLength(clipName) / speed;
        }
        public abstract bool IsPlaying(AnimationClip clip);
        public abstract float GetNormalizedTime(AnimationClip clip);
    }
}