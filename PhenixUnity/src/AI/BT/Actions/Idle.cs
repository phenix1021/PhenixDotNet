using UnityEngine;

namespace Phenix.Unity.AI
{
    [TaskIcon("TaskIcons/Idle.png")]
    public class Idle : Phenix.Unity.AI.Action<IdleImpl> { }

    [System.Serializable]
    public class IdleImpl : Phenix.Unity.AI.ActionImpl
    {
        /*public float time = 0;

        float _startTimer = 0;

        public override void OnTurnBegin()
        {
            base.OnTurnBegin();
            _startTimer = Time.timeSinceLevelLoad;
        }

        public override void OnTurnEnd()
        {
            base.OnTurnEnd();
        }*/

        public override TaskStatus Run()
        {
            /*if (time > 0 && _startTimer + time <= Time.timeSinceLevelLoad)
            {
                return TaskStatus.Success;
            }*/
            return TaskStatus.Running;
        }
    }
}