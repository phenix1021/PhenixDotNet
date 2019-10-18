using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Pattern;

namespace Phenix.Unity.Log
{
    public enum LogLevel
    {
        INFO = 0,
        WARNING,        
        ERROR,
    }

    /// <summary>
    /// 日志系统
    /// </summary>
    [AddComponentMenu("Phenix/Log")]
    public class Log : Singleton<Log>
    {
        // 日志级别
        public LogLevel level = LogLevel.INFO;

        // tag过滤集，只日志相应tag的条目，空则日志所有
        public List<string> tagFilter = new List<string>();
     
        public void Info(string message, UnityEngine.Object obj = null)
        {
            Info(string.Empty, message, obj);
        }

        public void Info(string tag, string message, UnityEngine.Object obj = null)
        {
            if (level > LogLevel.INFO)
            {
                return;
            }
            if (string.IsNullOrEmpty(tag) == false && tagFilter.Contains(tag) == false)
            {
                return;
            }
            if (string.IsNullOrEmpty(tag))
            {
                Debug.Log(message, obj);
            }
            else
            {
                Debug.LogFormat("[{%0}] {%1}", tag, message, obj);
            }
        }

        public void Warning(string message, UnityEngine.Object obj = null)
        {
            Warning(string.Empty, message, obj);
        }

        public void Warning(string tag, string message, UnityEngine.Object obj = null)
        {
            if (level > LogLevel.WARNING)
            {
                return;
            }
            if (string.IsNullOrEmpty(tag) == false && tagFilter.Contains(tag) == false)
            {
                return;
            }
            if (string.IsNullOrEmpty(tag))
            {
                Debug.LogWarning(message, obj);
            }
            else
            {
                Debug.LogWarningFormat("[{%0}] {%1}", tag, message, obj);
            }
        }
        public void Error(string message, UnityEngine.Object obj = null)
        {
            Error(string.Empty, message, obj);
        }

        public void Error(string tag, string message, UnityEngine.Object obj = null)
        {
            if (level > LogLevel.ERROR)
            {
                return;
            }
            if (string.IsNullOrEmpty(tag) == false && tagFilter.Contains(tag) == false)
            {
                return;
            }
            if (string.IsNullOrEmpty(tag))
            {
                Debug.LogError(message, obj);
            }
            else
            {
                Debug.LogErrorFormat("[{%0}] {%1}", tag, message, obj);
            }
            
        }

    }
} 
