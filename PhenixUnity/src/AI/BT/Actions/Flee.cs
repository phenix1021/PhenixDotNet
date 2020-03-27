using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Flee.png")]
    public class Flee : Action<FleeImpl> { }

    [System.Serializable]
    public class FleeImpl : NavMeshLocomotionImpl
    {
        [SerializeField]
        protected float fleedDistance = 20;
        [SerializeField]
        protected float lookAheadDistance = 5;
        [SerializeField]
        protected SharedGameObject target;        

        Locomotion.Flee _flee = new Locomotion.Flee();

        public override void OnStart()
        {
            base.OnStart();
            _flee.agent = Transform;
            _flee.navMeshAgent = navMeshAgent;
            _flee.navMeshObstacle = navMeshObstacle;
            _flee.speed = speed;
            _flee.angularSpeed = angularSpeed;
            _flee.arriveDistance = arriveDistance;
            _flee.stopOnEnd = stopOnEnd;
            _flee.updateRotation = updateRotation;

            _flee.fleedDistance = fleedDistance;
            _flee.lookAheadDistance = lookAheadDistance;

            if (target.Value != null)
            {
                _flee.target = target.Value.transform;
            }

            _flee.OnStart();
        }

        public override TaskStatus Run()
        {
            if (target.Value != null)
            {
                _flee.target = target.Value.transform;
            }            
            switch (_flee.OnUpdate())
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
            _flee.OnEnd();
        }
    }
}