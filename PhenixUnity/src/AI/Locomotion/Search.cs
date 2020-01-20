using UnityEngine;
using UnityEngine.Events;

namespace Phenix.Unity.AI.Locomotion
{
    // Search for a target by combining the wander, within hearing range, and the within seeing range tasks using the Unity NavMesh.    
    public class Search : NavMeshLocomotion
    {
        // Minimum distance ahead of the current position to look ahead for a destination
        public float minWanderDistance = 20;
        
        // Maximum distance ahead of the current position to look ahead for a destination
        public float maxWanderDistance = 20;
        
        // The amount that the agent rotates direction
        public float wanderRate = 1;

        // The minimum length of time that the agent should pause at each destination
        public float minPauseDuration = 0;

        // The maximum length of time that the agent should pause at each destination (zero to disable)
        public float maxPauseDuration = 0;

        // The maximum number of retries per tick (set higher if using a slow tick time)
        public int targetRetries = 1;

        // The field of view angle of the agent (in degrees)
        public float fieldOfViewAngle = 90;

        // The distance that the agent can see
        public float viewDistance = 30;

        // The LayerMask of the objects to ignore when performing the line of sight check
        public LayerMask ignoreLayerMask;// = 1 << LayerMask.NameToLayer("Ignore Raycast");

        // Should the search end if audio was heard?
        public bool senseAudio = true;

        // How far away the unit can hear
        public float hearingRadius = 30;

        // The raycast offset relative to the pivot position
        public Vector3 positionOffset;

        // The target raycast offset relative to the pivot position
        public Vector3 targetOffset;

        // The LayerMask of the objects that we are searching for
        public LayerMask objectLayerMask;

        // Specifies the maximum number of colliders that the physics cast can collide with
        public int maxCollisionCount = 200;

        // Should the target bone be used?
        public bool useTargetBone;

        // The target's bone if the target is a humanoid
        public HumanBodyBones targetBone;

        /*The further away a sound source is the less likely the agent will be able to hear it. 
          Set a threshold for the the minimum audibility level that the agent can hear*/
        public float audibilityThreshold = 0.05f;
        
        // The object that is found
        public GameObject returnedObject;

        float _pauseTime;
        float _destinationReachTime;

        Collider[] _overlapColliders;

        bool _targetSetting = false;

        public UnityAction onRest;
        public UnityAction onMove;

        // Keep searching until an object is seen or heard (if senseAudio is enabled)
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

            // Detect if any objects are within sight
            if (_overlapColliders == null)
            {
                _overlapColliders = new Collider[maxCollisionCount];
            }

            returnedObject = LocomotionUtility.WithinSight(agent, positionOffset, fieldOfViewAngle, viewDistance, _overlapColliders, 
                objectLayerMask, targetOffset, ignoreLayerMask, useTargetBone, targetBone);
            
            // If an object was seen then return success
            if (returnedObject != null)
            {
                return LocomotionStatus.SUCCESS;
            }
            
            // Detect if any object are within audio range (if enabled)
            if (senseAudio)
            {
                returnedObject = LocomotionUtility.WithinHearingRange(agent, positionOffset, audibilityThreshold, hearingRadius, 
                    _overlapColliders, objectLayerMask);
                // If an object was heard then return success
                if (returnedObject != null)
                {
                    return LocomotionStatus.SUCCESS;
                }
            }

            // No object has been seen or heard so keep searching
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

        public override void OnDrawGizmos()
        {
            LocomotionUtility.DrawViewOfSight(agent, positionOffset, fieldOfViewAngle, 0, viewDistance, false);
        }

        public override void OnDrawGizmosSelected()
        {
            LocomotionUtility.DrawViewOfSight(agent, positionOffset, fieldOfViewAngle, 0, viewDistance, false);
        }
    }
}