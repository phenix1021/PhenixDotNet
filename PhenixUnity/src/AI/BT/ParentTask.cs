using System.Collections.Generic;

namespace Phenix.Unity.AI.BT
{    
    public abstract class ParentTask : Task
    {
        //[SerializeField]
        List<Task> _children = new List<Task>();

        int _firstRunningChildIdx = -1;
        protected int FirstRunningChildIdx { get { return _firstRunningChildIdx; } set { _firstRunningChildIdx = value; } }

        public List<Task> Children { get { return _children; } }        

        protected virtual int MaxChildrenCount() { return int.MaxValue; }        

        public Task AddChild(Task task)
        {
            if (_children.Count >= MaxChildrenCount())
            {
                return null;
            }
            if (task != null)
            {
                _children.Add(task);
                Sort();                
                task.Parent = this;
            }            
            return task;
        }

        public void Sort()
        {
            _children.Sort((Task one, Task other) => { return one.nodeData.pos.x < other.nodeData.pos.x ? -1 : 1; });
        }

        public void RemoveChild(Task task)
        {
            _children.Remove(task);
            task.Parent = null;
        }

        public override void ForceTurnEnd()
        {
            if (Status == TaskStatus.RUNNING)
            {
                foreach (var child in Children)
                {
                    child.ForceTurnEnd();
                }
                OnEnd();
            }
        }

        public override float GetPriority()
        {
            float ret = 0;
            foreach (var child in Children)
            {
                ret += child.GetPriority();
            }
            return ret;
        }

        public override void OnAwake()
        {
            foreach (var child in Children)
            {
                child.OnAwake();
            }
        }

        /*public override void OnStart()
        {
            foreach (var child in Children)
            {
                child.OnStart();
            }
        }*/
    }
}