using UnityEngine;
using System;

namespace Phenix.Unity.AI
{
    public enum TaskStatus
    {
        None = 0,
        Success,
        Failure,
        Running,
        Ignored,
    }
        
    [Serializable]
    public class NodeData
    {        
        public Vector2 pos = Vector2.zero;         // 对应的编辑器节点位置        
    }

    [Serializable]
    public abstract class Task : ScriptableObject
    {
        BehaviorTree _bt;
        TaskStatus _status = TaskStatus.None;
        Task _parent = null;
        bool _firstRun = true;
        bool _turnOver = true;
        //string _guid;
                
        [HideInInspector]
        public NodeData nodeData = new NodeData();
                
        public BehaviorTree BT { get { return _bt; } set { _bt = value; } }
        public Task Parent { get { return _parent; } set { _parent = value; } }        
        public TaskStatus Status { get { return _status; } set { _status = value; } }
        //public string GUID { get { return _guid; } set { _guid = value; } }
        //public NodeData NodeData { get { return _nodeData; } set { _nodeData = value; } }

        protected virtual void OnEnable() { }
        protected virtual void OnFirstRun() { }
        protected virtual void OnTurnBegin() { }
        protected virtual void OnTurnEnd() { }

        public virtual void OnAwake() { }
        public virtual void OnStart() { }
        public virtual TaskStatus OnUpdate()
        {
            if (_status != TaskStatus.Ignored)
            {
                if (_firstRun)
                {
                    OnFirstRun();
                    _firstRun = false;
                }
                if (_turnOver)
                {
                    _turnOver = false;
                    OnTurnBegin();
                }
                _status = Run();
                if (_status != TaskStatus.Running)
                {
                    OnTurnEnd();
                    _turnOver = true;
                }
            }
            
            return _status;
        }        

        public virtual void ForceTurnEnd()
        {
            if (_status == TaskStatus.Running)
            {
                _status = TaskStatus.Failure;
                OnTurnEnd();
            }
        }

        public abstract TaskStatus Run();        
        public virtual float GetPriority() { return 0; }

        /*public void OnFixedUpdate()
        {

        }

        public void OnLateUpdate()
        {

        }

        public void OnAnimatorIK(int layerIndex)
        {

        }

        public void OnCollisionEnter(Collision collision)
        {

        }

        public void OnCollisionStay(Collision collision)
        {

        }

        public void OnCollisionExit(Collision collision)
        {

        }

        public void OnCollisionEnter2D(Collision2D collision)
        {

        }
        public void OnCollisionStay2D(Collision2D collision)
        {

        }
        public void OnCollisionExit2D(Collision2D collision)
        {

        }

        public void OnControllerColliderHit(ControllerColliderHit hit)
        {

        }

        public void OnDrawGizmos()
        {

        }

        public void OnDrawGizmosSelected()
        {

        }

        public void OnApplicationPause(bool pause)
        {

        }

        public void OnTriggerEnter(Collider other)
        {

        }

        public void OnTriggerStay(Collider other)
        {

        }

        public void OnTriggerExit(Collider other)
        {

        }

        public void OnTriggerEnter2D(Collider2D collision)
        {

        }

        public void OnTriggerStay2D(Collider2D collision)
        {

        }

        public void OnTriggerExit2D(Collider2D collision)
        {

        }*/
    }
}