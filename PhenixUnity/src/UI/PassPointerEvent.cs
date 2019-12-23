﻿using UnityEngine;
using UnityEngine.EventSystems;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.UI
{
    /// <summary>
    /// 透传pointer消息
    /// </summary>    
    [AddComponentMenu("Phenix/UI/PassPointerEvent")]
    public class PassPointerEvent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerUpHandler, 
        IPointerDownHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IScrollHandler
    {
        public bool passPointerEnter = false;
        public bool passPointerExit = false;
        public bool passPointerDown = false;
        public bool passPointerUp = false;
        public bool passPointerClick = false;        
        public bool passBeginDrag = false;
        public bool passDrag = false;
        public bool passEndDrag = false;
        public bool passDrop = false;
        public bool passScroll = false;

        public GameObject target = null;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (passBeginDrag)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.beginDragHandler, target);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {           
            if (passDrag)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.dragHandler, target);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (passDrop)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.dropHandler, target);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (passEndDrag)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.endDragHandler, target);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (passPointerClick)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.pointerClickHandler, target);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (passPointerDown)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.pointerDownHandler, target);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (passPointerEnter)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.pointerEnterHandler, target);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (passPointerExit)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.pointerExitHandler, target);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (passPointerUp)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.pointerUpHandler, target);
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (passScroll)
            {
                UITools.Instance.PassPointerEvent(eventData, ExecuteEvents.scrollHandler, target);
            }
        }
    }
}
