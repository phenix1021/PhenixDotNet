using UnityEngine;
using UnityEngine.AI;

namespace Phenix.Unity.AI.BT
{   
    [System.Serializable]
    public class NavMeshLocomotionImpl : ActionImpl
    {
        protected NavMeshAgent navMeshAgent;

        [SerializeField]
        protected float speed = 10;
        [SerializeField]
        protected float angularSpeed = 120;
        [SerializeField]
        protected float arriveDistance = 0.5f;
        [SerializeField]
        protected bool stopOnEnd = true;
        [SerializeField]
        protected bool updateRotation = true;

        public override void OnStart()
        {
            base.OnStart();
            navMeshAgent = Transform.GetComponent<NavMeshAgent>();
        }

        public override TaskStatus Run()
        {
            return TaskStatus.NONE;
        }
    }
}