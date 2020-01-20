using System.Collections.Generic;
using UnityEngine;

namespace Phenix.Unity.AI.GOAP
{    
    public class WorldStateBitDataGoal
    {
        public WorldStateBitData worldStateBitData;
        public bool cancelGoalEffectOnGoalExit = false;

        public WorldStateBitDataGoal(int bit, bool val, bool cancelGoalEffectOnGoalExit)            
        {
            worldStateBitData = new WorldStateBitData(bit, val);
            this.cancelGoalEffectOnGoalExit = cancelGoalEffectOnGoalExit;
        }
    }

    public abstract class GOAPGoal
    {
        List<WorldStateBitDataGoal> _goalProps;

        float _nextActiveTimer = 0;

        public int GOAPGoalType { get; protected set; }        

        public bool InProgress { get; private set; }

        protected GOAPGoal(int goalType, List<WorldStateBitDataGoal> goalProps)        
        {
            GOAPGoalType = goalType;
            _goalProps = goalProps;            
        }

        public virtual void Reset() { }

        public bool IsFinished(WorldState ws)
        {
            if (ws == null)
            {
                return false;
            }

            foreach (var item in _goalProps)
            {
                if (ws.Get(item.worldStateBitData.bit) != item.worldStateBitData.val)
                {
                    return false;
                }
            }

            return true;
        }

        public void ApplyEffect(WorldState ws)
        {
            if (ws == null)
            {
                return;
            }

            foreach (var item in _goalProps)
            {
                ws.Set(item.worldStateBitData.bit, item.worldStateBitData.val);
            }
        }

        public abstract float GetWeight(WorldState ws);        

        public virtual void OnEnter(WorldState ws)
        {
            InProgress = true;
        }

        public virtual void OnExit(WorldState ws)
        {
            foreach (var item in _goalProps)
            {
                if (item.cancelGoalEffectOnGoalExit)
                {
                    ws.Set(item.worldStateBitData.bit, !item.worldStateBitData.val);
                }
            }
            _nextActiveTimer = Time.timeSinceLevelLoad + GetCD();
            InProgress = false;
        }

        public virtual bool IsAborted() { return false; }        
        public virtual float GetCD() { return 0; }
        public bool InCD() { return _nextActiveTimer > Time.timeSinceLevelLoad; }
    }
}