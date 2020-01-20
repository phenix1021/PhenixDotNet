using System.Collections.Generic;
using UnityEngine;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.AI.Locomotion
{
    /// <summary>
    /// Check to see if the any objects are within hearing range of the current agent.
    /// </summary>
    public class CanHearObject : Locomotion//NavMeshLocomotion
    {
        // Should the 2D version be used?
        public bool usePhysics2D;

        // The object that we are searching for
        public GameObject targetObject;

        // The objects that we are searching for
        public List<GameObject> targetObjects;

        // The tag of the object that we are searching for
        public string targetTag;

        // The LayerMask of the objects that we are searching for
        public LayerMask objectLayerMask;

        // If using the object layer mask, specifies the maximum number of colliders that the physics cast can collide with
        public int maxCollisionCount = 200;

        // How far away the unit can hear
        public float hearingRadius = 50;

        /*The further away a sound source is the less likely the agent will be able to hear it. 
          Set a threshold for the the minimum audibility level that the agent can hear*/
        public float audibilityThreshold = 0.05f;

        // The hearing offset relative to the pivot position
        public Vector3 offset;

        // The returned object that is heard
        public GameObject returnedObject;

        private Collider[] overlapColliders;
        private Collider2D[] overlap2DColliders;

        public override void OnStart()
        {
            
        }

        // Returns success if an object was found otherwise failure
        public override LocomotionStatus OnUpdate()
        {
            if (targetObjects != null && targetObjects.Count > 0)
            {
                // If there are objects in the group list then search for the object within that list
                GameObject objectFound = null;
                for (int i = 0; i < targetObjects.Count; ++i)
                {
                    float audibility = 0;
                    GameObject obj;
                    if (Vector3.Distance(targetObjects[i].transform.position, agent.position) < hearingRadius)
                    {
                        if ((obj = LocomotionUtility.WithinHearingRange(agent, offset, audibilityThreshold, 
                            targetObjects[i], ref audibility)) != null)
                        {
                            objectFound = obj;
                        }
                    }
                }
                returnedObject = objectFound;
            }
            else if (targetObject == null)
            { 
                // If the target object is null then determine if there are any objects within hearing range based on the layer mask
                if (usePhysics2D)
                {
                    if (overlap2DColliders == null)
                    {
                        overlap2DColliders = new Collider2D[maxCollisionCount];
                    }
                    returnedObject = LocomotionUtility.WithinHearingRange2D(agent, offset, audibilityThreshold,
                        hearingRadius, overlap2DColliders, objectLayerMask);
                }
                else
                {
                    if (overlapColliders == null)
                    {
                        overlapColliders = new Collider[maxCollisionCount];
                    }
                    returnedObject = LocomotionUtility.WithinHearingRange(agent, offset, audibilityThreshold,
                        hearingRadius, overlapColliders, objectLayerMask);
                }
            }
            else
            {
                GameObject target;
                if (!string.IsNullOrEmpty(targetTag))
                {
                    target = GameObject.FindGameObjectWithTag(targetTag);
                }
                else
                {
                    target = targetObject;
                }
                if (Vector3.Distance(target.transform.position, agent.position) < hearingRadius)
                {
                    returnedObject = LocomotionUtility.WithinHearingRange(agent, offset, audibilityThreshold, targetObject);
                }
            }

            if (returnedObject != null)
            {
                // returnedObject success if an object was heard
                return LocomotionStatus.SUCCESS;
            }
            // An object is not within heard so return failure
            return LocomotionStatus.FAILURE;
        }

        // Reset the public variables
        public override void OnReset()
        {
            
        }
/*
        // Draw the hearing radius
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (Owner == null || hearingRadius == null) {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.up, hearingRadius.Value);
            UnityEditor.Handles.color = oldColor;
#endif
        }
        */
        public override void OnEnd()
        {
            TransformTools.Instance.ClearComponentCaches();
        }
    }
}