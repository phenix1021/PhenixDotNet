using UnityEngine;

namespace Phenix.Unity.AI.Locomotion
{
    /// <summary>
    /// 和Flee类似，区别在于Evade会进行方位预判
    /// </summary>
    public class Evade : NavMeshLocomotion
    {
        // The agent has evaded when the magnitude is greater than this value
        public float evadeDistance = 10;
        // The distance to look ahead when evading
        public float lookAheadDistance = 5;
        // How far to predict the distance ahead of the target. Lower values indicate less distance should be predicated
        public float targetDistPrediction = 20;
        // Multiplier for predicting the look ahead distance
        public float targetDistPredictionMult = 20;
        // The GameObject that the agent is evading
        public Transform target;

        // The position of the target at the last frame
        Vector3 _targetPosition;

        public override void OnStart()
        {
            base.OnStart();
            _targetPosition = target.position;
            SetDestination(Target());
        }

        // Evade from the target. Return success once the agent has fleed the target by moving far enough away from it
        // Return running if the agent is still fleeing
        public override LocomotionStatus OnUpdate()
        {
            if (Vector3.Magnitude(agent.position - target.position) > evadeDistance)
            {
                return LocomotionStatus.SUCCESS;
            }

            SetDestination(Target());

            return LocomotionStatus.RUNNING;
        }

        // Evade in the opposite direction
        private Vector3 Target()
        {
            // Calculate the current distance to the target and the current speed
            var distance = (target.position - agent.position).magnitude;
            var speed = Velocity().magnitude;

            float futurePrediction = 0;
            // Set the future prediction to max prediction if the speed is too small to give an accurate prediction
            if (speed <= distance / targetDistPrediction)
            {
                futurePrediction = targetDistPrediction;
            }
            else
            {
                futurePrediction = (distance / speed) * targetDistPredictionMult; // the prediction should be accurate enough
            }

            // Predict the future by taking the velocity of the target and multiply it by the future prediction
            var prevTargetPosition = _targetPosition;
            _targetPosition = target.position;
            var position = _targetPosition + (_targetPosition - prevTargetPosition) * futurePrediction;

            return agent.position + (agent.position - position).normalized * lookAheadDistance;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();
            target = null;
        }
    }
}