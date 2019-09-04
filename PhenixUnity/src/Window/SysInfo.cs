using UnityEngine;
using System.Text;

namespace Phenix.Unity.Window
{
    /// <summary>
    /// 系统信息窗口
    /// </summary>
    [AddComponentMenu("Phenix/Window")]
    public class SysInfo : FloatWindow
    {
        public bool fps = true;
        public bool operatingSystem = true;
        public bool processorType = true;
        public bool processorFrequency = true;
        public bool processorCount = true;
        public bool systemMemorySize = true; 
        public bool graphicsMemorySize = true;
        public bool deviceType = true;

        StringBuilder _sb = new StringBuilder();
        
        float _fps;
        int _fpsCounter = 0;
        // fps采样周期
        const float _fpsSamplePeriod = 0.5f;
        // fps下次采样时间
        float _fpsNextSampleTime;

        void UpdateFPS()
        {
            ++_fpsCounter;
            if (Time.realtimeSinceStartup >= _fpsNextSampleTime)
            {
                _fps = (int)(_fpsCounter / _fpsSamplePeriod);
                _fpsCounter = 0;
                _fpsNextSampleTime += _fpsSamplePeriod;
            }
        }

        private void Update()
        {
            UpdateFPS();
        }

        private void Awake()
        {
            _fpsNextSampleTime = Time.realtimeSinceStartup + _fpsSamplePeriod;
        }

        protected override void Draw()
        {            
            if (fps)
            {
                GUILayout.Label("fps: " + _fps.ToString());
            }
            if (operatingSystem)
            {
                GUILayout.Label("operating system: " + SystemInfo.operatingSystem);
            }
            if (processorType)
            {
                GUILayout.Label("processor type: " + SystemInfo.processorType);
            }
            if (processorFrequency)
            {
                GUILayout.Label("processor frequency: " + SystemInfo.processorFrequency.ToString());
            }
            if (processorCount)
            {
                GUILayout.Label("processor count: " + SystemInfo.processorCount.ToString());
            }
            if (systemMemorySize)
            {
                GUILayout.Label("system memory size: " + SystemInfo.systemMemorySize.ToString());
            }
            if (graphicsMemorySize)
            {
                GUILayout.Label("graphics memory size: " + SystemInfo.graphicsMemorySize.ToString());
            }
            if (deviceType)
            {
                GUILayout.Label("device type: " + SystemInfo.deviceType.ToString());
            }            
        }
    }
}