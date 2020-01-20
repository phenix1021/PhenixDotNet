using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Cover.png")]
    public class Cover : Action<CoverImpl> { }

    [System.Serializable]
    public class CoverImpl : NavMeshLocomotionImpl
    {
        [SerializeField]
        protected float maxCoverDistance = 1000;
        [SerializeField]
        protected LayerMask availableLayerCovers;
        [SerializeField]
        protected int maxRaycasts = 100;
        [SerializeField]
        protected float rayStep = 1;
        [SerializeField]
        protected float coverOffset = 2;
        [SerializeField]
        protected bool lookAtCoverPoint = false;
        [SerializeField]
        protected float rotationEpsilon = 0.5f;
        [SerializeField]
        protected float maxLookAtRotationDelta;

        Locomotion.Cover _cover = new Locomotion.Cover();

        public override void OnStart()
        {
            base.OnStart();
            _cover.agent = Transform;
            _cover.navMeshAgent = navMeshAgent;
            _cover.speed = speed;
            _cover.angularSpeed = angularSpeed;
            _cover.arriveDistance = arriveDistance;
            _cover.stopOnEnd = stopOnEnd;
            _cover.updateRotation = updateRotation;

            _cover.maxCoverDistance = maxCoverDistance;
            _cover.availableLayerCovers = availableLayerCovers;
            _cover.maxRaycasts = maxRaycasts;
            _cover.rayStep = rayStep;
            _cover.coverOffset = coverOffset;
            _cover.rotationEpsilon = rotationEpsilon;
            _cover.lookAtCoverPoint = lookAtCoverPoint;
            _cover.maxLookAtRotationDelta = maxLookAtRotationDelta;

            _cover.OnStart();
        }

        public override TaskStatus Run()
        {
            switch (_cover.OnUpdate())
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