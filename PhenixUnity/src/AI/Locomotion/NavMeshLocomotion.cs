using UnityEngine;
using UnityEngine.AI;

namespace Phenix.Unity.AI.Locomotion
{
    public abstract class NavMeshLocomotion : Locomotion
    {
        public NavMeshAgent navMeshAgent;

        public float speed = 10;        
        public float angularSpeed = 120;        

        /// <summary>
        /// The agent has arrived when the destination is less than the specified amount. 
        /// This distance should be greater than or equal to the NavMeshAgent StoppingDistance.
        /// </summary>
        public float arriveDistance = 0.5f;

        /// <summary>
        /// Should the NavMeshAgent be stopped when the task ends?
        /// </summary>
        public bool stopOnEnd = true;

        /// <summary>
        /// Should the NavMeshAgent rotation be updated when the task ends?
        /// </summary>
        public bool updateRotation = true;       
        
        bool _oriUpdateRotation;        

        /// <summary>
        /// Allow pathfinding to resume.
        /// </summary>
        public override void OnStart()
        {
            navMeshAgent.speed = speed;
            navMeshAgent.angularSpeed = angularSpeed;
            navMeshAgent.isStopped = false;
            _oriUpdateRotation = navMeshAgent.updateRotation;
            UpdateRotation(updateRotation);
        }

        /// <summary>
        /// Set a new pathfinding destination.
        /// </summary>
        /// <param name="destination">The destination to set.</param>
        /// <returns>True if the destination is valid.</returns>
        protected virtual bool SetDestination(Vector3 destination)
        {
            navMeshAgent.isStopped = false;
            return navMeshAgent.SetDestination(destination);
        }

        /// <summary>
        /// Specifies if the rotation should be updated.
        /// </summary>
        /// <param name="update">Should the rotation be updated?</param>
        protected virtual void UpdateRotation(bool update)
        {
            navMeshAgent.updateRotation = update;
            navMeshAgent.updateUpAxis = update;
        }

        /// <summary>
        /// Does the agent have a pathfinding path?
        /// </summary>
        /// <returns>True if the agent has a pathfinding path.</returns>
        protected virtual bool HasPath()
        {
            return navMeshAgent.hasPath && navMeshAgent.remainingDistance > arriveDistance;
        }

        /// <summary>
        /// Returns the velocity of the agent.
        /// </summary>
        /// <returns>The velocity of the agent.</returns>
        protected virtual Vector3 Velocity()
        {
            return navMeshAgent.velocity;
        }

        /// <summary>
        /// Returns true if the position is a valid pathfinding position.
        /// </summary>
        /// <param name="position">The position to sample.</param>
        /// <returns>True if the position is a valid pathfinding position.</returns>
        protected bool SamplePosition(Vector3 position)
        {
            NavMeshHit hit;
            return NavMesh.SamplePosition(position, out hit, navMeshAgent.height * 2, NavMesh.AllAreas);
        }

        /// <summary>
        /// Has the agent arrived at the destination?
        /// </summary>
        /// <returns>True if the agent has arrived at the destination.</returns>
        protected virtual bool HasArrived()
        {
            // The path hasn't been computed yet if the path is pending.
            float remainingDistance;
            if (navMeshAgent.pathPending)
            {
                remainingDistance = float.PositiveInfinity;
            }
            else
            {
                remainingDistance = navMeshAgent.remainingDistance;
            }

            return remainingDistance <= arriveDistance;
        }

        /// <summary>
        /// Stop pathfinding.
        /// </summary>
        protected virtual void Stop()
        {
            UpdateRotation(_oriUpdateRotation);
            if (navMeshAgent.hasPath)
            {
                navMeshAgent.isStopped = true;
            }
        }

        /// <summary>
        /// The task has ended. Stop moving.
        /// </summary>
        public override void OnEnd()
        {
            if (stopOnEnd)
            {
                Stop();
            }
            else
            {
                UpdateRotation(_oriUpdateRotation);
            }
        }

        /// <summary>
        /// Reset the values back to their defaults.
        /// </summary>
        public override void OnReset()
        {
            speed = 10;
            angularSpeed = 120;
            arriveDistance = 1;
            stopOnEnd = true;
        }
    }
}