using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.UI
{
    /// <summary>
    /// 可双击组件
    /// </summary>
    /// <mark>
    /// 1. 对于UI对象没有其它额外要求。
    /// 2. 对于一般场景对象，要求所挂载对象或其子对象必须有Collider组件，
    ///    并且主摄像机有Physics Raycaster组件，且场景对象的layer须符合
    ///    摄像机的culling mask
    /// </mark>>
    [AddComponentMenu("Phenix/UI/DoubleClickable")]
    public class DoubleClickable : MonoBehaviour, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [System.Serializable]
        public class DoubleClickEvent : UnityEvent { }

        public DoubleClickEvent onDoubleClick;        

        float _firstClickTime = 0;
        float _secondClickTime = 0;

        [Tooltip("是否透传pointer事件")]
        public bool passEvent = false;

        void DoubleClick()
        {
            if (onDoubleClick != null)
            {
                onDoubleClick.Invoke();
            }
            ResetClickTime();
        }       

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_firstClickTime == 0)
            {
                _firstClickTime = Time.timeSinceLevelLoad;
            }
            else
            {
                _secondClickTime = Time.timeSinceLevelLoad;
            }

            if (passEvent)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.pointerDownHandler);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            float diff = 0;
            if (_firstClickTime > 0 && _secondClickTime > 0)
            {
                diff = _secondClickTime - _firstClickTime;
                if (diff <= 400)
                {
                    DoubleClick();
                }
                else
                {
                    ResetClickTime();
                }
            }

            if (passEvent)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.pointerUpHandler);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {            
            ResetClickTime();

            if (passEvent)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.pointerExitHandler);
            }
        }

        void ResetClickTime()
        {
            _firstClickTime = _secondClickTime = 0;
        }     
    }
}
