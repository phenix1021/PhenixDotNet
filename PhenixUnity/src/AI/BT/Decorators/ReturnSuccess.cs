namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/ReturnSuccess.png")]
    public class ReturnSuccess : Decorator
    {
        public override TaskStatus Decorate(TaskStatus status)
        {
            //if (status == TaskStatus.RUNNING)
            //{
            //    return TaskStatus.RUNNING;
            //}
            return TaskStatus.SUCCESS;
        }
    }
}