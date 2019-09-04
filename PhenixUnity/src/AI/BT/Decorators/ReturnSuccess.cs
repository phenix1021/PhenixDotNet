namespace Phenix.Unity.AI
{
    [TaskIcon("TaskIcons/ReturnSuccess.png")]
    public class ReturnSuccess : Decorator
    {
        public override TaskStatus Decorate(TaskStatus status)
        {
            if (status == TaskStatus.Running)
            {
                return TaskStatus.Running;
            }
            return TaskStatus.Success;
        }
    }
}