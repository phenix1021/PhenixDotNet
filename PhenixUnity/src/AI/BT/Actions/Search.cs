using Phenix.Unity.AI.Locomotion;
using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Search.png")]
    public class Search : Action<SearchImpl> { }

    [System.Serializable]
    public class SearchImpl : NavMeshLocomotionImpl
    {
        [SerializeField]
        protected float minWanderDistance = 20;
        [SerializeField]
        protected float maxWanderDistance = 20;
        [SerializeField]
        protected float wanderRate = 1;
        [SerializeField]
        protected float minPauseDuration = 0;
        [SerializeField]
        protected float maxPauseDuration = 0;
        [SerializeField]
        protected int targetRetries = 1;
        [SerializeField]
        protected float fieldOfViewAngle = 90;
        [SerializeField]
        protected float viewDistance = 30;
        [SerializeField]
        protected LayerMask ignoreLayerMask;// = 1 << LayerMask.NameToLayer("Ignore Raycast");
        [SerializeField]
        protected bool senseSight = true;
        [SerializeField]
        protected bool senseAudio = true;
        [SerializeField]
        protected float hearingRadius = 30;
        [SerializeField]
        protected Vector3 positionOffset;
        [SerializeField]
        protected Vector3 targetOffset;
        [SerializeField]
        protected LayerMask objectLayerMask;
        [SerializeField]
        protected int maxCollisionCount = 200;
        [SerializeField]
        protected bool useTargetBone;
        [SerializeField]
        protected HumanBodyBones targetBone;
        [SerializeField]
        protected float audibilityThreshold = 0.05f;

        [SerializeField]
        protected SharedGameObject returnedObject;        

        protected virtual void OnRest() { }
        protected virtual void OnMove() { }

        Locomotion.Search _search = new Locomotion.Search();

        public override void OnStart()
        {
            base.OnStart();
            _search.agent = Transform;            
            _search.navMeshAgent = navMeshAgent;
            _search.navMeshObstacle = navMeshObstacle;
            _search.speed = speed;
            _search.angularSpeed = angularSpeed;
            _search.arriveDistance = arriveDistance;
            _search.stopOnEnd = stopOnEnd;
            _search.updateRotation = updateRotation;

            _search.minWanderDistance = minWanderDistance;
            _search.maxWanderDistance = maxWanderDistance;
            _search.wanderRate = wanderRate;
            _search.minPauseDuration = minPauseDuration;
            _search.maxPauseDuration = maxPauseDuration;
            _search.targetRetries = targetRetries;
            _search.fieldOfViewAngle = fieldOfViewAngle;
            _search.viewDistance = viewDistance;
            _search.ignoreLayerMask = ignoreLayerMask;
            _search.senseSight = senseSight;
            _search.senseAudio = senseAudio;
            _search.hearingRadius = hearingRadius;
            _search.positionOffset = positionOffset;
            _search.targetOffset = targetOffset;
            _search.objectLayerMask = objectLayerMask;
            _search.maxCollisionCount = maxCollisionCount;
            _search.useTargetBone = useTargetBone;
            _search.targetBone = targetBone;
            _search.audibilityThreshold = audibilityThreshold;            

            _search.onRest = OnRest;
            _search.onMove = OnMove;

            _search.OnStart();
        }

        public override TaskStatus Run()
        {            
            switch (_search.OnUpdate())
            {
                case Locomotion.LocomotionStatus.RUNNING:
                    return TaskStatus.RUNNING;
                case Locomotion.LocomotionStatus.SUCCESS:
                    returnedObject.Value = _search.returnedObject;
                    return TaskStatus.SUCCESS;
                case Locomotion.LocomotionStatus.FAILURE:
                    return TaskStatus.FAILURE;
                default:
                    return TaskStatus.NONE;
            }
        }

        public override void OnDrawGizmosSelected()
        {
            _search.OnDrawGizmosSelected();
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _search.OnEnd();
        }
    }
}