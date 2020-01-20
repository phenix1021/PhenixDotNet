namespace Phenix.Unity.AI.BT
{
    [TaskIcon("TaskIcons/Inverter.png")]
    public class Inverter : Decorator
    {
        public override TaskStatus Decorate(TaskStatus status)
        {
            if (status == TaskStatus.SUCCESS)
            {
                return TaskStatus.FAILURE;
            }
            else if (status == TaskStatus.FAILURE)
            {
                return TaskStatus.SUCCESS;
            }
            return TaskStatus.RUNNING;
        }
    }
}