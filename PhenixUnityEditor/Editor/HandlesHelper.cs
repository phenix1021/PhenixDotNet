using UnityEngine;
using UnityEditor;

namespace Phenix.Unity.Editor.Utilities
{
    public static class HandlesHelper
    {
        /// <summary>
        /// 在scene窗口绘制点
        /// </summary>
        /// <param name="pos">点的位置</param>        
        /// <param name="color">点的颜色</param>        
        /// 返回是否选中
        public static bool DrawDot(Vector3 pos, Color color)
        {
            Color preColor = Handles.color;
            Handles.color = color;
            float handleSize = HandleUtility.GetHandleSize(pos);
            // 创建交互点
            bool ret = Handles.Button(pos, Quaternion.identity, 0.06f * handleSize,
                0.08f * handleSize, Handles.DotHandleCap);
            Handles.color = preColor;
            return ret;
        }

        /// <summary>
        /// 在scene窗口指定位置添加移动轴（xyz轴）
        /// </summary>
        /// <param name="pos"></param>
        public static void DrawXYZAxis(ref Vector3 pos)
        {
            pos = Handles.DoPositionHandle(pos, Quaternion.identity);
        }

        /// <summary>
        /// 绘制线框球
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawWireSphere(Vector3 pos, float radius, Color color)
        {
            Color preColor = Handles.color;
            Handles.color = color;
            Handles.DrawWireArc(pos, Vector3.forward, Vector3.up, 360f, radius);
            Handles.DrawWireArc(pos, Vector3.up, Vector3.forward, 360f, radius);
            Handles.DrawWireArc(pos, Vector3.right, Vector3.up, 360f, radius);
            Handles.color = preColor;
        }

        /// <summary>
        /// Draws text on the scene
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        /// <param name="color"></param>
        public static void DrawLabel(string text, Vector3 pos, Color color)
        {
            Color preColor = GUI.color;
            GUI.color = color;
            Handles.color = color;
            Handles.Label(pos, text);
            GUI.color = preColor;
            Handles.color = preColor;
        }
    }
}