using UnityEngine;
using Phenix.Unity.Pattern;

namespace Phenix.Unity.DeviceInput
{
    /// <summary>
    /// 触摸屏输入
    /// </summary>
    public class FingerTouch : StandAloneSingleton<FingerTouch>
    {
        float _deltaX;
        float _deltaY;

        float _zoom;
        float _distanceOfTwoTouches;        

        /// <summary>
        /// 获得touch0的x偏移值[-1, 1]
        /// </summary>
        public float GetAxisX()
        {
            return _deltaX;
        }

        /// <summary>
        /// 获得touch0的y偏移值[-1, 1]
        /// </summary>
        public float GetAxisY()
        {
            return _deltaY;
        }

        /// <summary>
        /// 获得touch0, touch1的缩放值[-1, 1]
        /// </summary>
        public float GetAxisZoom()
        {
            return _zoom;
        }

        void HandleTouch()
        {
            if (UnityEngine.Input.touchCount == 1)
            {
                _deltaX = Mathf.Clamp(UnityEngine.Input.GetTouch(0).deltaPosition.x / 100, -1, 1);
                _deltaY = Mathf.Clamp(UnityEngine.Input.GetTouch(0).deltaPosition.y / 100, -1, 1);
            }
            else
            {
                _deltaX = _deltaY = 0;
            }
        }

        void HandleTouches()
        {
            if (UnityEngine.Input.touchCount <= 1)
            {
                _zoom = 0;
                return;
            }

            Touch finger0 = UnityEngine.Input.GetTouch(0);
            Touch finger1 = UnityEngine.Input.GetTouch(1);            

            if (finger1.phase == TouchPhase.Began)
            {
                _distanceOfTwoTouches = Vector2.Distance(finger0.position, finger1.position);
                _zoom = 0;
                return;
            }

            float tmp = _distanceOfTwoTouches;
            _distanceOfTwoTouches = Vector2.Distance(finger0.position, finger1.position);
            if (tmp == 0)
            {
                _zoom = 0;
            }
            else
            {
                _zoom = Mathf.Clamp((_distanceOfTwoTouches - tmp) / tmp, -1, 1);
            }
        }

        void Update()
        {
            HandleTouch();
            HandleTouches();            
        }
    }
}