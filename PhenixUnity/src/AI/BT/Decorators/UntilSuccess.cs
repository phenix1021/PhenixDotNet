namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/UntilSuccess.png")]
    public class UntilSuccess : Decorator
    {
        public override TaskStatus Decorate(TaskStatus status)
        {
            if (status == TaskStatus.SUCCESS)
            {
                return status;
            }
            return TaskStatus.RUNNING;
        }
    }
}