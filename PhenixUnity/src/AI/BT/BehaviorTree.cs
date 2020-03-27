using UnityEngine;
using System;
using System.Collections.Generic;

namespace Phenix.Unity.AI.BT
{
    [Serializable]
    public class BehaviorTreeParams
    {
        public bool restartOnTurnCompleted = false;

        [HideInInspector]
        public List<string> shareVariableNames = new List<string>();
    }

    // todo: Filter子树，Action子树，各种装饰节点，随机selector，动态权重Selector
    [Serializable]
    public class BehaviorTree : ScriptableObject
    {            
        bool _isTurnCompleted = false;
        EntryTask _entry;
        List<Task> _tasks;

        public BehaviorTreeParams btParams;

        // 本地黑板
        Blackboard _blackboard;

        [HideInInspector]
        public Transform Transform { get; set; }
        
        public Blackboard Blackboard { get { return _blackboard; } }
        // 全局黑板
        public static Blackboard globalBlackboard = new Blackboard();

        public EntryTask Entry { get { return _entry; } set { _entry = value; } }
        public List<Task> Tasks { get { return _tasks; } }        

        public bool IsUnactive { get { return _isTurnCompleted && btParams.restartOnTurnCompleted == false; } }

        private void OnEnable()
        {
            _tasks = new List<Task>();
            _blackboard = new Blackboard();
            btParams = new BehaviorTreeParams();                     
        }

        private void OnDisable()
        {
            
        }

        private void OnDestroy()
        {
            foreach (var task in _tasks)
            {
                DestroyImmediate(task);
            }
        }

        public T CreateTask<T>() where T : Task
        {
            Task task = ScriptableObject.CreateInstance<T>();
            if (task == null)
            {
                Console.WriteLine("fail to create task.");
                return null;
            }
            task.BT = this;
            //task.GUID = System.Guid.NewGuid().ToString();
            _tasks.Add(task);
            return task as T;
        }

        public Task CreateTask(string taskClassName)
        {
            Type type = Type.GetType(taskClassName);
            if (type == null)
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {                    
                    type = asm.GetType(taskClassName);
                    if (type != null)
                    {
                        break;
                    }
                }
                if (type == null)
                {
                    Debug.LogError(string.Format("fail to get the type for '{0}'", taskClassName));
                    return null;
                }                
            }
            Task task = ScriptableObject.CreateInstance(type) as Task;
            if (task == null)
            {
                Debug.LogError(string.Format("fail to create task with classname '{0}'", taskClassName));                    
                return null;
            }
            task.BT = this;
            //task.GUID = System.Guid.NewGuid().ToString();
            _tasks.Add(task);
            return task;
        }

        public Task CreateTask(Type type)
        {
            Task task = ScriptableObject.CreateInstance(type) as Task;
            if (task == null)
            {
                Console.WriteLine("fail to create task with Type {0}", type);
                return null;
            }
            task.BT = this;
            //task.GUID = System.Guid.NewGuid().ToString();
            _tasks.Add(task);
            return task;
        }

        public void RemoveTask(Task task)
        {
            _tasks.Remove(task);
        }
       
        public void OnAwake()
        {
            _entry.OnAwake();
        }

        /*public void OnStart()
        {
            _entry.OnStart();
        }*/

        public void OnUpdate()
        {
            if (IsUnactive)
            {
                return;
            }

            if (_isTurnCompleted)
            {
                Reset();
            }

            _isTurnCompleted = (_entry.OnUpdate() != TaskStatus.RUNNING);
            if (_isTurnCompleted)
            {
                OnTurnCompleted();                
            }
        }

        void Reset()
        {
            foreach (var task in _tasks)
            {
                task.Status = TaskStatus.NONE;
                if (task is BehaviorTreeReference)
                {
                    (task as BehaviorTreeReference).externalBTAsset.BT.Reset();
                }
            }
        }

        void OnTurnCompleted()
        {
            foreach (var task in _tasks)
            {                
                if (task is BehaviorTreeReference)
                {
                    (task as BehaviorTreeReference).externalBTAsset.BT.OnTurnCompleted();
                }
            }
        }

        public void OnDrawGizmos()
        {
            if (IsUnactive)
            {
                return;
            }

            foreach (var task in _tasks)
            {
                if (task.Status == TaskStatus.RUNNING || task is Conditional)
                {
                    task.OnDrawGizmos();
                }
            }
        }

        public void OnDrawGizmosSelected()
        {
            if (IsUnactive)
            {
                return;
            }

            foreach (var task in _tasks)
            {
                if (task.Status == TaskStatus.RUNNING || task is Conditional)
                {
                    task.OnDrawGizmosSelected();
                }
            }
        }


        /*public void OnFixedUpdate()
        {
            if (IsUnactive)
            {
                return;
            }
            _entry.OnFixedUpdate();
        }

        public void OnLateUpdate()
        {
            if (IsUnactive)
            {
                return;
            }
            _entry.OnFixedUpdate();
        }

        public void OnAnimatorIK(int layerIndex)
        {
            if (IsUnactive)
            {
                return;
            }
            _entry.OnFixedUpdate();
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (IsUnactive)
            {
                return;
            }
            _entry.OnFixedUpdate();
        }

        public void OnCollisionStay(Collision collision)
        {
            if (IsUnactive)
            {
                return;
            }
            _entry.OnFixedUpdate();
        }

        public void OnCollisionExit(Collision collision)
        {
            if (IsUnactive)
            {
                return;
            }
            _entry.OnFixedUpdate();
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsUnactive)
            {
                return;
            }
            _entry.OnFixedUpdate();
        }
        public void OnCollisionStay2D(Collision2D collision)
        {
            if (IsUnactive)
            {
                return;
            }
            _entry.OnFixedUpdate();
        }
        public void OnCollisionExit2D(Collision2D collision)
        {
            if (IsUnactive)
            {
                return;
            }
            _entry.OnFixedUpdate();
        }

        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (IsUnactive)
            {
                return;
            }
            _entry.OnFixedUpdate();
        }

        public void OnApplicationPause(bool pause)
        {
            if (IsUnactive)
            {
                return;
            }
            _entry.OnFixedUpdate();
        }

        public void OnTriggerEnter(Collider other)
        {

        }

        public void OnTriggerStay(Collider other)
        {

        }

        public void OnTriggerExit(Collider other)
        {

        }

        public void OnTriggerEnter2D(Collider2D collision)
        {

        }

        public void OnTriggerStay2D(Collider2D collision)
        {

        }

        public void OnTriggerExit2D(Collider2D collision)
        {

        }*/
    }
}