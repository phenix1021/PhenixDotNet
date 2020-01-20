namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Sequence.png")]
    [TaskDescription("Sequence")]
    public class Sequence : ParentTask
    {
        //public 自定义class taskParams;

        public override TaskStatus Run()
        {            
            if (Children.Count <= 0)
            {
                return TaskStatus.IGNORED;
            }

            TaskStatus status = TaskStatus.NONE;
            int startIdx = System.Math.Max(0, FirstRunningChildIdx);
            int ignoredCnt = 0;
            for (int i = startIdx; i < Children.Count; i++)
            {
                status = Children[i].OnUpdate();
                if (status == TaskStatus.FAILURE)
                {
                    break;
                }
                if (status == TaskStatus.RUNNING)
                {
                    FirstRunningChildIdx = i;
                    break;
                }
                if (status == TaskStatus.IGNORED)
                {
                    ++ignoredCnt;
                }
            }

            if (ignoredCnt == Children.Count - startIdx)
            {
                return TaskStatus.IGNORED;
            }

            return status;           
        }
        
        protected override void OnEnd()
        {
            FirstRunningChildIdx = -1;
        }
    }
}