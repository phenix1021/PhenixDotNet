﻿using System.Collections.Generic;

namespace Phenix.Unity.AI
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

        public int GOAPGoalType { get; protected set; }

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

        public abstract float GetWeight(WorldState ws);        

        public virtual void OnEnter(WorldState ws) { }
        public virtual void OnExit(WorldState ws)
        {
            foreach (var item in _goalProps)
            {
                if (item.cancelGoalEffectOnGoalExit)
                {
                    ws.Set(item.worldStateBitData.bit, !item.worldStateBitData.val);
                }
            }
        }

        public virtual bool IsAborted() { return false; }        
    }
}