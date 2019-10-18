using UnityEngine;
using System;
using System.Collections.Generic;

namespace Phenix.Unity.Log
{
    public class LogData
    {
        public LogLevel logLevel;
        public string tag;
        public DateTime time;
        public string message;
        public string stackTrace;
    }

    [RequireComponent(typeof(Log))]
    public abstract class LogAppender : MonoBehaviour
    {
        Queue<LogData> _logs = new Queue<LogData>();

        // 是否日志调用堆栈信息
        //public bool logStackTrace = true;

        protected Queue<LogData> Logs { get { return _logs; } }

        protected virtual void OnEnable()
        {
            Application.logMessageReceived += LogCallBack;
        }

        protected virtual void OnDisable()
        {
            Application.logMessageReceived -= LogCallBack;
        }

        void LogCallBack(string condition, string stackTrace, LogType type)
        {
            LogData logData = new LogData();
            logData.time = DateTime.Now;
            logData.message = condition.Replace("\n", "").Replace("\r", "");
            logData.stackTrace = stackTrace.Replace("\n", "").Replace("\r", "");

            switch (type)
            {
                case LogType.Log:
                    logData.logLevel = LogLevel.INFO;
                    break;
                case LogType.Warning:
                    logData.logLevel = LogLevel.WARNING;
                    break;
                case LogType.Error:
                    logData.logLevel = LogLevel.ERROR;
                    break;
            }

            _logs.Enqueue(logData);
        }
    }
}