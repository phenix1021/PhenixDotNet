using UnityEngine;
using System.Collections.Generic;

namespace Phenix.Unity.TurnBased
{
    public class TurnBasedClient : MonoBehaviour
    {        
        
        // 优先级[0, 1]
        public float priority;
        // 当前回合是否轮空
        public bool turnSkip;
        // 当前回合是否执行完毕
        public bool turnCompleted;

        bool _isDead;

        public bool isAI;

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

        // 是否死亡
        public bool IsDead
        {
            get
            {
                return _isDead;
            }
            set
            {
                _isDead = value;
                if (_isDead)
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
                        turnCompleted = true; // client本轮执行结束
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
            if (isAI)
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
    }
}