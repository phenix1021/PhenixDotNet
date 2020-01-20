using UnityEngine;
using System.Collections.Generic;

namespace Phenix.Unity.AI.Locomotion
{
    /// <summary>
    /// Check to see if the any object specified by the object list or tag is within the distance specified of the current agent.    
    /// </summary>
    public class WithinDistance : Locomotion//NavMeshLocomotion
    {
        // Should the 2D version be used?
        public bool usePhysics2D;

        // The object that we are searching for
        public GameObject targetObject;
        
        // The tag of the object that we are searching for
        public string targetTag;

        // The LayerMask of the objects that we are searching for
        public LayerMask objectLayerMask;

        // The distance that the object needs to be within
        public float magnitude = 5;

        /* If true, the object must be within line of sight to be within distance. For example, if this option is enabled then
         an object behind a wall will not be within distance even though it may be physically close to the other object*/
        public bool lineOfSight;

        // The LayerMask of the objects to ignore when performing the line of sight check
        public LayerMask ignoreLayerMask;// = 1 << LayerMask.NameToLayer("Ignore Raycast");

        // The raycast offset relative to the pivot position
        public Vector3 offset;

        // The target raycast offset relative to the pivot position
        public Vector3 targetOffset;

        // The object variable that will be set when a object is found what the object is
        public GameObject returnedObject;

        List<GameObject> _objects;

        // distance * distance, optimization so we don't have to take the square root
        float _sqrMagnitude;
        bool _overlapCast = false;

        public override void OnStart()
        {
            _sqrMagnitude = magnitude * magnitude;

            if (_objects != null)
            {
                _objects.Clear();
            }
            else
            {
                _objects = new List<GameObject>();
            }

            // if objects is null then find all of the objects using the layer mask or tag
            if (targetObject == null)
            {
                if (!string.IsNullOrEmpty(targetTag))
                {
                    var gameObjects = GameObject.FindGameObjectsWithTag(targetTag);
                    for (int i = 0; i < gameObjects.Length; ++i)
                    {
                        _objects.Add(gameObjects[i]);
                    }
                }
                else
                {
                    _overlapCast = true;
                }
            }
            else
            {
                _objects.Add(targetObject);
            }
        }

        public override void OnEnd()
        {
            
        }

        // returns success if any object is within distance of the current object. Otherwise it will return failure
        public override LocomotionStatus OnUpdate()
        {
            if (agent == null || _objects == null)
                return LocomotionStatus.FAILURE;

            if (_overlapCast)
            {
                _objects.Clear();
                if (usePhysics2D)
                {
                    var colliders = Physics.OverlapSphere(agent.position, magnitude, objectLayerMask.value);
                    for (int i = 0; i < colliders.Length; ++i)
                    {
                        _objects.Add(colliders[i].gameObject);
                    }
                }
                else
                {
                    var colliders = Physics2D.OverlapCircleAll(agent.position, magnitude, objectLayerMask.value);
                    for (int i = 0; i < colliders.Length; ++i)
                    {
                        _objects.Add(colliders[i].gameObject);
                    }
                }
            }

            Vector3 direction;
            // check each object. All it takes is one object to be able to return success
            for (int i = 0; i < _objects.Count; ++i)
            {
                if (_objects[i] == null)
                {
                    continue;
                }
                direction = _objects[i].transform.position - (agent.position + offset);
                // check to see if the square magnitude is less than what is specified
                if (Vector3.SqrMagnitude(direction) < _sqrMagnitude)
                {
                    // the magnitude is less. If lineOfSight is true do one more check
                    if (lineOfSight)
                    {
                        if (LocomotionUtility.LineOfSight(agent, offset, _objects[i], targetOffset, usePhysics2D, ignoreLayerMask.value))
                        {
                            // the object has a magnitude less than the specified magnitude and is within sight. Set the object and return success
                            returnedObject = _objects[i];
                            return LocomotionStatus.SUCCESS;
                        }
                    }
                    else
                    {
                        // the object has a magnitude less than the specified magnitude. Set the object and return success
                        returnedObject = _objects[i];
                        return LocomotionStatus.SUCCESS;
                    }
                }
            }
            // no objects are within distance. Return failure
            return LocomotionStatus.FAILURE;
        }

        public override void OnReset()
        {
            usePhysics2D = false;
            targetObject = null;
            targetTag = string.Empty;
            objectLayerMask = 0;
            magnitude = 5;
            lineOfSight = true;
            ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
            offset = Vector3.zero;
            targetOffset = Vector3.zero;
        }
/*
        // Draw the seeing radius
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (Owner == null || magnitude == null) {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(Owner.transform.position, usePhysics2D ? Owner.transform.forward : Owner.transform.up, magnitude.Value);
            UnityEditor.Handles.color = oldColor;
#endif
        }*/
    }
}