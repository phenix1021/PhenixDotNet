namespace Phenix.Unity.AI
{
    [TaskIcon("TaskIcons/ReturnFailure.png")]
    public class ReturnFailure : Decorator
    {
        public override TaskStatus Decorate(TaskStatus status)
        {
            if (status == TaskStatus.Running)
            {
                return TaskStatus.Running;
            }
            return TaskStatus.Failure;
        }
    }
}