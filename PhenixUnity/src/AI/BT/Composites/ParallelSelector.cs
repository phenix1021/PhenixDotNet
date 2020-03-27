using System;
using System.Collections.Generic;

namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/ParallelSelector.png")]
    [TaskDescription("ParallelSelector")]
    public class ParallelSelector : ParentTask
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
            List<int> succeed = new List<int>();
            int ignoredCnt = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                status = Children[i].OnUpdate();
                if (status == TaskStatus.SUCCESS)
                {
                    succeed.Add(i);
                }                
                else if (status == TaskStatus.RUNNING)
                {
                    running.Add(i);
                }
                else if (status == TaskStatus.IGNORED)
                {
                    ++ignoredCnt;
                }
            }

            if (succeed.Count > 0)
            {
                foreach (var item in running)
                {
                    Children[item].ForceEnd();
                }
                return TaskStatus.SUCCESS;
            }

            if (running.Count > 0)
            {
                return TaskStatus.RUNNING;
            }

            if (ignoredCnt == Children.Count)
            {
                return TaskStatus.IGNORED;
            }
            
            return TaskStatus.FAILURE;
        }
    }
}