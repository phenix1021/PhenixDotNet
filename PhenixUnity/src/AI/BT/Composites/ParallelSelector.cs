using System;
using System.Collections.Generic;

namespace Phenix.Unity.AI
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
                return TaskStatus.Ignored;
            }

            TaskStatus status = TaskStatus.None;
            List<int> running = new List<int>();
            List<int> succeed = new List<int>();
            int ignoredCnt = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                status = Children[i].OnUpdate();
                if (status == TaskStatus.Success)
                {
                    succeed.Add(i);
                }                
                else if (status == TaskStatus.Running)
                {
                    running.Add(i);
                }
                else if (status == TaskStatus.Ignored)
                {
                    ++ignoredCnt;
                }
            }

            if (succeed.Count > 0)
            {
                foreach (var item in running)
                {
                    Children[item].ForceTurnEnd();
                }
                return TaskStatus.Success;
            }

            if (running.Count > 0)
            {
                return TaskStatus.Running;
            }

            if (ignoredCnt == Children.Count)
            {
                return TaskStatus.Ignored;
            }
            
            return TaskStatus.Failure;
        }
    }
}