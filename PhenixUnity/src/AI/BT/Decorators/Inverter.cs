namespace Phenix.Unity.AI
{
    [TaskIcon("TaskIcons/Inverter.png")]
    public class Inverter : Decorator
    {
        public override TaskStatus Decorate(TaskStatus status)
        {
            if (status == TaskStatus.Success)
            {
                return TaskStatus.Failure;
            }
            else if (status == TaskStatus.Failure)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
}