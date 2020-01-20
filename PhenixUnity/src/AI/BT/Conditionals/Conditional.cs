﻿using System;
using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    public abstract class Conditional : Task { }
    public abstract class Conditional<T> : Conditional where T : ConditionalImpl, new()
    {
        public T taskParams = new T();

        public override void OnAwake()
        {
            taskParams.Transform = BT.Transform;
            taskParams.OnAwake();
        }

        /*public override void OnStart()
        {
            taskParams.OnStart();
        }*/

        public sealed override TaskStatus Run()
        {
            if (taskParams.Check())
            {
                Status = TaskStatus.SUCCESS;
            }
            else
            {
                Status = TaskStatus.FAILURE;
            }
            return Status;
        }

        public sealed override TaskStatus OnUpdate()
        {
            return base.OnUpdate();
        }

        protected sealed override void OnFirstRun()
        {
            taskParams.OnFirstRun();
        }

        protected sealed override void OnStart()
        {            
            taskParams.OnStart();
        }

        protected sealed override void OnEnd()
        {
            taskParams.OnTurnEnd();
        }

        public sealed override void OnDrawGizmos()
        {
            taskParams.OnDrawGizmos();
        }

        public sealed override void OnDrawGizmosSelected()
        {
            taskParams.OnDrawGizmosSelected();
        }
    }

    [Serializable]
    public abstract class ConditionalImpl
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
        public abstract bool Check();
        public virtual void OnDrawGizmos() { }
        public virtual void OnDrawGizmosSelected() { }
    }
}