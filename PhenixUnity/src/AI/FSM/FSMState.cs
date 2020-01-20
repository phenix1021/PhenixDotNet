namespace Phenix.Unity.AI.FSM
{
    public abstract class FSMState
    {
        public int StateType { get; private set; }

        public bool IsFinished { get; protected set; }

        public FSMState(int stateType)
        {
            StateType = stateType;
        }

        public virtual void OnEnter(FSMEvent ev = null)
        {
            IsFinished = false;
            Initialize(ev);
        }

        public virtual void OnExit() { }

        protected virtual void Initialize(FSMEvent ev = null) { }        

        public virtual bool OnEvent(FSMEvent ev) { return false; }
                
        public virtual void OnUpdate() { }        
    }
}