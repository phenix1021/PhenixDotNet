using UnityEngine;
using UnityEngine.Events;

namespace Phenix.Unity.AI.Locomotion
{    
    public class Seek : NavMeshLocomotion
    {
        // The GameObject that the agent is seeking
        public Transform target;

        // If target is null then use the target position
        public Vector3 targetPosition;

        public UnityAction onMove;
        public UnityAction onMoving;

        public override void OnStart()
        {
            base.OnStart();
            SetDestination(Target());
            if (onMove != null)
            {
                onMove.Invoke();
            }
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override LocomotionStatus OnUpdate()
        {
            if (HasArrived())
            {
                return LocomotionStatus.SUCCESS;
            }
            SetDestination(Target());
            if (onMoving != null)
            {
                onMoving.Invoke();
            }
            UpdateNavMeshObstacle();
            return LocomotionStatus.RUNNING;
        }
        
        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (target != null)
            {
                return target.position;
            }
            return targetPosition;
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            targetPosition = Vector3.zero;
        }
    }
}