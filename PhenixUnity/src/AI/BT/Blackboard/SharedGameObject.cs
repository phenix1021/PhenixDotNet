using System;
using UnityEngine;
using Phenix.Unity.Extend;

namespace Phenix.Unity.AI.BT
{
    [Serializable]
    public class SharedGameObject : SharedVariable<GameObject>
    {
        public SharedGameObject(Blackboard blackboard, bool dynamic = true)
            : base(blackboard, dynamic) { }
/*
        protected override GameObject GetStaticValue()
        {
            if (name.StartsWith("0:"))
            {
                // 参数name以“0:”开头约定为场景对象
                return GameObject.Find(name.Substring(2));
            }
            else if (name.StartsWith("1:"))
            {
                // 参数name以“1:”开头约定为prefab
                return Resources.Load(name.Substring(2)) as GameObject;
            }
            else
            {
                Debug.LogError("invalid name in SharedGameObject, must start with '0:' or '1:'!");
                return null;
            }
        }

        protected override void SetStaticValue(GameObject val)
        {            
            name = val.FullName();
            base.SetStaticValue(val);
        }*/
    }
}
