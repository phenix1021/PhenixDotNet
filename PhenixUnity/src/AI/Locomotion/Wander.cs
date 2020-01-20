using UnityEngine;
using UnityEngine.Events;

namespace Phenix.Unity.AI.Locomotion
{    
    public class Wander : NavMeshLocomotion
    {
        // Minimum distance ahead of the current position to look ahead for a destination
        public float minWanderDistance = 20;

        // Maximum distance ahead of the current position to look ahead for a destination
        public float maxWanderDistance = 20;

        // The amount that the agent rotates direction
        public float wanderRate = 2;

        // The minimum length of time that the agent should pause at each destination
        public float minPauseDuration = 0;

        // The maximum length of time that the agent should pause at each destination (zero to disable)
        public float maxPauseDuration = 0;

        // The maximum number of retries per tick (set higher if using a slow tick time)
        public int targetRetries = 1;

        float _pauseTime;
        float _destinationReachTime;
        bool _targetSetting = false;

        public UnityAction onRest;
        public UnityAction onMove;

        // There is no success or fail state with wander - the agent will just keep wandering
        public override LocomotionStatus OnUpdate()
        {
            if (HasArrived())
            {
                // The agent should pause at the destination only if the max pause duration is greater than 0
                if (maxPauseDuration > 0)
                {
                    if (_destinationReachTime == -1)
                    {
                        _destinationReachTime = Time.time;
                        _pauseTime = Random.Range(minPauseDuration, maxPauseDuration);
                        if (onRest != null)
                        {
                            onRest.Invoke();
                        }
                    }

                    if (_destinationReachTime + _pauseTime <= Time.time)
                    {
                        SetTarget();
                    }
                }
                else
                {
                    SetTarget();                   
                }
            }
            return LocomotionStatus.RUNNING;
        }

        void SetTarget()
        {
            if (TrySetTarget())
            {
                _destinationReachTime = -1;
                _targetSetting = false;
                if (onMove != null)
                {
                    onMove.Invoke();
                }
            }
            else if (_targetSetting == false)
            {
                _targetSetting = true;
                if (onRest != null)
                {
                    onRest.Invoke();
                }
            }
        }

        bool TrySetTarget()
        {
            var direction = agent.forward;
            var validDestination = false;
            var attempts = targetRetries;
            var destination = agent.position;
            while (!validDestination && attempts > 0)
            {
                direction = direction + Random.insideUnitSphere * wanderRate;
                destination = agent.position + direction.normalized * Random.Range(minWanderDistance, maxWanderDistance);
                validDestination = SamplePosition(destination);
                attempts--;
            }
            if (validDestination)
            {
                SetDestination(destination);
            }
            return validDestination;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();
        }
    }
}