namespace Phenix.Unity.AI
{
    [TaskIcon("TaskIcons/UntilSuccess.png")]
    public class UntilSuccess : Decorator
    {
        public override TaskStatus Decorate(TaskStatus status)
        {
            if (status == TaskStatus.Success)
            {
                return status;
            }
            return TaskStatus.Running;
        }
    }
}