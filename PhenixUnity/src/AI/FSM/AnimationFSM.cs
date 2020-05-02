using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.AI.FSM
{
    [System.Serializable]
    public class AnimationFSMState
    {
        public AnimationClip clip;
        public bool loop = false;
        public int level = 0;

        public UnityEvent onEnter;     // 进入动画
        public UnityEvent onComplete;  // 动画播放完成
        public UnityEvent onExit;      // 离开动画
    }

    /// <summary>
    /// 动画状态机
    /// </summary>
    public class AnimationFSM : MonoBehaviour
    {
        [SerializeField]
        Animation _anim;
        
        public AnimationClip defaultClip;   // 默认动画

        AnimationFSMState _default;     // 默认状态
        AnimationFSMState _current;     // 当前状态

        public List<AnimationFSMState> states = new List<AnimationFSMState>();
        Dictionary<string, AnimationFSMState> _states = new Dictionary<string, AnimationFSMState>(); // 状态集    

        public Animation Animation { get { return _anim; } }

        float _stateCompleteTimer = 0;
        float _stateTime = 0;

        // 切换动画状态
        public bool TransferTo(string clipName)
        {
            if (clipName == _current.clip.name)
            {
                return false;
            }

            if (_states.ContainsKey(clipName) == false)
            {
                return false;
            }

            AnimationFSMState nextState = _states[clipName];
            if (nextState.level < _current.level)
            {
                return false;
            }

            TransferTo(nextState);

            return true;
        }

        void TransferTo(AnimationFSMState nextState)
        {
            if (_current != null && _current.onExit != null)
            {
                _current.onExit.Invoke();
            }

            _current = nextState;
            if (_current.loop == false)
            {
                _stateTime = _current.clip.length;
                _stateCompleteTimer = Time.timeSinceLevelLoad + _stateTime;
            }
            else
            {
                _stateTime = 0;
                _stateCompleteTimer = 0;
            }

            if (_current.onEnter != null)
            {
                _current.onEnter.Invoke();
            }

            AnimationTools.PlayAnim(_anim, _current.clip.name, 0.1f);
        }

        // Use this for initialization
        void Start()
        {
            foreach (var item in states)
            {
                if (item != null)
                {
                    _anim[item.clip.name].wrapMode = item.loop ? WrapMode.Loop : WrapMode.Once;
                    _anim[item.clip.name].layer = item.level;
                    _states.Add(item.clip.name, item);

                    if (item.clip == defaultClip)
                    {
                        _default = item;
                    }
                }
            }

            TransferTo(GetDefaultState());
        }

        // Update is called once per frame
        void Update()
        {
            if (_stateCompleteTimer > 0 && Time.timeSinceLevelLoad > _stateCompleteTimer)
            {
                AnimationFSMState oriState = _current;
                TransferTo(GetDefaultState());
                if (oriState.onComplete != null)
                {
                    oriState.onComplete.Invoke();
                }                
            }
        }

        protected virtual AnimationFSMState GetDefaultState()
        {
            return _default;
        }
    }
}