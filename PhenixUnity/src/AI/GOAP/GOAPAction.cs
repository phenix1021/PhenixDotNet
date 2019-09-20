using System.Collections.Generic;

namespace Phenix.Unity.AI
{
    public class WorldStateBitDataAction
    {
        public WorldStateBitData worldStateBitData;
        public bool applyActionEffectOnActionExit = false;

        public WorldStateBitDataAction(int bit, bool val, bool applyActionEffectOnActionExit)
        {
            worldStateBitData = new WorldStateBitData(bit, val);
            this.applyActionEffectOnActionExit = applyActionEffectOnActionExit;
        }
    }

    public abstract class GOAPAction
    {
        public int GOAPActionType { get; protected set; }

        List<WorldStateBitData> _WSPrecondition;
        List<WorldStateBitDataAction> _WSEffect;

        protected GOAPAction(int actionType, List<WorldStateBitData> WSPrecondition, 
            List<WorldStateBitDataAction> WSEffect)
        {
            GOAPActionType = actionType;
            _WSPrecondition = WSPrecondition;
            _WSEffect = WSEffect;
        }

        protected void AddWSPrecondition(int bit, bool val)
        {            
            foreach (var condition in _WSPrecondition)
            {
                if (condition.bit == bit && condition.val == val)
                {
                    return;
                }
            }
            _WSPrecondition.Add(new WorldStateBitData(bit, val));
        }

        protected void RemoveWSPrecondition(int bit, bool val)
        {
            foreach (var condition in _WSPrecondition)
            {
                if (condition.bit == bit && condition.val == val)
                {
                    _WSPrecondition.Remove(condition);
                    return;
                }
            }            
        }

        public virtual void Reset() { }

        public bool CheckWorldStatePrecondition(WorldState ws)
        {
            return ws.Include(_WSPrecondition);
        }

        public void ApplyWorldStateEffect(WorldState ws)
        {
            foreach (var item in _WSEffect)
            {
                ws.Set(item.worldStateBitData.bit, item.worldStateBitData.val);
            }            
        }               

        public virtual int GetCost() { return 0; }
        public virtual int GetPrecedence() { return 0; }

        public virtual void OnEnter() { }
        public virtual void OnExit(WorldState ws)
        {
            foreach (var item in _WSEffect)
            {
                if (item.applyActionEffectOnActionExit)
                {
                    ApplyWorldStateEffect(ws);
                }
            }
        }
        public virtual void OnUpdate() { }

        public virtual bool IsAborted() { return false; }
        public abstract bool IsFinished();
        public virtual void BeforeBuildPlan(GOAPGoal goal) { }
    }
}