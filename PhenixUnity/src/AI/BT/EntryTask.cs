namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Entry.png")]
    [TaskDescription("Entry")]
    public sealed class EntryTask : ParentTask
    {
        //public 自定义class taskParams;

        protected override int MaxChildrenCount()
        {
            return 1;
        }

        public override TaskStatus Run()
        {
            if (Children.Count <= 0)
            {
                return TaskStatus.IGNORED;
            }
            
            return Children[0].OnUpdate();            
        }        
    }
}