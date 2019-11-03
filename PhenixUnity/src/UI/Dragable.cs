using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Phenix.Unity.UI
{
    /// <summary>
    /// 可拖动组件
    /// </summary>
    /// <mark>
    /// 1. 对于UI对象没有其它额外要求。
    /// 2. 对于一般场景对象，要求所挂载对象或其子对象必须有Collider组件，
    ///    并且主摄像机有Physics Raycaster组件，且场景对象的layer须符合
    ///    摄像机的culling mask
    /// </mark>>
    [AddComponentMenu("Phenix/UI/Dragable")]
    public class Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [System.Serializable]
        public class BeginDragEvent : UnityEvent<Vector2/*pointer的坐标*/> { }

        [System.Serializable]
        public class DraggingEvent : UnityEvent<GameObject/*pointer进入的对象*/, Vector2/*pointer的坐标*/> {}

        [System.Serializable]
        public class EndDragEvent : UnityEvent<GameObject/*pointer进入的对象*/, Vector2/*pointer的坐标*/> { }        

        public BeginDragEvent onDragBegin;             // 开始拖动
        public DraggingEvent onDragging;               // 拖动中
        public EndDragEvent onDragEnd;                 // 拖动完成        

        public virtual void OnBeginDrag(PointerEventData eventData)
        {        
            if (onDragBegin != null)
            {
                onDragBegin.Invoke(eventData.position);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (onDragging != null)
            {
                onDragging.Invoke(eventData.pointerEnter, eventData.position);
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (onDragEnd != null)
            {
                onDragEnd.Invoke(eventData.pointerEnter, eventData.position);
            }            
        }        
    }
}
