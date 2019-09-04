using System;
using UnityEngine;

namespace Phenix.Unity.AI
{
    [Serializable]
    public abstract class SharedVariable { }
    
    [Serializable]
    public abstract class SharedVariable<T> : SharedVariable
    {
        [SerializeField]
        bool _dynamic = true;

        [SerializeField]
        protected T value;

        [SerializeField]
        protected string name;

        Blackboard _blackboard;

        protected Blackboard Blackboard { get { return _blackboard; } set { _blackboard = value; } }
        public string Name { get { return name; } set { name = value; } }

        public bool IsDynamic() { return _dynamic; }

        public SharedVariable(Blackboard blackboard, bool dynamic = true)
        {
            _blackboard = blackboard;
            _dynamic = dynamic;
        }
        
        protected virtual T GetStaticValue()
        {
            return value;
        }

        protected virtual void SetStaticValue(T val)
        {
            value = val;
        }

        public T Value
        {
            get
            {
                if (_dynamic)
                {
                    // 动态
                    return (T)_blackboard.Get(Name);
                }
                // 静态
                return GetStaticValue();
            }
            set
            {
                if (_dynamic)
                {
                    // 动态
                    _blackboard.Set(Name, value);
                }
                else
                {
                    // 静态
                    SetStaticValue(value);
                }
                
            }
        }
    }

    [Serializable]
    public abstract class SharedGameObject : SharedVariable<GameObject>
    {
        public SharedGameObject(Blackboard blackboard, bool dynamic = true)
            : base(blackboard, dynamic) { }

        protected override GameObject GetStaticValue()
        {
            if (value == null && string.IsNullOrEmpty(name) == false)
            {
                value = GameObject.Find(name);
            }
            return value;
        }

        protected override void SetStaticValue(GameObject val)
        {
            value = val;
            name = val.name;
        }
    }
    
}