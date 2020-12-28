using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [System.Serializable]
    public class RepeaterParams
    {
        public float time = 5;     // 重复时长（秒）
        public TaskStatus returnStatusOnExpire = TaskStatus.SUCCESS;
    }

    [TaskIcon("TaskIcons/Repeater.png")]
    public class Repeater : Decorator
    {
        public RepeaterParams taskParams = new RepeaterParams();
        float _expireTimer = 0;        

        protected override void OnStart()
        {
            base.OnStart();
            _expireTimer = Time.timeSinceLevelLoad + taskParams.time;
        }

        public override TaskStatus Decorate(TaskStatus status)
        {
            if (status == TaskStatus.SUCCESS || status == TaskStatus.FAILURE)
            {
                return status;
            }
            else if (status == TaskStatus.RUNNING && 
                Time.timeSinceLevelLoad >= _expireTimer)
            {
                Children[0].ForceEnd();
                return taskParams.returnStatusOnExpire;
            }
            else
            {
                return TaskStatus.RUNNING;
            }            
        }
    }
}