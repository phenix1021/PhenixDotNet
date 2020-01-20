using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Patrol.png")]
    public class Patrol : Action<PatrolImpl> { }

    [System.Serializable]
    public class PatrolImpl : NavMeshLocomotionImpl
    {
        [SerializeField]
        protected bool randomPatrol = false;
        [SerializeField]
        protected float waypointPauseDuration = 0;
        [SerializeField]
        protected SharedGameObject[] waypoints;

        Locomotion.Patrol _patrol = new Locomotion.Patrol();

        public override void OnStart()
        {
            base.OnStart();
            _patrol.agent = Transform;
            _patrol.navMeshAgent = navMeshAgent;
            _patrol.speed = speed;
            _patrol.angularSpeed = angularSpeed;
            _patrol.arriveDistance = arriveDistance;
            _patrol.stopOnEnd = stopOnEnd;
            _patrol.updateRotation = updateRotation;

            _patrol.randomPatrol = randomPatrol;
            _patrol.waypointPauseDuration = waypointPauseDuration;            
            foreach (var waypoint in waypoints)
            {
                _patrol.waypoints.Add(waypoint.Value.gameObject);
            }

            _patrol.OnStart();
        }

        public override TaskStatus Run()
        {
            switch (_patrol.OnUpdate())
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