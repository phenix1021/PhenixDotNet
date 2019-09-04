using UnityEngine;

namespace Phenix.Unity.Window
{
    /// <summary>
    /// 信息基类窗口
    /// </summary>
    public abstract class FloatWindow : MonoBehaviour
    {
        [Tooltip("窗体矩形")]
        public Rect windowRect = new Rect(0, 0, 500, 400);

        [Tooltip("窗体标题")]
        public string title = "";

        // 窗体是否收起
        bool _collapsed = false;

        // 收起时的窗体矩形的宽度
        float _collapsedWidth = 50;
        // 收起时的窗体矩形的高度
        float _collapsedHeight = 50;

        // Update is called once per frame
        void OnGUI()
        {
            GUI.backgroundColor = Color.yellow;
            GUI.contentColor = Color.white;            
            GUILayout.BeginArea(windowRect);
            windowRect = GUILayout.Window(_collapsed ? 0 : 1, windowRect, winProc, title);
            GUILayout.EndArea();
        }

        void winProc(int flag)
        {
            if (GUILayout.Button(_collapsed ? "+" : "-"))
            {
                _collapsed = !_collapsed;

                float w;
                w = windowRect.width;
                windowRect.width = _collapsedWidth;
                _collapsedWidth = w;

                float h;
                h = windowRect.height;
                windowRect.height = _collapsedHeight;
                _collapsedHeight = h;
            }
            switch (flag)
            {
                case 0:
                    break;
                case 1:
                    Draw();
                    break;
                default:
                    break;
            }
            GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
        }

        protected abstract void Draw();
    }
}