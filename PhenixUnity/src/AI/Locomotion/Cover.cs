using UnityEngine;

namespace Phenix.Unity.AI.Locomotion
{
    /// <summary>
    /// Find a place to hide and move to it using the Unity NavMesh.
    /// </summary>
    public class Cover : NavMeshLocomotion
    {
        // The distance to search for cover
        public float maxCoverDistance = 1000;

        // The layermask of the available cover positions
        public LayerMask availableLayerCovers;

        // The maximum number of raycasts that should be fired before the agent gives up looking for an agent to find cover behind
        public int maxRaycasts = 100;

        // How large the step should be between raycasts
        public float rayStep = 1;

        // Once a cover point has been found, multiply this offset by the normal to prevent the agent from hugging the wall
        public float coverOffset = 2;

        // Should the agent look at the cover point after it has arrived?
        public bool lookAtCoverPoint = false;

        // The agent is done rotating to the cover point when the square magnitude is less than this value
        public float rotationEpsilon = 0.5f;

        // Max rotation delta if lookAtCoverPoint
        public float maxLookAtRotationDelta;

        Vector3 _coverPoint;
        
        // The position to reach, offsetted from coverPoint
        Vector3 _coverTarget;
        
        // Was cover found?
        bool _foundCover;

        public override void OnStart()
        {
            RaycastHit hit;
            int raycastCount = 0;
            var direction = agent.forward;
            float step = 0;
            _coverTarget = agent.position;
            _foundCover = false;
            // Keep firing a ray until too many rays have been fired
            while (raycastCount < maxRaycasts)
            {
                var ray = new Ray(agent.position, direction);
                if (Physics.Raycast(ray, out hit, maxCoverDistance, availableLayerCovers.value))
                {
                    // A suitable agent has been found. Find the opposite side of that agent by shooting a ray in the opposite direction from a point far away
                    if (hit.collider.Raycast(new Ray(hit.point - hit.normal * maxCoverDistance, hit.normal), out hit, Mathf.Infinity))
                    {
                        _coverPoint = hit.point;
                        _coverTarget = hit.point + hit.normal * coverOffset;
                        _foundCover = true;
                        break;
                    }
                }
                // Keep sweeiping along the y axis
                step += rayStep;
                direction = Quaternion.Euler(0, agent.eulerAngles.y + step, 0) * Vector3.forward;
                raycastCount++;
            }

            if (_foundCover)
            {
                SetDestination(_coverTarget);
            }

            base.OnStart();
        }

        // Seek to the cover point. Return success as soon as the location is reached or the agent is looking at the cover point
        public override LocomotionStatus OnUpdate()
        {
            if (!_foundCover)
            {
                return LocomotionStatus.FAILURE;
            }
            if (HasArrived())
            {
                var rotation = Quaternion.LookRotation(_coverPoint - agent.position);
                // Return success if the agent isn't going to look at the cover point or it has completely rotated to look at the cover point
                if (!lookAtCoverPoint || Quaternion.Angle(agent.rotation, rotation) < rotationEpsilon)
                {
                    return LocomotionStatus.SUCCESS;
                }
                else
                {
                    // Still needs to rotate towards the target
                    agent.rotation = Quaternion.RotateTowards(agent.rotation, rotation, maxLookAtRotationDelta);
                }
            }
            UpdateNavMeshObstacle();
            return LocomotionStatus.RUNNING;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnStart();
        }
    }
}