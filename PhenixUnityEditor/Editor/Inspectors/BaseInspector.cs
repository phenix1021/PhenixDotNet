using UnityEditor;

namespace Phenix.Unity.Editor.Inspector
{
    public abstract class BaseInspector : UnityEditor.Editor
    {
        /// <summary>
        /// 获得指定成员(要求可序列化)的SerializedProperty，尤其适用于自定义类型成员对象。        
        /// </summary>
        /// <param name="memberName">成员变量名</param>
        protected SerializedProperty FindMember(string memberName)
        {
            return serializedObject.FindProperty(memberName);
        }

        /// <summary>
        /// 对于基本类型的成员可以通过IntField、FloatField等控件，配合（target as 对应对象类型）来读写。
        /// 对于自定义类型成员只能通过FindMember获得成员的SerializedProperty，然后配合PropertyField控件读写。
        /// </summary>
        public sealed override void OnInspectorGUI()
        {
            // 同步最新值
            serializedObject.Update();
            // 绘制Inpector内容
            ProcessControls();
            // 提交改变
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnEnable() { }
        protected virtual void ProcessControls() { }
        protected virtual void OnSceneGUI() { }
    }
}