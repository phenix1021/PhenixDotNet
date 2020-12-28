using UnityEngine;
using UnityEngine.AI;
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
        public UnityAction onArrived;

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
                if (onArrived != null)
                {
                    onArrived.Invoke();
                }

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
            Vector3 rlt = Vector3.zero;
            if (target != null)
            {
                rlt = target.position;
            }
            else
            {
                rlt = targetPosition;
            }

            NavMeshPath path = new NavMeshPath();
            navMeshAgent.CalculatePath(rlt, path);
            if (path.status == NavMeshPathStatus.PathInvalid)
            {
                rlt = navMeshAgent.transform.position;
            }
            else
            {
                rlt = path.corners[path.corners.Length - 1];
            }

            return rlt;
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            targetPosition = Vector3.zero;
        }
    }
}