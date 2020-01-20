using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Seek.png")]
    public class Seek : Action<SeekImpl> { }

    [System.Serializable]
    public class SeekImpl : NavMeshLocomotionImpl
    {
        [SerializeField]
        protected SharedGameObject target;
        [SerializeField]
        protected Vector3 targetPosition;

        Locomotion.Seek _seek = new Locomotion.Seek();

        public override void OnStart()
        {
            base.OnStart();
            _seek.agent = Transform;
            _seek.navMeshAgent = navMeshAgent;
            _seek.speed = speed;
            _seek.angularSpeed = angularSpeed;
            _seek.arriveDistance = arriveDistance;
            _seek.stopOnEnd = stopOnEnd;
            _seek.updateRotation = updateRotation;

            if (target.Value != null)
            {
                _seek.target = target.Value.transform;
            }            
            _seek.targetPosition = targetPosition;

            _seek.OnStart();
        }

        public override TaskStatus Run()
        {
            if (target.Value != null)
            {
                _seek.target = target.Value.transform;
            }
            _seek.targetPosition = targetPosition;
            switch (_seek.OnUpdate())
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