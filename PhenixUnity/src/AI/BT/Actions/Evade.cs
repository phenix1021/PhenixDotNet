using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Evade.png")]
    public class Evade : Action<EvadeImpl> { }

    [System.Serializable]
    public class EvadeImpl : NavMeshLocomotionImpl
    {
        [SerializeField]
        protected float evadeDistance = 10;
        [SerializeField]
        protected float lookAheadDistance = 5;
        [SerializeField]
        protected float targetDistPrediction = 20;
        [SerializeField]
        protected float targetDistPredictionMult = 20;
        [SerializeField]
        protected SharedGameObject target;

        Locomotion.Evade _evade = new Locomotion.Evade();

        public override void OnStart()
        {
            base.OnStart();
            _evade.agent = Transform;
            _evade.navMeshAgent = navMeshAgent;
            _evade.navMeshObstacle = navMeshObstacle;
            _evade.speed = speed;
            _evade.angularSpeed = angularSpeed;
            _evade.arriveDistance = arriveDistance;
            _evade.stopOnEnd = stopOnEnd;
            _evade.updateRotation = updateRotation;

            _evade.evadeDistance = evadeDistance;
            _evade.lookAheadDistance = lookAheadDistance;
            _evade.targetDistPrediction = targetDistPrediction;
            _evade.targetDistPredictionMult = targetDistPredictionMult;

            if (target.Value != null)
            {
                _evade.target = target.Value.transform;
            }

            _evade.OnStart();
        }

        public override TaskStatus Run()
        {
            if (target.Value != null)
            {
                _evade.target = target.Value.transform;
            }

            switch (_evade.OnUpdate())
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
            _evade.OnEnd();
        }
    }
}