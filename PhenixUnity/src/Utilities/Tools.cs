using UnityEngine;

namespace Phenix.Unity.Utilities
{
    public class Tools
    {
        public static string ColorString(string content, Color color)
        {
            return string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(color), content);
        }

        public static Texture2D CreateTexture(int width, int height, Color color)
        {            
            Color[] pixels = new Color[width*height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            Texture2D tex = new Texture2D(width, height);
            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }

        public static void CursorHide()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void CursorShow()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        /// <summary>
        /// 获取内置资源（mat、font、fbx等）
        /// </summary>        
        /// <param name="path">文件名.扩展名</param>        
        public static Object GetBuiltinResource(System.Type type, string path)
        {
            return Resources.GetBuiltinResource(type, path);
        }
    }
}