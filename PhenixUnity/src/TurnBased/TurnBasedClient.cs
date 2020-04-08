using UnityEngine;
using System.Collections.Generic;

namespace Phenix.Unity.TurnBased
{
    public class TurnBasedClient : MonoBehaviour
    {   
        // 优先级[0, 1]
        public int priority;
        // 当前回合是否轮空
        public bool turnSkip;
        // 当前回合是否执行完毕
        public bool turnCompleted;

        bool _isQuit;

        [SerializeField]
        bool _isAI;

        public bool IsAI { get { return _isAI; } set { _isAI = value; OnIsAIChanged(); } }

        Queue<ITurnBasedCommand> _commands = new Queue<ITurnBasedCommand>(); // 本回合command集合

        // 获得当前正在执行的命令
        public ITurnBasedCommand CurCommand
        {
            get
            {
                if (_commands.Count == 0)
                {
                    return null;
                }

                return _commands.Peek();
            }
        }

        // 是否退出战斗
        public bool IsQuit
        {
            get
            {
                return _isQuit;
            }
            set
            {
                _isQuit = value;
                if (_isQuit)
                {
                    turnCompleted = true;
                }
            }
        }       

        public void AddCommand(ITurnBasedCommand command)
        {
            _commands.Enqueue(command);
            if (_commands.Count == 1)
            {
                _commands.Peek().OnStart();
            }
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
            if (_commands.Count > 0)
            {
                _commands.Peek().OnUpdate();
                if (_commands.Peek().Finished)
                {
                    // 如果当前command执行完毕
                    _commands.Peek().OnEnd();
                    _commands.Dequeue();
                    if (_commands.Count == 0)
                    {
                        turnCompleted = true; // client本回合所有命令执行完毕
                    }
                    else
                    {
                        // 开始下一command
                        _commands.Peek().OnStart();
                    }
                }
            }
        }

        public void OnExecute(int turnID)
        {
            if (_isAI)
            {
                ExecuteAI(turnID);
            }
            else
            {
                ExecuteManual(turnID);
            }
        }

        protected virtual void ExecuteAI(int turnID)
        {
            // 敌方英雄或player设置“自动”时由AI模块处理，生成command提交到hero
            // AIMgr.Instance.Execute(this);
        }

        protected virtual void ExecuteManual(int turnID)
        {
            // 本方手动控制
            // 发送UI提示，高显当前hero、行走范围、可攻击对象。玩家选择后生成command提交到hero
        }

        protected virtual void OnIsAIChanged()
        {

        }
    }
}