using UnityEngine;
using System.Collections.Generic;

namespace Phenix.Unity.Anim
{

    [System.Serializable]
    public class AnimatorSpeedData
    {
        public AnimationClip clip;
        public float speed;
    }

    /// <summary>
    /// animator.speed是全局值，无法在代码中读写指定clip的speed，所以写了这个类来进行管理。
    /// 注意如果使用本类管理speed，会和animator动画状态机编辑器中设置的同名状态speed冲突，
    /// 结果unexpected。所以只能二选一。
    /// </summary>
    public class AnimatorProxy : AnimProxy
    {
        public Animator animator;

        public List<AnimatorSpeedData> speeds = new List<AnimatorSpeedData>();
        public Dictionary<string, float> _speeds = new Dictionary<string, float>();

        private void Start()
        {
            foreach (var speedData in speeds)
            {
                if (speedData.clip == null)
                {
                    continue;
                }
                _speeds.Add(speedData.clip.name, speedData.speed);
            }
        }

        private void Update()
        {
            if (animator == null)
            {
                return;
            }

            AnimatorStateInfo curState = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(0);
            /*nextState.length > 0, 说明animator已经开始朝nextState动画过度，应采用其speed*/
            AnimatorStateInfo state = nextState.length > 0 ? nextState : curState;
            
            foreach (var speedData in _speeds)
            {
                string clipName = speedData.Key;
                if (state.IsName(clipName))
                {
                    float speed = speedData.Value;
                    animator.speed = speed;
                    return;
                }
            }

            animator.speed = 1;
        }

        AnimationClip GetClip(string clipName)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == clipName)
                {
                    return clip;
                }
            }
            return null;
        }

        public override float GetLength(string clipName)
        {
            if (animator == null)
            {
                return 0;
            }
            AnimationClip clip = GetClip(clipName);
            if (clip == null)
            {
                return 0;
            }
            return clip.length;
        }

        public override void Play(string clipName, float fadeTime)
        {
            if (animator == null)
            {
                return;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(clipName))
            {
                // 如果正在播放clipName
                animator.Rebind();
            }
            animator.CrossFade(clipName, fadeTime);
        }

        public override float GetSpeed(string clipName)
        {
            if (_speeds.ContainsKey(clipName) == false)
            {
                return 1;
            }

            return _speeds[clipName];
        }

        public override void SetSpeed(string clipName, float val)
        {
            if (_speeds.ContainsKey(clipName))
            {
                _speeds[clipName] = val;
                return;
            }

            _speeds.Add(clipName, val);
        }

        public override float GetTime(string clipName)
        {
            // todo: 待实现
            return 0;
        }

        public override bool IsPlaying(AnimationClip clip)
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(clip.name);
        }

        public override float GetNormalizedTime(AnimationClip clip)
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName(clip.name) == false)
            {
                return float.NegativeInfinity;
            }

            return info.normalizedTime;
        }
    }
}