using UnityEngine;

namespace Phenix.Unity.Pattern
{/*
    /// <summary>
    /// 统一注册各个单件的组件，该组件无需挂载对象
    /// 把本cs文件复制到项目中填充需要的单件组件即可
    /// </summary>
    public class RegisterGlobalSingleton : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod] // // 进程初始化时自动运行，无论该组件是否挂载对象
        static void Register()
        {
            GameObject global = new GameObject("global", typeof(Singleton派生组件1), typeof(Singleton派生组件2), ...)
                { hideFlags = HideFlags.HideInHierarchy }; // 可选
            DontDestroyOnLoad(global);            
        }
    }*/
}