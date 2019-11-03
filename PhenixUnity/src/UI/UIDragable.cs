﻿using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Phenix.Unity.UI
{    
    [AddComponentMenu("Phenix/UI/UIDragable")]
    public class UIDragable : Dragable
    {
        public RectTransform canvasTransform;

        void ChangeToAnchorPos(ref PointerEventData eventData)
        {
            Vector2 pointerAnchoredPos = Vector2.zero;
            bool ret = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, eventData.position, eventData.pressEventCamera, out pointerAnchoredPos);
            if (ret)
            {
                eventData.position = pointerAnchoredPos;
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            ChangeToAnchorPos(ref eventData);
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            ChangeToAnchorPos(ref eventData);
            base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            ChangeToAnchorPos(ref eventData);
            base.OnEndDrag(eventData);
        }                
    }
}
