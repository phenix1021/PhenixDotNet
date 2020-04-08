using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phenix.Unity.TurnBased
{
    /// <summary>
    /// 回合管理
    /// </summary>
    public class TurnBasedMgr<T> : MonoBehaviour where T : TurnBasedClient
    {
        bool _active = false;

        // 当前回合ID
        public int TurnID { get; private set; }

        List<T> _clients = new List<T>();
        public List<T> Clients { get { return _clients; } }
        public T CurClient { get; private set; }

        public System.Action onCombatBegin;                                 // 回调：战斗开始
        public System.Action<int/*当前回合ID*/> onTurnBegin;                 // 回调：回合开始        
        public System.Action<int/*当前回合ID*/> onTurnEnd;                   // 回调：回合结束
        public System.Action onCombatEnd;                                   // 回调：战斗结束
        public System.Func<int/*0为无效，其它为胜负平flag标识*/> onResultJudge;// 回调：胜负平判定

        public float clientInterval = 1;  // client之间的执行间隔（秒）
        public float turnInterval = 2;    // 回合之间的执行间隔（秒）

        public void StartCombat()
        {
            _active = true;
            OnCombatBegin();
            StartCoroutine(HandleAllTurns());
        }

        void StopCombat()
        {            
            _active = false;
            _clients.Clear();
        }

        void OnCombatBegin()
        {
            if (onCombatBegin != null)
            {
                onCombatBegin.Invoke();
            }            
        }

        IEnumerator HandleAllTurns()
        {
            while (true)
            {
                OnTurnBegin();

                foreach (var client in _clients)
                {
                    if (client == null || client.turnCompleted)
                    {                        
                        continue;
                    }

                    if (client.turnSkip)
                    {
                        // 本回合轮空                    
                        continue;
                    }

                    CurClient = client;

                    // 每个client执行本回合逻辑
                    client.OnExecute(TurnID);
                    while (client != null && client.turnCompleted == false)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    CurClient = null;

                    if (IsCombatOver())
                    {
                        // 战斗结束
                        OnCombatEnd();
                        yield break;
                    }

                    yield return new WaitForSeconds(clientInterval);
                }

                OnTurnEnd();

                yield return new WaitForSeconds(turnInterval);
            }            
        }

        bool IsCombatOver()
        {
            if (onResultJudge != null)
            {
                return onResultJudge.Invoke() != 0;
            }
            return false;
        }

        void OnCombatEnd()
        {
            if (onCombatEnd != null)
            {
                onCombatEnd.Invoke();
            }

            StopCombat();
        }

        void OnTurnBegin()
        {
            TurnID += 1; // 回合数+1

            foreach (var client in _clients)
            {
                if (client != null)
                {
                    client.turnCompleted = client.IsQuit;
                }
            }

            // 依据priority进行降序排列
            _clients.Sort((a, b) => 
            {
                int ret = -a.priority.CompareTo(b.priority);
                if (ret == 0)
                {
                    return -a.GetHashCode().CompareTo(b.GetHashCode());
                }
                return ret;
            });

            if (onTurnBegin != null)
            {
                // 回合开始回调
                onTurnBegin.Invoke(TurnID);
            }
        }

        void OnTurnEnd()
        {
            if (onTurnEnd != null)
            {
                onTurnEnd.Invoke(TurnID);
            }
        }

        // 加入新的client
        public void Join(T client)
        {
            if (_active)
            {
                // 进行中不允许中途加入
                return;
            }
            
            _clients.Add(client);            
        }
    }
}