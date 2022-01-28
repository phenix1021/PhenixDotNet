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

        public bool fixedCenter;    // 是否固定wander中心点位置，即wander范围限于agent初始位置为中心，wanderDistance为半径的圆内

        float _pauseTime;
        float _destinationReachTime;
        
        bool _moveTriggered = false; // 保证onRest、onMove不重复触发

        Vector3 _oriPos;

        public UnityAction onRest;
        public UnityAction onMove;

        public Wander(Vector3 oriPos)
        {
            _oriPos = oriPos;
        }

        public override void OnStart()
        {
            base.OnStart();
            Stop();
            //_oriPos = agent.position;
        }

        // There is no success or fail state with wander - the agent will just keep wandering
        public override LocomotionStatus OnUpdate()
        {
            if (HasArrived() || navMeshAgent.isStopped)
            {
                _moveTriggered = false;
                // The agent should pause at the destination only if the max pause duration is greater than 0
                if (maxPauseDuration > 0) // 设置了待机时间
                {
                    if (_destinationReachTime == -1)
                    {
                        _destinationReachTime = Time.time; // 设置待机开始时刻
                        _pauseTime = Random.Range(minPauseDuration, maxPauseDuration); // 设置待机时长
                        if (onRest != null)
                        {
                            onRest.Invoke();
                        }
                    }

                    if (_destinationReachTime + _pauseTime <= Time.time) // 待机超时
                    {
                        SetTarget();
                    }
                }
                else // 没设置待机时间，继续前进
                {
                    SetTarget();                   
                }
            }
            else if (navMeshAgent.pathPending) // 寻路中（运动中途调用SetDestination规划新路径，这时pathPending但在完成前依旧沿原路线行进）
            {
                if (_moveTriggered)
                {
                    _moveTriggered = false;
                    if (onRest != null)
                    {
                        onRest.Invoke();
                    }
                }
            }
            else // 行进中
            {
                if (_moveTriggered == false)
                {
                    _moveTriggered = true;
                    if (onMove != null)
                    {
                        onMove.Invoke();
                    }
                }
            }

            UpdateNavMeshObstacle();
            return LocomotionStatus.RUNNING;
        }

        void SetTarget()
        {
            if (TrySetTarget())
            {
                _destinationReachTime = -1;                
            }
            else if (_moveTriggered)
            {
                _moveTriggered = false;
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
                if (fixedCenter)
                {
                    destination = _oriPos + Random.insideUnitSphere * Random.Range(minWanderDistance, maxWanderDistance);
                }
                else
                {
                    direction = direction + Random.insideUnitSphere * wanderRate;
                    destination = agent.position + direction.normalized * Random.Range(minWanderDistance, maxWanderDistance);
                }
                
                validDestination = SamplePosition(ref destination);
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