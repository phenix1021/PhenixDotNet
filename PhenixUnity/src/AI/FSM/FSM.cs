using System.Collections.Generic;

namespace Phenix.Unity.AI
{

    public class FSM
    {
        List<FSMState> _states;                         // 状态集        
        FSMEvent _curEvent;                             // 当前事件
        protected FSMState CurState { get; set; }       // 当前状态
        public FSMState DefState { get; set; }          // 默认状态                    

        public FSM(List<FSMState> states, FSMState defState)
        {
            _states = states;
            CurState = DefState = defState;
            CurState.OnEnter();
        }        

        public FSMState GetState(int stateType)
        {
            foreach (var state in _states)
            {
                if (state.StateType == stateType)
                {
                    return state;
                }
            }
            return null;
        }

        public void OnUpdate()
        {
            if (CurState == null)
            {
                return;
            }

            CurState.OnUpdate();
            if (CurState.IsFinished)
            {
                CurState.OnExit();
                CurState = DefState;
                CurState.OnEnter();
            }            
            else if (_curEvent != null)
            {
                FSMState next = OnTransfer(_curEvent);
                CurState.OnExit();
                CurState = next;
                CurState.OnEnter(_curEvent);
                _curEvent = null;
            }
        }

        protected virtual FSMState OnTransfer(FSMEvent ev)
        {
            return ev == null ? null : GetState(ev.FSMStateTypeToTransfer);
        }

        public void SendEvent(FSMEvent ev)
        {
            if (CurState == null)
            {
                return;
            }            
            if (CurState.OnEvent(ev))
            {
                return;
            }
            _curEvent = ev;            
        }

        public void TransferTo(int stateType, FSMEvent ev = null)
        {            
            foreach (var item in _states)
            {
                if (item.StateType == stateType)
                {
                    TransferTo(item, ev);
                    return;
                }
            }            
        }

        void TransferTo(FSMState state, FSMEvent ev)
        {
            if (state == null)
            {
                return;
            }

            if (state == CurState)
            {
                return;
            }

            CurState.OnExit();
            CurState = state;
            CurState.OnEnter(ev);
        }
    }
}