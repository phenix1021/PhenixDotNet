using UnityEngine;

namespace Phenix.Unity.AI.Locomotion
{
    public class Flee : NavMeshLocomotion
    {
        // The agent has fleed when the magnitude is greater than this value
        public float fleedDistance = 20;
        // The distance to look ahead when fleeing
        public float lookAheadDistance = 5;
        
        public Transform target;

        private bool _hasMoved;

        public override void OnStart()
        {
            base.OnStart();
            _hasMoved = false;
            SetDestination(Target());
        }

        // Flee from the target. Return success once the agent has fleed the target by moving far enough away from it
        // Return running if the agent is still fleeing
        public override LocomotionStatus OnUpdate()
        {
            if (Vector3.Magnitude(agent.position - target.position) > fleedDistance)
            {
                return LocomotionStatus.SUCCESS;
            }

            if (HasArrived())
            {
                if (!_hasMoved)
                {
                    return LocomotionStatus.FAILURE;
                }
                if (!SetDestination(Target()))
                {
                    return LocomotionStatus.FAILURE;
                }
                _hasMoved = false;
            }
            else
            {
                // If the agent is stuck the task shouldn't continue to return a status of running.
                var velocityMagnitude = Velocity().sqrMagnitude;
                if (_hasMoved && velocityMagnitude <= 0f)
                {
                    return LocomotionStatus.FAILURE;
                }
                _hasMoved = velocityMagnitude > 0f;
            }
            UpdateNavMeshObstacle();
            return LocomotionStatus.RUNNING;
        }

        // Flee in the opposite direction
        private Vector3 Target()
        {
            return agent.position + (agent.position - target.position).normalized * lookAheadDistance;
        }

        // Return false if the position isn't valid on the NavMesh.
        protected override bool SetDestination(Vector3 destination)
        {
            if (!SamplePosition(ref destination))
            {
                return false;
            }
            return base.SetDestination(destination);
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();            
            target = null;
        }
    }
}