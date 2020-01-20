﻿using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Pursue.png")]
    public class Pursue : Action<PursueImpl> { }

    [System.Serializable]
    public class PursueImpl : NavMeshLocomotionImpl
    {
        [SerializeField]
        protected float targetDistPrediction = 20;
        [SerializeField]
        protected float targetDistPredictionMult = 20;
        [SerializeField]
        protected SharedGameObject target;        

        Locomotion.Pursue _pursue = new Locomotion.Pursue();

        public override void OnStart()
        {
            base.OnStart();
            _pursue.agent = Transform;
            _pursue.navMeshAgent = navMeshAgent;
            _pursue.speed = speed;
            _pursue.angularSpeed = angularSpeed;
            _pursue.arriveDistance = arriveDistance;
            _pursue.stopOnEnd = stopOnEnd;
            _pursue.updateRotation = updateRotation;

            _pursue.targetDistPrediction = targetDistPrediction;
            _pursue.targetDistPredictionMult = targetDistPredictionMult;

            if (target.Value != null)
            {
                _pursue.target = target.Value.transform;
            }            

            _pursue.OnStart();
        }

        public override TaskStatus Run()
        {
            if (target.Value != null)
            {
                _pursue.target = target.Value.transform;
            }            
            switch (_pursue.OnUpdate())
            {
                case Locomotion.LocomotionStatus.RUNNING:
                    return TaskStatus.RUNNING;
                case Locomotion.LocomotionStatus.SUCCESS:
                    return TaskStatus.SUCCESS;
                case Locomotion.LocomotionStatus.FAILURE:
                    return TaskStatus.FAILURE;
                default:
                    return TaskStatus.NONE;
            }
        }
    }
}