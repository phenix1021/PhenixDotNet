using System.Collections.Generic;
using UnityEngine;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.AI.Locomotion
{
    /// <summary>
    /// Check to see if the any objects are within sight of the agent.
    /// </summary>
    public class CanSeeObject : Locomotion//NavMeshLocomotion
    {
        // Should the 2D version be used?
        public bool usePhysics2D;

        // The object that we are searching for
        public Transform targetObject;

        // The objects that we are searching for
        public List<Transform> targetObjects;

        // The tag of the object that we are searching for
        public string targetTag;

        // The LayerMask of the objects that we are searching for
        public LayerMask objectLayerMask;

        // If using the object layer mask, specifies the maximum number of colliders that the physics cast can collide with
        public int maxCollisionCount = 200;

        // The LayerMask of the objects to ignore when performing the line of sight check
        public LayerMask ignoreLayerMask;// = 1 << LayerMask.NameToLayer("Ignore Raycast");

        // The field of view angle of the agent (in degrees)
        public float fieldOfViewAngle = 90;

        // The distance that the agent can see
        public float viewDistance = 1000;
        
        // The raycast offset relative to the pivot position
        public Vector3 positionOffset;

        // The target raycast offset relative to the pivot position
        public Vector3 targetOffset;

        // The offset to apply to 2D angles
        public float angleOffset2D;
        
        // Should the target bone be used?
        public bool useTargetBone;

        // The target's bone if the target is a humanoid
        public HumanBodyBones targetBone;

        // Should the agent's layer be disabled before the Can See Object check is executed?
        public bool disableAgentColliderLayer;
        
        // The object that is within sight
        public GameObject returnedObject;

        GameObject[] _agentColliderGameObjects;
        int[] _originalColliderLayer;
        Collider[] _overlapColliders;
        Collider2D[] _overlap2DColliders;

        int _ignoreRaycastLayer;// = LayerMask.NameToLayer("Ignore Raycast");

        public override void OnStart()
        {

        }

        // Returns success if an object was found otherwise failure
        public override LocomotionStatus OnUpdate()
        {
            // The collider layers on the agent can be set to ignore raycast to prevent them from interferring with the raycast checks.
            if (disableAgentColliderLayer)
            {
                if (_agentColliderGameObjects == null)
                {
                    if (usePhysics2D)
                    {
                        var colliders = agent.GetComponentsInChildren<Collider2D>();
                        _agentColliderGameObjects = new GameObject[colliders.Length];
                        for (int i = 0; i < _agentColliderGameObjects.Length; ++i)
                        {
                            _agentColliderGameObjects[i] = colliders[i].gameObject;
                        }
                    }
                    else
                    {
                        var colliders = agent.GetComponentsInChildren<Collider>();
                        _agentColliderGameObjects = new GameObject[colliders.Length];
                        for (int i = 0; i < _agentColliderGameObjects.Length; ++i)
                        {
                            _agentColliderGameObjects[i] = colliders[i].gameObject;
                        }
                    }
                    _originalColliderLayer = new int[_agentColliderGameObjects.Length];
                }

                // Change the layer. Remember the previous layer so it can be reset after the check has completed.
                for (int i = 0; i < _agentColliderGameObjects.Length; ++i)
                {
                    _originalColliderLayer[i] = _agentColliderGameObjects[i].layer;
                    _agentColliderGameObjects[i].layer = _ignoreRaycastLayer;
                }
            }

            if (usePhysics2D)
            {
                if (targetObjects != null && targetObjects.Count > 0)
                { 
                    // If there are objects in the group list then search for the object within that list
                    GameObject objectFound = null;
                    float minAngle = Mathf.Infinity;
                    for (int i = 0; i < targetObjects.Count; ++i)
                    {
                        float angle;
                        GameObject obj;
                        if ((obj = LocomotionUtility.WithinSight(agent, positionOffset, fieldOfViewAngle,
                            viewDistance, targetObjects[i].gameObject, targetOffset, true, angleOffset2D,
                            out angle, ignoreLayerMask, useTargetBone, targetBone)) != null)
                        {
                            // This object is within sight. Set it to the objectFound GameObject if the angle is less than any of the other objects
                            if (angle < minAngle)
                            {
                                minAngle = angle;
                                objectFound = obj;
                            }
                        }
                    }
                    returnedObject = objectFound;
                }
                else if (targetObject != null)
                { 
                    // If the target is not null then determine if that object is within sight
                    returnedObject = LocomotionUtility.WithinSight2D(agent, positionOffset, fieldOfViewAngle, 
                        viewDistance, targetObject.gameObject, targetOffset, angleOffset2D, ignoreLayerMask,
                        useTargetBone, targetBone);
                }
                else if (!string.IsNullOrEmpty(targetTag))
                {
                    // If the target tag is not null then determine if there are any objects within sight based on the tag
                    returnedObject = LocomotionUtility.WithinSight2D(agent, positionOffset, fieldOfViewAngle,
                        viewDistance, GameObject.FindGameObjectWithTag(targetTag), targetOffset,
                        angleOffset2D, ignoreLayerMask, useTargetBone, targetBone);
                }
                else
                {
                    // If the target object is null and there is no tag then determine if there are any objects within sight based on the layer mask
                    if (_overlap2DColliders == null)
                    {
                        _overlap2DColliders = new Collider2D[maxCollisionCount];
                    }
                    returnedObject = LocomotionUtility.WithinSight2D(agent, positionOffset, fieldOfViewAngle,
                        viewDistance, _overlap2DColliders, objectLayerMask, targetOffset, angleOffset2D, ignoreLayerMask);
                }
            }
            else
            {
                if (targetObjects != null && targetObjects.Count > 0)
                {
                    // 指定对象列表是否在视野
                    // If there are objects in the group list then search for the object within that list
                    GameObject objectFound = null;
                    float minAngle = Mathf.Infinity;
                    for (int i = 0; i < targetObjects.Count; ++i)
                    {
                        float angle;
                        GameObject obj;
                        if ((obj = LocomotionUtility.WithinSight(agent, positionOffset, fieldOfViewAngle, viewDistance,
                            targetObjects[i].gameObject, targetOffset, false, angleOffset2D, out angle, ignoreLayerMask,
                            useTargetBone, targetBone)) != null)
                        {
                            // This object is within sight. Set it to the objectFound GameObject if the angle is less than any of the other objects
                            if (angle < minAngle)
                            {
                                minAngle = angle;
                                objectFound = obj;
                            }
                        }
                    }
                    returnedObject = objectFound;
                }
                else if (targetObject != null)
                {
                    // 指定对象是否在视野
                    // If the target is not null then determine if that object is within sight
                    returnedObject = LocomotionUtility.WithinSight(agent, positionOffset, fieldOfViewAngle, 
                        viewDistance, targetObject.gameObject, targetOffset, ignoreLayerMask, useTargetBone, targetBone);
                }
                else if (!string.IsNullOrEmpty(targetTag))
                {
                    // 指定tag的对象是否在视野
                    // If the target tag is not null then determine if there are any objects within sight based on the tag
                    returnedObject = LocomotionUtility.WithinSight(agent, positionOffset, fieldOfViewAngle,
                        viewDistance, GameObject.FindGameObjectWithTag(targetTag), targetOffset, ignoreLayerMask,
                        useTargetBone, targetBone);
                }
                else
                {
                    // 通过碰撞检测是否有target在视野
                    // If the target object is null and there is no tag then determine if there are any objects within sight based on the layer mask
                    if (_overlapColliders == null)
                    {
                        _overlapColliders = new Collider[maxCollisionCount];
                    }
                    returnedObject = LocomotionUtility.WithinSight(agent, positionOffset, fieldOfViewAngle,
                        viewDistance, _overlapColliders, objectLayerMask, targetOffset, ignoreLayerMask, useTargetBone, targetBone);
                }
            }

            if (disableAgentColliderLayer)
            {
                for (int i = 0; i < _agentColliderGameObjects.Length; ++i)
                {
                    _agentColliderGameObjects[i].layer = _originalColliderLayer[i];
                }
            }

            if (returnedObject != null)
            {
                // Return success if an object was found
                return LocomotionStatus.SUCCESS;
            }
            // An object is not within sight so return failure
            return LocomotionStatus.FAILURE;
        }

        // Reset the public variables
        public override void OnReset()
        {
            
        }

        public override void OnDrawGizmos()
        {
            LocomotionUtility.DrawViewOfSight(agent, positionOffset, fieldOfViewAngle, angleOffset2D, viewDistance, usePhysics2D);
        }

        public override void OnDrawGizmosSelected()
        {
            LocomotionUtility.DrawViewOfSight(agent, positionOffset, fieldOfViewAngle, angleOffset2D, viewDistance, usePhysics2D);
        }

        public override void OnEnd()
        {
            TransformTools.Instance.ClearComponentCaches();
        }
    }
}