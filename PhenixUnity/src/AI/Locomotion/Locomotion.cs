using UnityEngine;

namespace Phenix.Unity.AI.Locomotion
{
    public enum LocomotionStatus
    {
        NONE = -1,
        RUNNING = 0,
        SUCCESS = 1,
        FAILURE = 2,
    }

    public abstract class Locomotion
    {
        public Transform agent;        

        public abstract void OnStart();

        public abstract LocomotionStatus OnUpdate();

        public abstract void OnEnd();

        public abstract void OnReset();
        public virtual void OnDrawGizmos() { }
        public virtual void OnDrawGizmosSelected() { }
    }
}