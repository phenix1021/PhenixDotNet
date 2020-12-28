using UnityEngine;
using System;

namespace Phenix.Unity.AI.BT
{
    [Serializable]
    public enum TaskStatus
    {
        NONE = 0,
        SUCCESS,
        FAILURE,
        RUNNING,
        IGNORED,
        ERROR,
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
        TaskStatus _status = TaskStatus.NONE;
        Task _parent = null;
        //bool _firstRun = true;
        protected bool turnOver = true;        
                
        [HideInInspector]
        public NodeData nodeData = new NodeData();
                
        public BehaviorTree BT { get { return _bt; } set { _bt = value; } }
        public Task Parent { get { return _parent; } set { _parent = value; } }        
        public TaskStatus Status { get { return _status; } set { _status = value; } }        

        //protected virtual void OnEnable() { }
        public virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual void OnEnd() { }        
        
        public virtual TaskStatus OnUpdate()
        {
            if (_status != TaskStatus.IGNORED)
            {
                /*if (_firstRun)
                {
                    OnAwake();
                    _firstRun = false;
                }*/
                if (turnOver)
                {
                    turnOver = false;
                    OnStart();
                }
                _status = Run();
                if (_status != TaskStatus.RUNNING)
                {
                    OnEnd();
                    turnOver = true;
                }
            }
            
            return _status;
        }        

        public virtual void ForceEnd()
        {
            if (_status == TaskStatus.RUNNING)
            {
                _status = TaskStatus.FAILURE;
                OnEnd();
                turnOver = true;
            }
        }

        public abstract TaskStatus Run();        
        public virtual float GetPriority() { return 0; }

        public virtual void OnDrawGizmos() { }

        public virtual void OnDrawGizmosSelected() { }


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