using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Repeater.png")]
    public class Repeater : Decorator
    {
        [Tooltip("less or equal 0 means forever")]
        public int count = 1;        
        public bool endOnFailure = false;

        int executionCnt = 0;

        public override TaskStatus Decorate(TaskStatus status)
        {
            if (status == TaskStatus.SUCCESS || status == TaskStatus.FAILURE)
            {
                ++executionCnt;
            }

            if (status == TaskStatus.FAILURE)
            {
                if (endOnFailure)
                {
                    return TaskStatus.FAILURE;
                }                
            }

            if (count > 0)
            {
                if (count <= executionCnt)
                {
                    return TaskStatus.IGNORED;
                }                
            }
            
            return TaskStatus.RUNNING;
        }
    }
}