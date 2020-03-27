using UnityEngine;

namespace Phenix.Unity.AI.Locomotion
{    
    public class Follow : NavMeshLocomotion
    {        
        public Transform target;
        
        // Start moving towards the target if the target is further than the specified distance
        public float moveDistance = 2;

        Vector3 _lastTargetPosition;
        bool _hasMoved;

        public override void OnStart()
        {
            base.OnStart();

            if (target == null)
            {
                return;
            }

            _lastTargetPosition = target.transform.position + Vector3.one * (moveDistance + 1);
            _hasMoved = false;
        }

        // Follow the target. The task will never return success as the agent should continue to follow the target even after arriving at the destination.
        public override LocomotionStatus OnUpdate()
        {
            if (target == null)
            {
                return LocomotionStatus.FAILURE;
            }

            // Move if the target has moved more than the moveDistance since the last time the agent moved.
            var targetPosition = target.position;
            if ((targetPosition - _lastTargetPosition).magnitude >= moveDistance)
            {
                SetDestination(targetPosition);
                _lastTargetPosition = targetPosition;
                _hasMoved = true;
            }
            else
            {
                // Stop moving if the agent is within the moveDistance of the target.
                if (_hasMoved && (targetPosition - agent.position).magnitude < moveDistance)
                {
                    Stop();
                    _hasMoved = false;
                    _lastTargetPosition = targetPosition;
                }
            }
            UpdateNavMeshObstacle();
            return LocomotionStatus.RUNNING;
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;            
        }
    }
}