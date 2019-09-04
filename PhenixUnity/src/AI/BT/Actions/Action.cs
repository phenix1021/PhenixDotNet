using UnityEngine;
using System;

namespace Phenix.Unity.AI
{
    /*public abstract class Action : Task
    {
        public sealed override TaskStatus OnUpdate()
        {
            return base.OnUpdate();
        }
    }

    [System.Serializable]
    public abstract class ActionParams { }

    public abstract class Action<T> : Action where T : ActionParams
    {
        public ActionParams actionParams;
    }*/

    public abstract class Action : Task { }

    public abstract class Action<T> : Action where T : ActionImpl, new()
    {
        public T taskParams = new T();

        public override void OnAwake()
        {
            taskParams.Transform = BT.Transform;
            taskParams.OnAwake();
        }

        public override void OnStart()
        {
            taskParams.OnStart();
        }

        protected sealed override void OnFirstRun()
        {            
            taskParams.OnFirstRun();
        }


        protected sealed override void OnTurnBegin()
        {            
            taskParams.OnTurnBegin();            
        }

        protected sealed override void OnTurnEnd()
        {
            taskParams.OnTurnEnd();
        }

        public sealed override TaskStatus OnUpdate()
        {
            return base.OnUpdate();
        }

        public sealed override TaskStatus Run()
        {
            Status = taskParams.Run();
            return Status;
        }

        public override float GetPriority()
        {
            return taskParams.GetPriority();
        }

       /* public void OnFixedUpdate()
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

    [Serializable]
    public abstract class ActionImpl
    {
        Transform _transform;

        public Transform Transform { get { return _transform; } set { _transform = value; } }
        public GameObject GameObject { get { return _transform != null ? _transform.gameObject : null; } }
        public T GetComponent<T>() where T : Component
        {
            if (_transform != null)
            {
                return _transform.GetComponent<T>();
            }
            return null;
        }

        public Component GetComponent(Type type)
        {
            if (_transform != null)
            {
                return _transform.GetComponent(type);
            }
            return null;
        }

        public virtual void OnAwake() { }
        public virtual void OnStart() { }
        public virtual void OnFirstRun() { }
        public virtual void OnTurnBegin() { }
        public virtual void OnTurnEnd() { }        
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
