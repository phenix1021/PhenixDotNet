using UnityEngine;

namespace Phenix.Unity.Pattern
{
    /// <summary>
    /// 独立单件模式（不需要显式挂接对象，但无法在设计期序列化字段）
    /// </summary>
    public abstract class StandAloneSingleton<T> : MonoBehaviour where T : StandAloneSingleton<T>
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