using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Wander.png")]
    public class Wander : Action<WanderImpl> { }

    [System.Serializable]
    public class WanderImpl : NavMeshLocomotionImpl
    {
        [SerializeField]
        protected float minWanderDistance = 20;

        [SerializeField]
        protected float maxWanderDistance = 20;

        [SerializeField]
        protected float wanderRate = 2;

        [SerializeField]
        protected float minPauseDuration = 0;

        [SerializeField]
        protected float maxPauseDuration = 0;

        [SerializeField]
        protected int targetRetries = 1;

        Locomotion.Wander _wander = new Locomotion.Wander();

        protected virtual void OnRest() { }
        protected virtual void OnMove() { }

        public override void OnStart()
        {
            base.OnStart();
            _wander.agent = Transform;
            _wander.navMeshAgent = navMeshAgent;
            _wander.speed = speed;
            _wander.angularSpeed = angularSpeed;
            _wander.arriveDistance = arriveDistance;
            _wander.stopOnEnd = stopOnEnd;
            _wander.updateRotation = updateRotation;

            _wander.minWanderDistance = minWanderDistance;
            _wander.maxWanderDistance = maxWanderDistance;
            _wander.wanderRate = wanderRate;
            _wander.minPauseDuration = minPauseDuration;
            _wander.maxPauseDuration = maxPauseDuration;
            _wander.targetRetries = targetRetries;

            _wander.onRest = OnRest;
            _wander.onMove = OnMove;

            _wander.OnStart();
        }

        public override TaskStatus Run()
        {            
            switch (_wander.OnUpdate())
            {
                case Locomotion.LocomotionStatus.RUNNING:
                    return TaskStatus.RUNNING;
                case Locomotion.LocomotionStatus.SUCCESS:
                    return TaskStatus.SUCCESS;
                case Locomotion.LocomotionStatus.FAILURE:
                    return TaskStatus.FAILURE;
                default:
                    return TaskStatus.NONE;
            }
        }
    }
}