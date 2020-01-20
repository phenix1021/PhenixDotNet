using UnityEngine.Events;

namespace Phenix.Unity.AI.FSM
{
    public enum FSMEventResult
    {
        NONE = 0,
        FAILURE,
        SUCCESS,
    }

    public abstract class FSMEvent
    {
        bool _isFinished = false;

        public int EventCode { get; protected set; }        
        public int FSMStateTypeToTransfer { get; set; }
        public virtual void Release() { }

        public event System.Action<FSMEvent> onEventFinished;

        public bool IsFinished
        {
            get { return _isFinished; }
            set
            {
                _isFinished = value;
                if (_isFinished && onEventFinished != null)
                {
                    onEventFinished.Invoke(this);
                }
            }
        }
    }
}