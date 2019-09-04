

namespace Phenix.Unity.AI
{    
    [TaskIcon("TaskIcons/Selector.png")]
    [TaskDescription("Selector")]
    public class Selector : ParentTask
    {
        //public 自定义class taskParams;

        public override TaskStatus Run()
        {
            if (Children.Count <= 0)
            {
                return TaskStatus.Ignored;
            }

            TaskStatus status = TaskStatus.None;
            int startIdx = System.Math.Max(0, FirstRunningChildIdx);
            int ignoredCnt = 0;
            for (int i = startIdx; i < Children.Count; i++)
            {
                status = Children[i].OnUpdate();
                if (status == TaskStatus.Success)
                {
                    break;
                }
                if (status == TaskStatus.Running)
                {
                    FirstRunningChildIdx = i;
                    break;
                }
                if (status == TaskStatus.Ignored)
                {
                    ++ignoredCnt;
                }
            }

            if (ignoredCnt == Children.Count - startIdx)
            {
                return TaskStatus.Ignored;
            }

            return status;
        }

        protected override void OnTurnEnd()
        {
            FirstRunningChildIdx = -1;
        }
        
    }
}