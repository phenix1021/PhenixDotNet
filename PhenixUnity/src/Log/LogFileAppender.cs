using UnityEngine;
using System.Text;
using System.IO;
using System;

namespace Phenix.Unity.Log
{
    /// <summary>
    /// 日志输出到文件
    /// </summary>
    [AddComponentMenu("Phenix/Log")]
    public class LogFileAppender : LogAppender
    {        
        // 每帧处理的日志数量
        public int logCountPerFrame = 50;

        StreamWriter _sw;        

        private void Awake()
        {
            string logPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "log" + Path.DirectorySeparatorChar;
            if (Directory.Exists(logPath) == false)
            {
                Directory.CreateDirectory(logPath);
            }
            string fileName = logPath + DateTime.Now.ToString("yyyyMMdd") + ".log";
            if (File.Exists(fileName) == false)
                _sw = File.CreateText(fileName);
            else
                _sw = File.AppendText(fileName);
        }

        private void OnDestroy()
        {
            if (_sw != null)
            {
                _sw.Close();
            }
        }

        string GetContent(LogData logData)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(logData.time.ToString("yyyy/MM/dd HH:mm:ss")).
                Append(" [").Append(logData.logLevel).Append("] ");
            if (string.IsNullOrEmpty(logData.message) == false)
            {
                sb.Append("message: ").Append(logData.message);
            }
            if (string.IsNullOrEmpty(logData.stackTrace) == false)
            {
                sb.Append(" stackTrace: ").Append(logData.stackTrace);
            }
            return sb.ToString();
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < logCountPerFrame && Logs.Count > 0; i++)
            {
                _sw.WriteLine(GetContent(Logs.Dequeue()));
                _sw.Flush();
            }
        }
    }
}