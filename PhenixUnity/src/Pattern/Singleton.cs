using UnityEngine;

namespace Phenix.Unity.Pattern
{
    /// <summary>
    /// 单件模式
    /// </summary>
    /// <remark>派生类继承该类时需要调用base.Awake()或不写Awake函数</remark>>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            Instance = (T)this;
        }
    }
}