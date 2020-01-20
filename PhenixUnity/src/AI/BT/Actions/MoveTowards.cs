using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/MoveTowards.png")]
    public class MoveTowards : Action<MoveTowardsImpl> { }

    [System.Serializable]
    public class MoveTowardsImpl : ActionImpl
    {
        [SerializeField]
        protected float speed = 10;

        [SerializeField]
        protected float angularSpeed = 120;

        [SerializeField]
        protected float arriveDistance = 0.5f;

        [SerializeField]
        protected bool lookAtTarget = true;

        [SerializeField]
        protected SharedGameObject target;        

        Locomotion.MoveTowards _moveTowards = new Locomotion.MoveTowards();

        public override void OnStart()
        {
            base.OnStart();

            _moveTowards.agent = Transform;
            _moveTowards.speed = speed;
            _moveTowards.angularSpeed = angularSpeed;
            _moveTowards.arriveDistance = arriveDistance;
            _moveTowards.lookAtTarget = lookAtTarget;

            _moveTowards.OnStart();
        }

        public override TaskStatus Run()
        {
            if (target.Value == null)
            {
                return TaskStatus.RUNNING;
            }
            else
            {
                _moveTowards.target = target.Value.transform;                
                switch (_moveTowards.OnUpdate())
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
}