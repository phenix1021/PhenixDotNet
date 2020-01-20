using System;

namespace Phenix.Unity.AI.BT
{    
    public abstract class Decorator : ParentTask
    {
        protected override int MaxChildrenCount()
        {
            return 1;
        }

        public sealed override TaskStatus Run()
        {
            if (Children.Count <= 0)
            {
                return TaskStatus.IGNORED;
            }
            return Decorate(Children[0].OnUpdate());
        }

        public abstract TaskStatus Decorate(TaskStatus status);        
    }
}