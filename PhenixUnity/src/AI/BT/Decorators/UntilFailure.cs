namespace Phenix.Unity.AI
{
    [TaskIcon("TaskIcons/UntilFailure.png")]
    public class UntilFailure : Decorator
    {
        public override TaskStatus Decorate(TaskStatus status)
        {
            if (status == TaskStatus.Failure)
            {
                return status;
            }
            return TaskStatus.Running;
        }
    }
}