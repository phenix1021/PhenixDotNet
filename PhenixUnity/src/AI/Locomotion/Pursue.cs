using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Phenix.Unity.AI.Locomotion
{    
    /// <summary>
    /// 和Seek区别在于Pursue不断校正路径，并且可以选择预测目标前进方位
    /// </summary>
    public class Pursue : NavMeshLocomotion
    {
        // 是否预测目标行进位置
        public bool usePrediction;
        // How far to predict the distance ahead of the target. Lower values indicate less distance should be predicated
        public float targetDistPrediction = 20;
        // Multiplier for predicting the look ahead distance
        public float targetDistPredictionMult = 20;

        // The GameObject that the agent is pursuing
        public Transform target;  // 追逐目标        

        public UnityAction onMove;
        public UnityAction onMoving;
        public UnityAction onArrived;        

        // The position of the target at the last frame
        Vector3 _targetPosition;

        public override void OnStart()
        {
            base.OnStart();

            _targetPosition = target.position;
            /************************************
            SetDestination(Target()); 
            这是原来的异步寻路写法。无法确定onMove的调用时机，会导致待机动作进行移动。
            曾经尝试在update中进行 if (pathPending) then onIdle。但运动过程中会不断
            规划新路径的，pathPending时往往依旧沿着老路径移动。这是就会造成待机动作在
            移动的奇怪表现。
            *************************************/
            NavMeshPath path = new NavMeshPath(); // 同步计算初始路径
            if (navMeshAgent.CalculatePath(Target(), path))
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetPath(path);             
            }

            if (onMove != null)
            {
                onMove.Invoke(); // 触发onMove回调
            }
        }

        // Pursue the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override LocomotionStatus OnUpdate()
        {
            if (HasArrived())
            {
                if (onArrived != null)
                {
                    onArrived.Invoke();
                }

                return LocomotionStatus.SUCCESS;
            }

            // Target will return the predicated position
            SetDestination(Target());

            UpdateNavMeshObstacle();

            if (onMoving != null)
            {
                onMoving.Invoke();
            }

            return LocomotionStatus.RUNNING;
        }

        // Predict the position of the target
        private Vector3 Target()
        {
            if (usePrediction == false)
            {
                return target.position;  // 不预测，使用玩家当前位置
            }

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
            Vector3 preTargetPosition = _targetPosition; // 目标原来位置
            _targetPosition = target.position;
            return _targetPosition + (_targetPosition - preTargetPosition) * futurePrediction;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();
            target = null;
        }
    }
}