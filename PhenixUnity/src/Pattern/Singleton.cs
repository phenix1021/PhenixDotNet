using UnityEngine;

namespace Phenix.Unity.Pattern
{
    /// <summary>
    /// 单件模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remark>派生类继承该类时需要调用base.Awake()或不写Awake函数</remark>>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        static T _inst;

        public static T Instance
        {
            get
            {
                if (_inst == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T)) { hideFlags = HideFlags.HideInHierarchy };
                    DontDestroyOnLoad(obj);
                    _inst = obj.GetComponent<T>();                    
                }
                return _inst;
            }
        }
    }
}