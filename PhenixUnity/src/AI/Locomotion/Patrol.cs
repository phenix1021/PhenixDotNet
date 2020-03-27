using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Phenix.Unity.AI.Locomotion
{
    // Patrol around the specified waypoints using the Unity NavMesh.
    public class Patrol : NavMeshLocomotion
    {
        // Should the agent patrol the waypoints randomly?
        public bool randomPatrol = false;

        // The length of time that the agent should pause when arriving at a waypoint
        public float waypointPauseDuration = 0;

        // The waypoints to move to
        public List<GameObject> waypoints = new List<GameObject>();

        // The current index that we are heading towards within the waypoints array
        int _waypointIndex;
        float _waypointReachedTime;

        public UnityAction onRest;
        public UnityAction onMove;

        public override void OnStart()
        {
            base.OnStart();

            // initially move towards the closest waypoint
            float distance = Mathf.Infinity;
            float localDistance;
            for (int i = 0; i < waypoints.Count; ++i)
            {
                if ((localDistance = Vector3.Magnitude(agent.position - waypoints[i].transform.position)) < distance)
                {
                    distance = localDistance;
                    _waypointIndex = i;
                }
            }
            _waypointReachedTime = -1;
            SetDestination(Target());
            if (onMove != null)
            {
                onMove.Invoke();
            }
        }

        // Patrol around the different waypoints specified in the waypoint array. Always return a task status of running. 
        public override LocomotionStatus OnUpdate()
        {
            if (waypoints.Count == 0)
            {
                return LocomotionStatus.FAILURE;
            }
            if (HasArrived())
            {
                if (_waypointReachedTime == -1)
                {
                    _waypointReachedTime = Time.time;
                    if (onRest != null)
                    {
                        onRest.Invoke();
                    }
                }
                // wait the required duration before switching waypoints.
                if (_waypointReachedTime + waypointPauseDuration <= Time.time)
                {
                    if (randomPatrol)
                    {
                        if (waypoints.Count == 1)
                        {
                            _waypointIndex = 0;
                        }
                        else
                        {
                            // prevent the same waypoint from being selected
                            var newWaypointIndex = _waypointIndex;
                            while (newWaypointIndex == _waypointIndex)
                            {
                                newWaypointIndex = Random.Range(0, waypoints.Count);
                            }
                            _waypointIndex = newWaypointIndex;
                        }
                    }
                    else
                    {
                        _waypointIndex = (_waypointIndex + 1) % waypoints.Count;
                    }

                    SetDestination(Target());
                    if (onMove != null)
                    {
                        onMove.Invoke();
                    }
                    _waypointReachedTime = -1;
                }
            }
            UpdateNavMeshObstacle();
            return LocomotionStatus.RUNNING;
        }

        // Return the current waypoint index position
        private Vector3 Target()
        {
            if (_waypointIndex >= waypoints.Count)
            {
                return agent.position;
            }
            return waypoints[_waypointIndex].transform.position;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();
        }
        /*
        // Draw a gizmo indicating a patrol 
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (waypoints == null || waypoints == null) {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            for (int i = 0; i < waypoints.Value.Count; ++i) {
                if (waypoints.Value[i] != null) {
                    UnityEditor.Handles.SphereHandleCap(0, waypoints.Value[i].transform.position, waypoints.Value[i].transform.rotation, 1, EventType.Repaint);
                }
            }
            UnityEditor.Handles.color = oldColor;
#endif
        }*/
    }
}