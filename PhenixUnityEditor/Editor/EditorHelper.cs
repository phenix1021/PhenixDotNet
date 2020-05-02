using UnityEngine;
using UnityEditor;

namespace Phenix.Unity.Editor.Utilities
{

    public static class EditorHelper
    {
        /// <summary>
        /// 游戏是否正在play
        /// </summary>
        public static bool IsPlaying { get { return EditorApplication.isPlaying; } }

        /// <summary>
        /// 给资源（如Texture2D、ScriptObject等）打flag，标记其数据改变，需要存储到硬盘
        /// </summary>
        public static void SetAssetDirty(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);
        }

        /// <summary>
        /// 给当前场景打标记，说明其数据改变，需要存储
        /// </summary>
        public static void SetSceneDirty()
        {            
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        /// <summary>
        /// Pulls variables from runtime so we have the latest values.
        /// 常用在Editor.OnInspectorGUI()的开始
        /// </summary>
        public static void UpdateValue(SerializedObject so)
        {
            so.Update();
        }

        /// <summary>
        /// Inspector中获得指定成员的SerializedProperty
        /// 对于基本类型的成员可以通过IntField、FloatField来访问，
        /// 对于自定义类型成员就只能通过本方法然后再由PropertyField访问
        /// </summary>
        /// <param name="so">如Inspector中的serializedObject</param>
        /// <param name="memberName">成员变量名</param>
        public static SerializedProperty FindSP(SerializedObject so, string memberName)
        {
            return so.FindProperty(memberName);
        }

        /// <summary>
        /// Pushes the values back to the runtime so it has the changes
        /// 常用在Editor.OnInspectorGUI()的结尾
        /// </summary>
        public static void Submit(SerializedObject so)
        {
            so.ApplyModifiedProperties();
        }        

        /// <summary>
        /// 在scene窗口绘制点
        /// </summary>
        /// <param name="pos">点的位置</param>        
        /// <param name="color">点的颜色</param>        
        /// 返回是否选中
        public static bool Dot(Vector3 pos, Color color)
        {            
            Handles.color = color;            
            float handleSize = HandleUtility.GetHandleSize(pos);
            // 创建交互点
            return Handles.Button(pos, Quaternion.identity, 0.06f * handleSize, 
                0.08f * handleSize, Handles.DotHandleCap);            
        }

        /// <summary>
        /// 在scene窗口指定位置添加移动轴（xyz轴）
        /// </summary>
        /// <param name="pos"></param>
        public static void AddMoveAxis(ref Vector3 pos)
        {
            pos = Handles.DoPositionHandle(pos, Quaternion.identity);
        }
    }
}