using System;
using System.Collections.Generic;

namespace Phenix.Unity.AI
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
                return TaskStatus.Ignored;
            }

            TaskStatus status = TaskStatus.None;
            List<int> running = new List<int>();
            List<int> fail = new List<int>();
            int ignoredCnt = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Status == TaskStatus.Success)
                {
                    continue;
                }
                status = Children[i].OnUpdate();
                if (status == TaskStatus.Running)
                {
                    running.Add(i);
                }
                if (status == TaskStatus.Failure)
                {
                    fail.Add(i);
                }
                if (status == TaskStatus.Ignored)
                {
                    ++ignoredCnt;
                }
            }

            if (fail.Count > 0)
            {
                foreach (var item in running)
                {
                    Children[item].ForceTurnEnd();
                }
                return TaskStatus.Failure;                
            }

            if (running.Count > 0)
            {
                return TaskStatus.Running;                
            }

            if (ignoredCnt == Children.Count)
            {
                return TaskStatus.Ignored;
            }                

            return TaskStatus.Success;
        }
    }
}
