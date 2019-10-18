using UnityEngine;
using System.Text;

namespace Phenix.Unity.Log
{
    /// <summary>
    /// 日志输出到界面
    /// </summary>
    [AddComponentMenu("Phenix/Log")]
    public class LogGUIAppender : LogAppender
    {

        // 窗体矩形
        public Rect formRect = new Rect(0, 0, 500, 400);

        // 窗体标题
        public string title;

        // 窗体是否折叠
        bool _collapsed = false;

        // 收起时的窗体矩形的宽度
        float _collapsedWidth = 50;
        // 收起时的窗体矩形的高度
        float _collapsedHeight = 50;

        Vector2 _scrollViewPos = new Vector2(0, 0);

        // Update is called once per frame
        void OnGUI()
        {
            GUI.backgroundColor = Color.yellow;
            GUI.contentColor = Color.white;
            GUILayout.BeginArea(formRect);
            formRect = GUILayout.Window(_collapsed ? 0 : 1, formRect, winProc, title);
            GUILayout.EndArea();
        }

        void winProc(int flag)
        {
            if (GUILayout.Button(_collapsed ? "+" : "-"))
            {
                _collapsed = !_collapsed;

                float w;
                w = formRect.width;
                formRect.width = _collapsedWidth;
                _collapsedWidth = w;

                float h;
                h = formRect.height;
                formRect.height = _collapsedHeight;
                _collapsedHeight = h;
            }
            switch (flag)
            {
                case 0:
                    break;
                case 1:
                    DrawLog();
                    break;
                default:
                    break;
            }
            GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
        }

        string GetContent(LogData log)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(log.time.ToString("yyyy/MM/dd HH:mm:ss")).
            Append(" [").Append(log.logLevel).Append("] ");
            if (string.IsNullOrEmpty(log.message) == false)
            {
                sb.Append("message: ").Append(log.message);
            }
            if (string.IsNullOrEmpty(log.stackTrace) == false)
            {
                sb.Append(" stackTrace: ").Append(log.stackTrace);
            }
            return sb.ToString();
        }

        // Update is called once per frame
        void DrawLog()
        {
            _scrollViewPos = GUILayout.BeginScrollView(_scrollViewPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            foreach (var log in Logs)
            {
                string content = GetContent(log);
                switch (log.logLevel)
                {
                    case LogLevel.INFO:
                        GUI.contentColor = Color.white;
                        break;
                    case LogLevel.WARNING:
                        GUI.contentColor = Color.yellow;
                        break;
                    default:
                        GUI.contentColor = Color.red;
                        break;
                }
                GUILayout.Label(content);
            }
            GUILayout.EndScrollView();
        }
    }
}