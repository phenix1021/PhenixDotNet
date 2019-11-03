using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Phenix.Unity.UI
{
    [System.Serializable]
    public class UnityEventOnQueryData : UnityEvent<int, GameObject> { }

    /// <summary>
    /// 适用于小量数据全部展示 或 海量数据分页展示
    /// </summary>
    public class SimpleScrollView : MonoBehaviour
    {
        public enum ScrollDirection
        {
            HORIZONTAL = 0,
            VERTICAL,
        }
        
        public ScrollDirection scrollDirection;

        [SerializeField]
        List<GameObject> _cells = new List<GameObject>();
                
        public RectTransform viewPort;
                
        public RectTransform content;

        float _cellSize;          // 内容格子高度值

        public ScrollRect scrollRect;
        HorizontalOrVerticalLayoutGroup _layout;

        List<float> _normalizedPosInScroll = new List<float>(); // 各个格子中心经过_viewPort中心时的scrollRect.horizontalNormalizedPosition值列表

        // Use this for initialization
        void Start()
        {
            if (scrollDirection == ScrollDirection.VERTICAL)
            {
                content.anchorMin = new Vector2(0, 1);
                content.anchorMax = new Vector2(1, 1);
                _layout = content.GetComponent<VerticalLayoutGroup>();
                _cellSize = content.rect.height; // 内容每个格子的宽、高由初始_content的尺寸决定
            }
            else
            {
                content.anchorMin = new Vector2(0, 0);
                content.anchorMax = new Vector2(0, 1);
                _layout = content.GetComponent<HorizontalLayoutGroup>();
                _cellSize = content.rect.width; // 内容每个格子的宽、高由初始_content的尺寸决定
            }

            _layout.childAlignment = TextAnchor.MiddleCenter;
            _layout.childControlHeight = _layout.childControlWidth = true;
            _layout.childForceExpandHeight = _layout.childForceExpandWidth = true;

            content.DetachChildren();            

            foreach (var item in _cells)
            {
                item.transform.parent = content;
            }

            Refresh();

            //scrollRect.onValueChanged.AddListener(OnValueChanged);
        }

        /*void OnValueChanged(Vector2 arg)
        {
            Debug.Log(arg);
        }*/

        public void Add(List<GameObject> cells)
        {
            foreach (var cell in cells)
            {
                Add(cell, false);
            }
            Refresh();
        }

        public void Add(GameObject cell, bool refresh = true)
        {
            _cells.Add(cell);
            cell.transform.parent = content;
            if (cell.activeSelf == false)
            {
                cell.SetActive(true);
            }

            if (refresh)
            {
                Refresh();
            }
        }

        public float GetNormalizedPosition()
        {
            if (scrollDirection == ScrollDirection.VERTICAL)
            {
                return scrollRect.verticalNormalizedPosition;
            }
            else
            {
                return scrollRect.horizontalNormalizedPosition;
            }
        }

        public void Del(GameObject cell)
        {
            _cells.Remove(cell);
            cell.transform.parent = null;
            cell.SetActive(false);
            Refresh();
        }

        public void Clear()
        {
            _cells.Clear();
            content.DetachChildren();
            _normalizedPosInScroll.Clear();
            Refresh();
        }

        void Refresh()
        {
            int count = Mathf.Max(_cells.Count, 1);

            float totalSpace = 0;
            if (scrollDirection == ScrollDirection.VERTICAL)
            {
                totalSpace = _layout.spacing * (count - 1) + _layout.padding.top + _layout.padding.bottom;
                content.sizeDelta = new Vector2(content.sizeDelta.x, _cellSize * count + totalSpace);
            }
            else
            {
                totalSpace = _layout.spacing * (count - 1) + _layout.padding.left + _layout.padding.right;
                content.sizeDelta = new Vector2(_cellSize * count + totalSpace, content.sizeDelta.y);
            }

            RefreshNormalizedPosInScroll();
        }

        void RefreshNormalizedPosInScroll()
        {
            if (_cells.Count == 0)
            {
                return;
            }
            _normalizedPosInScroll.Clear();
            float distance = 0;
            if (scrollDirection == ScrollDirection.VERTICAL)
            {
                distance = content.rect.height - viewPort.rect.height;
                for (int i = 0; i < _cells.Count; i++)
                {
                    float pos = _layout.padding.top + _cellSize * 0.5f + (_cellSize + _layout.spacing) * i;
                    pos -= (viewPort.rect.height * 0.5f);
                    pos = (distance - pos) / distance;
                    _normalizedPosInScroll.Add(pos);
                }
            }
            else
            {
                distance = content.rect.width - viewPort.rect.width;
                for (int i = 0; i < _cells.Count; i++)
                {
                    float pos = _layout.padding.left + _cellSize * 0.5f + (_cellSize + _layout.spacing) * i;
                    pos -= (viewPort.rect.width * 0.5f);
                    pos /= distance;
                    _normalizedPosInScroll.Add(pos);
                }
            }
        }

        public void GoToCell(int cellIdx)
        {
            if (cellIdx < 0 || cellIdx >= _cells.Count)
            {
                return;
            }

            if (scrollDirection == ScrollDirection.VERTICAL)
            {
                scrollRect.verticalNormalizedPosition = _normalizedPosInScroll[cellIdx];
            }
            else
            {
                scrollRect.horizontalNormalizedPosition = _normalizedPosInScroll[cellIdx];
            }
        }

        /*static int idx = 0;
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                GoToCell(idx++);
            }
            if (Input.GetKeyUp(KeyCode.B))
            {
                Debug.Log("cur value " + scrollRect.verticalNormalizedPosition);
            }
        }*/
    }
}