using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Phenix.Unity.AI.Locomotion
{    
    /// <summary>
    /// ��Seek��������Pursue����У��·�������ҿ���ѡ��Ԥ��Ŀ��ǰ����λ
    /// </summary>
    public class Pursue : NavMeshLocomotion
    {
        // �Ƿ�Ԥ��Ŀ���н�λ��
        public bool usePrediction;
        // How far to predict the distance ahead of the target. Lower values indicate less distance should be predicated
        public float targetDistPrediction = 20;
        // Multiplier for predicting the look ahead distance
        public float targetDistPredictionMult = 20;

        // The GameObject that the agent is pursuing
        public Transform target;  // ׷��Ŀ��        

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
            ����ԭ�����첽Ѱ·д�����޷�ȷ��onMove�ĵ���ʱ�����ᵼ�´������������ƶ���
            ����������update�н��� if (pathPending) then onIdle�����˶������л᲻��
            �滮��·���ģ�pathPendingʱ��������������·���ƶ������Ǿͻ���ɴ���������
            �ƶ�����ֱ��֡�
            *************************************/
            NavMeshPath path = new NavMeshPath(); // ͬ�������ʼ·��
            if (navMeshAgent.CalculatePath(Target(), path))
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetPath(path);             
            }

            if (onMove != null)
            {
                onMove.Invoke(); // ����onMove�ص�
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
                return target.position;  // ��Ԥ�⣬ʹ����ҵ�ǰλ��
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
            Vector3 preTargetPosition = _targetPosition; // Ŀ��ԭ��λ��
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