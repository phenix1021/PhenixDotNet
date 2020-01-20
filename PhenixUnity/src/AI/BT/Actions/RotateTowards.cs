using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/RotateTowards.png")]
    public class RotateTowards : Action<RotateTowardsImpl> { }

    [System.Serializable]
    public class RotateTowardsImpl : ActionImpl
    {
        [SerializeField]
        protected float angularSpeed = 120;

        [SerializeField]
        // The agent is done rotating when the angle is less than this value
        protected float rotationEpsilon = 0.5f;

        [SerializeField]
        // Should the rotation to target only affect the Y axis?
        protected bool onlyYForTarget;

        [SerializeField]
        protected SharedGameObject target;

        Locomotion.RotateTowards _rotateTowards = new Locomotion.RotateTowards();

        public override void OnStart()
        {
            base.OnStart();

            _rotateTowards.agent = Transform;
            _rotateTowards.angularSpeed = angularSpeed;
            _rotateTowards.rotationEpsilon = rotationEpsilon;
            _rotateTowards.onlyYForTarget = onlyYForTarget;

            _rotateTowards.OnStart();
        }

        public override TaskStatus Run()
        {
            if (target.Value == null)
            {
                return TaskStatus.RUNNING;
            }
            else
            {
                _rotateTowards.target = target.Value.transform;
                switch (_rotateTowards.OnUpdate())
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