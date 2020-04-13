using Phenix.Unity.AI.SEARCH;
using System.Collections.Generic;
using UnityEngine;

namespace Phenix.Unity.AI.GOAP
{
    public class GOAPPlan
    {
        protected Queue<GOAPAction> steps = new Queue<GOAPAction>();
        GOAPAStarBase _astar;        
        List<GOAPAction> _path = new List<GOAPAction>();

        protected WorldState CurWorldState { get; private set; }                
         
        public GOAPPlan(GOAPAStarBase astar)
        {
            _astar = astar;
        }

        public void OnUpdate()
        {
            if (IsEmpty())
            {
                return;
            }

            GOAPAction action = steps.Peek();
            action.OnUpdate();
            if (action.IsFinished())
            {
                action.ApplyWorldStateEffect(CurWorldState);
                action.OnExit(CurWorldState);                
                steps.Dequeue();
                if (steps.Count > 0)
                {
                    steps.Peek().OnEnter();
                }
            }
        }

        public void Reset()
        {
            CurWorldState = null;
            steps.Clear();
        }

        public bool IsEmpty()
        {
            return steps == null || steps.Count == 0;
        }

        public bool IsAborted()
        {
            if (IsEmpty())
            {
                return false;
            }
            return steps.Peek().IsAborted() /*|| _steps.Peek().CheckWorldStatePrecondition(CurWorldState) == false*/;
        }

        public void Build(WorldState curWS, GOAPGoal goal)
        {
            CurWorldState = curWS;
            while (steps.Count > 0)
            {
                steps.Dequeue().OnExit(curWS);
            }
            BuildPlan(goal);            
            if (steps.Count > 0)
            {
                steps.Peek().OnEnter();
            }
        }

        // 可以A*搜索生成plan，也可以手动配置plan
        void BuildPlan(GOAPGoal goal)
        {
            int maxOverLoad = 100;
            AStarResultCode ret = _astar.FindPath(goal, CurWorldState, ref _path, maxOverLoad);
            switch (ret)
            {
                case AStarResultCode.SUCCESS:
                    foreach (var action in _path)
                    {
                        steps.Enqueue(action);
                    }
                    break;
                case AStarResultCode.FAIL_NO_WAY:
                    Debug.Log(string.Format("寻路失败！无法获得goalType = {0}的路径。", goal.GetType()));
                    break;
                case AStarResultCode.FAIL_OVERLOAD:
                    Debug.Log(string.Format("寻路失败！在寻找goalType = {0}的路径时open列表元素个数超过上限{1}。",
                        goal.GetType(), maxOverLoad));
                    break;
                default:
                    break;
            }    
        }
    }
}