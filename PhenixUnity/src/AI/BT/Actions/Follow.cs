using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Follow.png")]
    public class Follow : Action<FollowImpl> { }

    [System.Serializable]
    public class FollowImpl : NavMeshLocomotionImpl
    {
        [SerializeField]
        protected float moveDistance = 2;

        [SerializeField]
        protected SharedGameObject target;

        Locomotion.Follow _follow = new Locomotion.Follow();

        public override void OnStart()
        {
            base.OnStart();
            _follow.agent = Transform;
            _follow.navMeshAgent = navMeshAgent;
            _follow.navMeshObstacle = navMeshObstacle;
            _follow.speed = speed;
            _follow.angularSpeed = angularSpeed;
            _follow.arriveDistance = arriveDistance;
            _follow.stopOnEnd = stopOnEnd;
            _follow.updateRotation = updateRotation;

            _follow.moveDistance = moveDistance;            

            if (target.Value != null)
            {
                _follow.target = target.Value.transform;
            }

            _follow.OnStart();
        }

        public override TaskStatus Run()
        {
            if (target.Value != null)
            {
                _follow.target = target.Value.transform;
            }
            switch (_follow.OnUpdate())
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
            _follow.OnEnd();
        }
    }
}