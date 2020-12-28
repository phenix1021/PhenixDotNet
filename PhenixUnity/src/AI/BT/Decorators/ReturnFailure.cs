namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/ReturnFailure.png")]
    public class ReturnFailure : Decorator
    {
        public override TaskStatus Decorate(TaskStatus status)
        {
            //if (status == TaskStatus.RUNNING)
            //{
            //    return TaskStatus.RUNNING;
            //}
            return TaskStatus.FAILURE;
        }
    }
}