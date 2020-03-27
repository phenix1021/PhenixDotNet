namespace Phenix.Unity.AI.BT
{
    //[TaskIcon("TaskIcons/While.png")]
    [TaskDescription("While")]
    public class While : ParentTask
    {
        //public 自定义class taskParams;       
        Task _whileAction;
        Task _doAction;

        public override TaskStatus Run()
        {
            if (Status == TaskStatus.ERROR)
            {
                return Status;
            }

            TaskStatus status = TaskStatus.NONE;            
            for (int i = 0; i < Children.Count; i++)
            {
                status = Children[i].OnUpdate();
                if (status != TaskStatus.SUCCESS)
                {
                    break;
                }                
            }

            if (status != TaskStatus.RUNNING)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].ForceEnd();
                }
            }

            return status;           
        }

        public override void OnAwake()
        {
            base.OnAwake();
            if (Children.Count != 2)
            {
                Status = TaskStatus.ERROR;
                return;
            }

            _whileAction = Children[0];
            _doAction = Children[1];
        }
    }
}