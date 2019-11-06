using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Phenix.Unity.Pattern;

namespace Phenix.Unity.Utilities
{
    public class UITools : StandAloneSingleton<UITools>
    {        
        // pointer是否在当前UI对象(即调用本函数的UI对象)上
        public bool IsPointerOverUIGameObject()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        // pointer事件透传（从当前UI对象（即调用本函数的UI对象）向所覆盖的其它对象发射射线透传eventData）
        public void PassPointerEvent<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> func, GameObject target = null) 
            where T : IEventSystemHandler
        {
            List<RaycastResult> rlt = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, rlt);
            foreach (var item in rlt)
            {
                if (target && target != item.gameObject)
                {
                    // 如果不是指定目标
                    continue;
                }

                if (item.gameObject == eventData.pointerCurrentRaycast.gameObject)
                {
                    // 过滤当前对象，否则会死循环
                    continue;
                }

                ExecuteEvents.Execute(item.gameObject, eventData, func);
            }
        }

        public void PassPointerEventClick(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.pointerClickHandler, target);
        }

        public void PassPointerEventDown(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.pointerDownHandler, target);
        }

        public void PassPointerEventUp(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.pointerUpHandler, target);
        }

        public void PassPointerEventEnter(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.pointerEnterHandler, target);
        }

        public void PassPointerEventExit(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.pointerExitHandler, target);
        }

        public void PassPointerEventBeginDrag(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.beginDragHandler, target);
        }

        public void PassPointerEventDrag(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.dragHandler, target);
        }

        public void PassPointerEventEndDrag(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.endDragHandler, target);
        }

        public void PassPointerEventDrop(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.dropHandler, target);
        }

        public void PassPointerEventScroll(PointerEventData eventData, GameObject target = null)
        {
            PassPointerEvent(eventData, ExecuteEvents.scrollHandler, target);
        }
    }
}