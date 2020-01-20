using System.Collections.Generic;

namespace Phenix.Unity.AI.GOAP
{
    public abstract class GOAPPlan
    {
        protected Queue<GOAPAction> steps = new Queue<GOAPAction>();

        protected WorldState CurWorldState { get; private set; }                
         
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

        public virtual void Reset()
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

        public void Build(WorldState curWS, GOAPGoal goal, List<GOAPAction> actions)
        {
            CurWorldState = curWS;
            while (steps.Count > 0)
            {
                steps.Dequeue().OnExit(curWS);
            }
            BuildPlan(goal, actions);            
            if (steps.Count > 0)
            {
                steps.Peek().OnEnter();
            }
        }

        // 可以A*搜索生成plan，也可以手动配置plan
        public abstract void BuildPlan(GOAPGoal goal, List<GOAPAction> actions);        
    }
}