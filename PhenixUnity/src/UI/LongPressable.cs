using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Phenix.Unity.UI
{
    /// <summary>
    /// 可长按组件
    /// </summary>
    /// <mark>
    /// 1. 对于UI对象没有其它额外要求。
    /// 2. 对于一般场景对象，要求所挂载对象或其子对象必须有Collider组件，
    ///    并且主摄像机有Physics Raycaster组件，且场景对象的layer须符合
    ///    摄像机的culling mask
    /// </mark>>
    [AddComponentMenu("Phenix/UI/LongPressable")]
    public class LongPressable : MonoBehaviour, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [System.Serializable]
        public class LongPressEvent : UnityEvent<float/*秒*/> { }

        public float minPressingSeconds = 0.6f;        // 长按最短时长（秒）

        public LongPressEvent onLongPressing;          // 长按中 
        public LongPressEvent onLongPressed;           // 长按完成
        public LongPressEvent onLongPressAbort;        // 长按中断

        float _pressDownTime = 0;                      

        void LongPressing(float second)
        {
            if (onLongPressing != null)
            {                
                onLongPressing.Invoke(second);
            }            
        }

        void LongPressed(float second)
        {
            if (onLongPressed != null)
            {
                onLongPressed.Invoke(second);
            }
            ResetLongPressTime();
        }

        void LongPressAbort(float second)
        {
            if (onLongPressAbort != null)
            {
                onLongPressAbort.Invoke(second);
            }
            ResetLongPressTime();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressDownTime = Time.timeSinceLevelLoad;            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsLongPressing())
            {
                float diff = Time.timeSinceLevelLoad - _pressDownTime;
                LongPressed(diff);                
            }
            ResetLongPressTime();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsLongPressing())
            {
                float diff = Time.timeSinceLevelLoad - _pressDownTime;
                LongPressAbort(diff);
            }
            ResetLongPressTime();
        }
        
        void ResetLongPressTime()
        {
            _pressDownTime = 0;
        }

        bool IsLongPressing()
        {
            return _pressDownTime > 0 && Time.timeSinceLevelLoad - _pressDownTime >= minPressingSeconds;
        }

        void Update()
        {
            if (IsLongPressing())
            {
                float diff = Time.timeSinceLevelLoad - _pressDownTime;                
                LongPressing(diff);
            }
        }
    }
}
