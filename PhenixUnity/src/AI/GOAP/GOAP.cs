using System.Collections.Generic;

namespace Phenix.Unity.AI.GOAP
{    
    public class GOAP
    {   
        List<GOAPGoal> _goals;
        List<GOAPAction> _actions;

        GOAPGoal _curGoal;
        GOAPPlan _plan;

        protected Dictionary<int, GOAPGoal> goalsMap = new Dictionary<int, GOAPGoal>();

        public WorldState WorldState { get; private set; }

        public GOAP(WorldState ws, List<GOAPGoal> goals, List<GOAPAction> actions, GOAPAStarBase astar)
        {
            WorldState = ws;
            _goals = goals;
            _actions = actions;
            _plan = new GOAPPlan(astar);
            _curGoal = null;
            foreach (var goal in goals)
            {
                goalsMap.Add(goal.GOAPGoalType, goal);
            }
        }

        public void Reset()
        {
            if (WorldState != null)
            {
                WorldState.Clear();                
            }

            if (_plan != null)
            {
                _plan.Reset();
            }

            _curGoal = null;

            foreach (var goal in _goals)
            {
                goal.Reset();
            }

            foreach (var action in _actions)
            {
                action.Reset();
            }
        }

        protected virtual GOAPGoal FindTopGoal()
        {
            float max = -1;
            GOAPGoal ret = null;
            foreach (var goal in _goals)
            {
                if (goal.InCD())
                {
                    continue;
                }
                float weight = goal.GetWeight(WorldState);
                if (weight > max)
                {
                    max = weight;
                    ret = goal;
                }
            }
            return ret;
        }

        public void OnUpdate()
        {
            if (_curGoal != null)
            {
                if (_curGoal.IsAborted() || _curGoal.IsFinished(WorldState) || _plan.IsAborted())
                {
                    _curGoal.OnExit(WorldState);                    
                    _curGoal = null;
                }                
            }

            GOAPGoal topGoal = FindTopGoal();
            if (topGoal == null /*|| topGoal.IsAborted()*/ || topGoal.IsFinished(WorldState))
            {
                // 如果topGoal不合理
                if (_curGoal != null)
                {
                    _plan.OnUpdate();
                }
                return;
            }

            // 如果topGoal合理
            if (_curGoal != topGoal)
            {
                if (_curGoal != null)
                {
                    _curGoal.OnExit(WorldState);
                }                     
                _curGoal = topGoal;
                foreach (var action in _actions)
                {
                    action.BeforeBuildPlan(_curGoal);
                }
                _plan.Build(WorldState, _curGoal);
                _curGoal.OnEnter(WorldState);
            }
            else if (_plan.IsEmpty())
            {
                _plan.Build(WorldState, _curGoal);
            }

            _plan.OnUpdate();            
        }
    }
}