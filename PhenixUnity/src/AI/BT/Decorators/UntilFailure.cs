namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/UntilFailure.png")]
    public class UntilFailure : Decorator
    {
        public override TaskStatus Decorate(TaskStatus status)
        {
            if (status == TaskStatus.FAILURE)
            {
                return status;
            }
            return TaskStatus.RUNNING;
        }
    }
}