using UnityEngine;

namespace Phenix.Unity.AI.Locomotion
{    
    public class RotateTowards : Locomotion
    {
        public float angularSpeed = 120;

        // The agent is done rotating when the angle is less than this value
        public float rotationEpsilon = 0.5f;
        
        // Should the rotation to target only affect the Y axis?
        public bool onlyYForTarget;
        
        public Transform target;

        // If target is null then use the target rotation
        public Vector3 targetRotation;

        public override LocomotionStatus OnUpdate()
        {
            var rotation = Target();
            // Return a task status of success once we are done rotating
            if (Quaternion.Angle(agent.rotation, rotation) < rotationEpsilon) {
                return LocomotionStatus.SUCCESS;
            }
            // We haven't reached the target yet so keep rotating towards it
            agent.rotation = Quaternion.RotateTowards(agent.rotation, rotation, angularSpeed * Time.deltaTime);
            return LocomotionStatus.RUNNING;
        }

        // Return targetPosition if targetTransform is null
        Quaternion Target()
        {
            if (target == null)
            {
                return Quaternion.Euler(targetRotation);
            }
            var position = target.position - agent.position;
            if (onlyYForTarget)
            {
                position.y = 0;
            }
            return Quaternion.LookRotation(position);
        }

        // Reset the public variables
        public override void OnReset()
        {            
            target = null;            
        }

        public override void OnStart()
        {
         
        }

        public override void OnEnd()
        {
         
        }
    }
}