using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Repeater.png")]
    public class Repeater : Decorator
    {        
        public float time = 5;     // 重复时长（秒）
        float _expireTimer = 0;

        public TaskStatus returnStatusOnExpire = TaskStatus.SUCCESS;

        protected override void OnStart()
        {
            base.OnStart();
            _expireTimer = Time.realtimeSinceStartup + time;
        }

        public override TaskStatus Decorate(TaskStatus status)
        {
            if (status == TaskStatus.SUCCESS || status == TaskStatus.FAILURE)
            {
                return status;
            }
            else if (status == TaskStatus.RUNNING && 
                Time.realtimeSinceStartup >= _expireTimer)
            {
                return returnStatusOnExpire;
            }
            else
            {
                return TaskStatus.RUNNING;
            }            
        }
    }
}