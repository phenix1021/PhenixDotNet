using UnityEngine;
using UnityEditor;

namespace Phenix.Unity.Editor.Control
{
    /// <summary>
    /// 各种GUI控件
    /// </summary>
    public static class EditorControl
    {
        // ---------------------------逻辑类控件------------------------------

        /// <summary>
        /// 开始监控控件数据变化
        /// 和EndCheckControlChange配合使用
        /// </summary>
        public static void BeginCheckControlChange()
        {
            EditorGUI.BeginChangeCheck();
        }

        /// <summary>
        /// 停止监控控件数据变化，返回是否变化
        /// 和BeginCheckControlChange配合使用
        /// </summary>
        public static bool EndCheckControlChange()
        {
            return EditorGUI.EndChangeCheck();
        }

        /// <summary>
        /// check控件组。如果val==false则位于IfGroup和EndIfGroup之间的控件都灰显
        /// </summary>
        public static void IfEnableCheckGroup(string head, string tip, ref bool val)
        {
            val = EditorGUILayout.BeginToggleGroup(new GUIContent(head, tip), val);
        }

        public static bool IfEnableCheckGroup(string head, string tip, bool val)
        {
            return EditorGUILayout.BeginToggleGroup(new GUIContent(head, tip), val);
        }

        public static void EndIfEnableCheckGroup()
        {
            EditorGUILayout.EndToggleGroup();
        }

        /// <summary>
        /// 折叠开关
        /// 范例：
        /// bool showWeapons;
        /// showWeapons = EditorGUILayout.Foldout(showWeapons,"Weapons");
        /// if(showWeapons)
        /// {
        ///    player.weaponDamage1 = EditorGUILayout.FloatField("Weapon 1 Damage",player.weaponDamage1);
        ///    player.weaponDamage2 = EditorGUILayout.FloatField("Weapon 2 Damage",player.weaponDamage2);
        /// }
        /// </summary>
        public static void Foldout(string head, string tip, ref bool onOrOff)
        {
            onOrOff = EditorGUILayout.Foldout(onOrOff, new GUIContent(head, tip));
        }

        public static bool Foldout(string head, string tip, bool onOrOff)
        {
            return EditorGUILayout.Foldout(onOrOff, new GUIContent(head, tip));
        }

        public static bool Button(string head, string tip)
        {
            return GUILayout.Button(new GUIContent(head, tip));
        }

        // ---------------------------布局类控件------------------------------

        /// <summary>
        /// 开始水平布局
        /// </summary>
        public static void BeginHLayout()
        {
            EditorGUILayout.BeginHorizontal();
        }

