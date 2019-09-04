using UnityEngine;

namespace Phenix.Unity.AI
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
            if (status == TaskStatus.Success || status == TaskStatus.Failure)
            {
                ++executionCnt;
            }

            if (status == TaskStatus.Failure)
            {
                if (endOnFailure)
                {
                    return TaskStatus.Failure;
                }                
            }

            if (count > 0)
            {
                if (count <= executionCnt)
                {
                    return TaskStatus.Ignored;
                }                
            }
            
            return TaskStatus.Running;
        }
    }
}