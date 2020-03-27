namespace Phenix.Unity.AI.BT
{
    //[TaskIcon("TaskIcons/IfElse.png")]
    [TaskDescription("IfElse")]
    public class IfElse : ParentTask
    {
        //public 自定义class taskParams;
        Task _ifAction = null;
        Task _thenAction = null;
        Task _elseAction = null;        

        public override TaskStatus Run()
        {            
            if (Status == TaskStatus.ERROR)
            {
                return Status;
            }

            TaskStatus status = TaskStatus.NONE;
            if (_ifAction.Status == TaskStatus.NONE ||
                _ifAction.Status == TaskStatus.RUNNING)
            {
                status = _ifAction.OnUpdate();
                if (status == TaskStatus.SUCCESS)
                {
                    status = _thenAction.OnUpdate();
                }
                else if (status == TaskStatus.FAILURE && _elseAction)
                {
                    status = _elseAction.OnUpdate();
                }
            }
            else if (_ifAction.Status == TaskStatus.SUCCESS)
            {
                status = _thenAction.OnUpdate();
            }
            else if (_ifAction.Status == TaskStatus.FAILURE && _elseAction)
            {
                status = _elseAction.OnUpdate();
            }

            return status;           
        }
        
        public override void OnAwake()
        {
            base.OnAwake();
            if (Children.Count == 2)
            {
                _ifAction = Children[0];
                _thenAction = Children[1];                
            }
            else if (Children.Count == 3)
            {
                _ifAction = Children[0];
                _thenAction = Children[1];
                _elseAction = Children[2];                
            }
            else
            {
                Status = TaskStatus.ERROR;
            }            
        }
    }
}