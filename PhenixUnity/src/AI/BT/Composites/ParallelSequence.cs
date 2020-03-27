using System;
using System.Collections.Generic;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/ParallelSequence.png")]
    [TaskDescription("ParallelSequence")]
    public class ParallelSequence : ParentTask
    {
        //public 自定义class taskParams;

        public override TaskStatus Run()
        {
            if (Children.Count <= 0)
            {
                return TaskStatus.IGNORED;
            }

            TaskStatus status = TaskStatus.NONE;
            List<int> running = new List<int>();
            List<int> fail = new List<int>();
            int ignoredCnt = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Status == TaskStatus.SUCCESS)
                {
                    continue;
                }
                status = Children[i].OnUpdate();
                if (status == TaskStatus.RUNNING)
                {
                    running.Add(i);
                }
                if (status == TaskStatus.FAILURE)
                {
                    fail.Add(i);
                }
                if (status == TaskStatus.IGNORED)
                {
                    ++ignoredCnt;
                }
            }

            if (fail.Count > 0)
            {
                foreach (var item in running)
                {
                    Children[item].ForceEnd();
                }
                return TaskStatus.FAILURE;                
            }

            if (running.Count > 0)
            {
                return TaskStatus.RUNNING;                
            }

            if (ignoredCnt == Children.Count)
            {
                return TaskStatus.IGNORED;
            }                

            return TaskStatus.SUCCESS;
        }
    }
}
