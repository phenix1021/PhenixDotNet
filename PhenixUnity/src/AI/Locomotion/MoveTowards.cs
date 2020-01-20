using UnityEngine;

namespace Phenix.Unity.AI.Locomotion
{
    public class MoveTowards : Locomotion
    {        
        public float speed = 10;
        public float angularSpeed = 120;
        public float arriveDistance = 0.1f;        
        public bool lookAtTarget = true;
        
        public Transform target;        
        public Vector3 targetPosition;

        public override LocomotionStatus OnUpdate()
        {
            var position = Target();
            // Return a task status of success once we've reached the target
            if (Vector3.Magnitude(agent.position - position) < arriveDistance)
            {
                return LocomotionStatus.SUCCESS;
            }

            // We haven't reached the target yet so keep moving towards it
            agent.position = Vector3.MoveTowards(agent.position, position, speed * Time.deltaTime);
            if (lookAtTarget && (position - agent.position).sqrMagnitude > 0.01f)
            {
                agent.rotation = Quaternion.RotateTowards(agent.rotation, Quaternion.LookRotation(position - agent.position), 
                    angularSpeed * Time.deltaTime);
            }

            return LocomotionStatus.RUNNING;
        }

        // Return targetPosition if targetTransform is null
        Vector3 Target()
        {
            if (target == null)
            {
                return targetPosition;
            }
            return target.position;
        }

        // Reset the public variables
        public override void OnReset()
        {
            
        }

        public override void OnStart()
        {
            
        }

        public override void OnEnd()
        {
            
        }
    }
}