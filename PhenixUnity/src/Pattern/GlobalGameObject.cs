using UnityEngine;

namespace Phenix.Unity.Pattern
{
    /// <summary>
    /// 使挂载对象成为跨场景全局对象
    /// </summary>    
    public class GlobalGameObject : MonoBehaviour
    {
        void Awake()
        {        
            DontDestroyOnLoad(gameObject);
        }
    }
}