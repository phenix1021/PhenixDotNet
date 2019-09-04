using System;

namespace Phenix.Unity.AI
{
    /// <summary>
    /// 自定义task图标文件
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TaskIconAttribute : Attribute
    {
        string _iconFile;

        public TaskIconAttribute(string iconPath)
        {
            _iconFile = iconPath;
        }

        public string IconFile { get { return _iconFile; } }
    }

    /// <summary>
    /// task功能描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TaskDescriptionAttribute : Attribute
    {
        string _desc;

        public TaskDescriptionAttribute(string desc)
        {
            _desc = desc;
        }

        public string Desc { get { return _desc; } }
    }
}