using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.UI
{
    /// <summary>
    /// 可点击组件
    /// </summary>
    /// <mark>
    /// 1. 对于UI对象没有其它额外要求。
    /// 2. 对于一般场景对象，要求所挂载对象或其子对象必须有Collider组件，
    ///    并且主摄像机有Physics Raycaster组件，且场景对象的layer须符合
    ///    摄像机的culling mask
    /// </mark>>
    [AddComponentMenu("Phenix/UI/Clickable")]
    public class Clickable : MonoBehaviour, IPointerClickHandler
    {
        [System.Serializable]
        public class ClickEvent : UnityEvent { }

        public ClickEvent onClick;        

        [Tooltip("是否透传pointer事件")]
        public bool passEvent = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null)
            {
                onClick.Invoke();
            }

            if (passEvent)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.pointerClickHandler);
            }
        }
    }
}
