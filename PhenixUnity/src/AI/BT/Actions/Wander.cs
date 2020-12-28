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

        public bool fixedCenter;    // 是否固定wander中心点位置，即wander范围限于agent初始位置为中心，wanderDistance为半径的圆内

        Locomotion.Wander _wander;

        protected virtual void OnRest() { }
        protected virtual void OnMove() { }

        public override void OnAwake()
        {
            base.OnAwake();
            _wander = new Locomotion.Wander(Transform.position);
        }

        public override void OnStart()
        {
            base.OnStart();
            _wander.agent = Transform;
            _wander.navMeshAgent = navMeshAgent;
            _wander.navMeshObstacle = navMeshObstacle;
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

            _wander.fixedCenter = fixedCenter;
            
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

        public override void OnEnd()
        {
            base.OnEnd();
            _wander.OnEnd();
        }
    }
}