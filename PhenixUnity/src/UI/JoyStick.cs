using UnityEngine;
using UnityEngine.UI;

namespace Phenix.Unity.UI
{
    /// <summary>
    /// 摇杆
    /// </summary>
    [AddComponentMenu("Phenix/UI/JoyStick")]
    public class JoyStick : MonoBehaviour
    {
        public UIDragable dragable;
        
        public RectTransform panel;
        public RectTransform slider;        

        float _moveRadius = 0;    // 摇杆移动半径
        Vector2 _offset;      // 拖动时slider和pointer的偏移值

        static float _horizontalValue = 0;
        static float _horizontalRawValue = 0;
        static float _verticalValue = 0;
        static float _verticalRawValue = 0;

        public static float GetAxisH() { return _horizontalValue; }
        public static float GetRawAxisH() { return _horizontalRawValue; }
        public static float GetAxisV() { return _verticalValue; }
        public static float GetRawAxisV() { return _verticalRawValue; }

        // Use this for initialization
        void Awake()
        {
            dragable.onDragBegin.AddListener(OnBeginDrag);
            dragable.onDragging.AddListener(OnDragging);
            dragable.onDragEnd.AddListener(OnEndDrag);

            slider.SetParent(panel);
            panel.pivot = slider.pivot = new Vector2(0.5f, 0.5f);
            _moveRadius = Mathf.Min(panel.rect.width, panel.rect.height) * 0.5f;

            Reset();
        }

        void OnBeginDrag(Vector2 pointerPos)
        {
            _offset = slider.anchoredPosition - pointerPos;
        }

        void OnDragging(GameObject pointerEnter, Vector2 pointerPos)
        {
            slider.anchoredPosition = Vector2.ClampMagnitude(pointerPos + _offset, _moveRadius);

            _horizontalValue = Mathf.Clamp(slider.anchoredPosition.x / _moveRadius, -1, 1);
            _horizontalRawValue = slider.anchoredPosition.x > 0 ? 1 : (slider.anchoredPosition.x == 0 ? 0 : -1);
            _verticalValue = Mathf.Clamp(slider.anchoredPosition.y / _moveRadius, -1, 1);
            _verticalRawValue = slider.anchoredPosition.y > 0 ? 1 : (slider.anchoredPosition.y == 0 ? 0 : -1);
        }

        void OnEndDrag(GameObject pointerEnter, Vector2 pointerPos)
        {
            Reset();
        }

        public void Reset()
        {
            _horizontalValue = _horizontalRawValue = _verticalValue = _verticalRawValue = 0;
            slider.anchoredPosition = Vector2.zero;
        }
    }
}