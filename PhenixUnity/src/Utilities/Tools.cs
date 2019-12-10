using UnityEngine;

namespace Phenix.Unity.Utilities
{
    public class Tools
    {
        public static string ColorString(string content, Color color)
        {
            return string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(color), content);
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
    }
}