        /// <summary>
        /// 停止水平布局
        /// </summary>
        public static void EndHLayout()
        {
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 开始垂直布局
        /// </summary>
        public static void BeginVLayout()
        {
            EditorGUILayout.BeginVertical();
        }

        /// <summary>
        /// 停止垂直布局
        /// </summary>
        public static void EndVLayout()
        {
            EditorGUILayout.EndVertical();
        }

        // ---------------------------可编辑控件------------------------------

        public static void IntField(string head, string tip, ref int val)
        {
            val = EditorGUILayout.IntField(new GUIContent(head, tip), val);
        }

        public static int IntField(string head, string tip, int val)
        {
            return EditorGUILayout.IntField(new GUIContent(head, tip), val);
        }

        /// <summary>
        /// 延迟int字段，即输入之后要按下enter或移开输入焦点时才会返回结果值
        /// </summary>
        public static void DelayedIntField(string head, string tip, ref int val)
        {
            val = EditorGUILayout.DelayedIntField(new GUIContent(head, tip), val);
        }

        public static int DelayedIntField(string head, string tip, int val)
        {
            return EditorGUILayout.DelayedIntField(new GUIContent(head, tip), val);
        }

        public static void FloatField(string head, string tip, ref float val)
        {
            val = EditorGUILayout.FloatField(new GUIContent(head, tip), val);
        }

        public static float FloatField(string head, string tip, float val)
        {
            return EditorGUILayout.FloatField(new GUIContent(head, tip), val);
        }
        /// <summary>
        /// 延迟float字段，即输入之后要按下enter或移开输入焦点时才会返回结果值
        /// </summary>
        public static void DelayedFloatField(string head, string tip, ref float val)
        {
            val = EditorGUILayout.DelayedFloatField(new GUIContent(head, tip), val);
        }

        public static float DelayedFloatField(string head, string tip, float val)
        {
            return EditorGUILayout.DelayedFloatField(new GUIContent(head, tip), val);
        }

        public static void DoubleField(string head, string tip, ref double val)
        {
            val = EditorGUILayout.DoubleField(new GUIContent(head, tip), val);
        }

        public static double DoubleField(string head, string tip, double val)
        {
            return EditorGUILayout.DoubleField(new GUIContent(head, tip), val);
        }

        /// <summary>
        /// 延迟double字段，即输入之后要按下enter或移开输入焦点时才会返回结果值
        /// </summary>
        public static void DelayedDoubleField(string head, string tip, ref double val)
        {
            val = EditorGUILayout.DelayedDoubleField(new GUIContent(head, tip), val);
        }

        public static double DelayedDoubleField(string head, string tip, double val)
        {
            return EditorGUILayout.DelayedDoubleField(new GUIContent(head, tip), val);
        }

        public static void BoolField(string head, string tip, ref bool val)
        {
            val = EditorGUILayout.Toggle(new GUIContent(head, tip), val);
        }

        public static bool BoolField(string head, string tip, bool val)
        {
            return EditorGUILayout.Toggle(new GUIContent(head, tip), val);
        }

        public static void Vector3Field(string head, string tip, ref Vector3 val)
        {
            val = EditorGUILayout.Vector3Field(new GUIContent(head, tip), val);
        }

        public static Vector3 Vector3Field(string head, string tip, Vector3 val)
        {
            return EditorGUILayout.Vector3Field(new GUIContent(head, tip), val);
        }

        public static void Vector2Field(string head, string tip, ref Vector2 val)
        {
            val = EditorGUILayout.Vector2Field(new GUIContent(head, tip), val);
        }

        public static Vector2 Vector2Field(string head, string tip, Vector2 val)
        {
            return EditorGUILayout.Vector2Field(new GUIContent(head, tip), val);
        }

        public static void Vector4Field(string head, string tip, ref Vector4 val)
        {
            val = EditorGUILayout.Vector4Field(new GUIContent(head, tip), val);
        }

        public static Vector4 Vector4Field(string head, string tip, Vector4 val)
        {
            return EditorGUILayout.Vector4Field(new GUIContent(head, tip), val);
        }

        public static void ColorField(string head, string tip, ref Color val)
        {
            val = EditorGUILayout.ColorField(new GUIContent(head, tip), val);
        }

        public static Color ColorField(string head, string tip, Color val)
        {
            return EditorGUILayout.ColorField(new GUIContent(head, tip), val);
        }
        
        public static void CurveField(string head, string tip, AnimationCurve val)
        {
            EditorGUILayout.CurveField(new GUIContent(head, tip), val);
        }

        public static void RectField(string head, string tip, ref Rect val)
        {
            val = EditorGUILayout.RectField(new GUIContent(head, tip), val);
        }

        public static Rect RectField(string head, string tip, Rect val)
        {
            return EditorGUILayout.RectField(new GUIContent(head, tip), val);
        }

        public static void ObjectField(string head, string tip, Object val, bool allowSceneObjects = true)
        {
            EditorGUILayout.ObjectField(new GUIContent(head, tip), val, typeof(Object), allowSceneObjects);
        }

        public static void StringField(string head, string tip, string val)
        {
            EditorGUILayout.TextField(new GUIContent(head, tip), val);
        }

        /// <summary>
        /// 延迟文本字段，即输入之后要按下enter或移开输入焦点时才会返回结果值
        /// </summary>
        public static void DelayedStringField(string head, string tip, string val)
        {
            EditorGUILayout.DelayedTextField(new GUIContent(head, tip), val);
        }

        public static void TextField(string val)
        {
            EditorGUILayout.TextArea(val);
        }

        public static void SliderFloatField(string head, string tip, ref float val, float min, float max)
        {
            val = EditorGUILayout.Slider(new GUIContent(head, tip), val, min, max);
        }

        public static float SliderFloatField(string head, string tip, float val, float min, float max)
        {
            return EditorGUILayout.Slider(new GUIContent(head, tip), val, min, max);
        }

        public static void PropertyField(string head, string tip, SerializedProperty sp)
        {
            EditorGUILayout.PropertyField(sp, new GUIContent(head, tip), true);
        }

        public static void LayerField(string head, string tip, ref int val)
        {
            val = EditorGUILayout.LayerField(new GUIContent(head, tip), val);
        }

        public static int LayerField(string head, string tip, int val)
        {
            return EditorGUILayout.LayerField(new GUIContent(head, tip), val);
        }

        public static void TagField(string head, string tip, string val)
        {
            EditorGUILayout.TagField(new GUIContent(head, tip), val);
        }

        /// <summary>
        /// 单选enum
        /// </summary>
        public static System.Enum EnumField(string head, string tip, System.Enum val)
        {
            return EditorGUILayout.EnumPopup(new GUIContent(head, tip), val);
        }

        /// <summary>
        /// 单选、多选enum（如mask的everything、nothing、mixedXXX）
        /// </summary>
        public static System.Enum EnumMaskField(string head, string tip, System.Enum val)
        {
            // 注意：Unity2019 EditorGUILayout.EnumMaskField已过时，可以用EnumFlagsField
            return EditorGUILayout.EnumMaskField(new GUIContent(head, tip), val);
        }

        // ---------------------------不可编辑控件------------------------------

        /// <summary>
        /// 添加label，不可编辑
        /// </summary>
        public static void Label(string label, float width = 0)
        {
            if (width > 0)
            {
                EditorGUILayout.LabelField(new GUIContent(label), GUILayout.Width(width));
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent(label));
            }
        }

        /// <summary>
        /// 添加space，不可编辑
        /// </summary>
        public static void Space(float width = 0)
        {
            if (width == 0)
            {
                GUILayout.FlexibleSpace();
            }
            else
            {
                GUILayout.Space(width);
            }
        }

        /// <summary>
        /// 消息框
        /// </summary>
        public static void MessageBox(string message, MessageType msgType)
        {
            EditorGUILayout.HelpBox(message, msgType);
        }

        /// <summary>
        /// 进度条
        /// </summary>
        public static void ProgressBar(string text, float progress, float barHeight, Color barColor)
        {
            Rect rect = GUILayoutUtility.GetRect(0, barHeight);
            Color preColor = GUI.color;
            GUI.color = barColor;
            EditorGUI.ProgressBar(rect, progress, text);
            GUI.color = preColor;
        }

    }
}