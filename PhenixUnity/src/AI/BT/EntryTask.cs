using UnityEngine;
using System;
using System.Collections;

namespace Phenix.Unity.AI
{
    [TaskIcon("TaskIcons/Entry.png")]
    [TaskDescription("Entry")]
    public sealed class EntryTask : ParentTask
    {
        //public 自定义class taskParams;

        protected override int MaxChildrenCount()
        {
            return 1;
        }

        public override TaskStatus Run()
        {
            if (Children.Count <= 0)
            {
                return TaskStatus.Ignored;
            }
            
            return Children[0].OnUpdate();            
        }        
    }
}