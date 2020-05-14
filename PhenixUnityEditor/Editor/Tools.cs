using UnityEngine;
using UnityEditor;

namespace Phenix.Unity.Editor.Utilities
{

    public static class Tools
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

        public static void DrawRect(Rect rect, Color color)
        {            
            EditorGUI.DrawRect(rect, color);
        }

        /// <summary>
        /// 获取内置资源（mat、font、fbx等）
        /// </summary>        
        /// <param name="path">文件名.扩展名</param>    
        public static Object GetBuiltinExtraResource(System.Type type, string path)
        {
            return AssetDatabase.GetBuiltinExtraResource(type, path);
        }

        public static float GetSingleLineHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public static float GetCurrentViewWidth()
        {
            return EditorGUIUtility.currentViewWidth;
        }

        /* 关于GUISkin和GUIStyle
         1. GUISkin是资源文件，由许多style组成，如box、button、lable、customStyles等等。
         2. unity内置了一些skin以及style：            
            2.1 GUISkin gameSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Game) 可读写
            2.2 GUISkin sceneSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene) 可读写
            2.3 GUISkin inspectorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector) 可读写
            2.4 GUI.skin  可读写。这个最常用（如创建gui控件时填入对应GUIStyle，GUI.skin.box、GUI.skin.label等，
                以及创建自己的GUISkin时也常作为模板，如 GUIStyle style = new GUIStyle(GUI.skin.box)）
            2.5 EditorStyles.*** 包含许多只读的属性，如内置Font、GUIStyle等
         */
    }
}