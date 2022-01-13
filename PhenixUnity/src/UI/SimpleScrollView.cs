using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Phenix.Unity.UI
{
    [System.Serializable]
    public class UnityEventCellSelected : UnityEvent<GameObject/*原选中对象*/, GameObject/*新选中对象*/> { }

    /// <summary>
    /// 适用于小量数据全部展示 或 海量数据分页展示
    /// </summary>
    [AddComponentMenu("Phenix/UI/SimpleScrollView")]
    [ExecuteInEditMode]
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
        
        [SerializeField]
        Vector2 _cellSize = new Vector2(100, 100);

        public ScrollRect scrollRect;
        HorizontalOrVerticalLayoutGroup _layout;

        List<float> _normalizedPosInScroll = new List<float>(); // 各个格子中心经过_viewPort中心时的scrollRect.horizontalNormalizedPosition值列表

        [SerializeField]
        GameObject _cellSelected = null;
        public UnityEventCellSelected onCellSelected;
        public GameObject CellSelected
        {
            get { return _cellSelected; }
            set
            {
                if (value != _cellSelected)
                {
                    if (onCellSelected != null)
                    {
                        GameObject preSelected = _cellSelected;
                        _cellSelected = value;
                        onCellSelected.Invoke(preSelected, value);
                    }                                    
                    else
                    {
                        _cellSelected = value;
                    }
                }
            }
        }

        public int ContentChildrenCount { get { return content.childCount; } }

        private void Awake()
        {
            if (scrollDirection == ScrollDirection.VERTICAL)
            {
                content.anchorMin = new Vector2(0, 1);
                content.anchorMax = new Vector2(1, 1);
                _layout = content.GetComponent<VerticalLayoutGroup>();
            }
            else
            {
                content.anchorMin = new Vector2(0, 0);
                content.anchorMax = new Vector2(0, 1);
                _layout = content.GetComponent<HorizontalLayoutGroup>();
            }

            _layout.childAlignment = TextAnchor.MiddleCenter;
            _layout.childForceExpandHeight = _layout.childForceExpandWidth = true;
        }

        // Use this for initialization
        void Start()
        {
            content.DetachChildren();            

            foreach (var item in _cells)
            {
                if (item == null)
                {
                    continue;
                }
                item.GetComponent<RectTransform>().sizeDelta = _cellSize;
                item.transform.SetParent(content);
            }

            Refresh();

            //scrollRect.onValueChanged.AddListener(OnValueChanged);
        }

        /*void OnValueChanged(Vector2 arg)
        {
            Debug.Log(arg);
        }*/

        public GameObject GetCell(int idx)
        {
            if (idx < 0 || idx >= _cells.Count)
            {
                return null;
            }

            return _cells[idx];
        }

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
            if (cell == null)
            {
                return;
            }
            _cells.Add(cell);
            
            RectTransform rt = cell.GetComponent<RectTransform>();
            rt.sizeDelta = _cellSize;            
            cell.transform.SetParent(content);
            cell.transform.localPosition = new Vector3(cell.transform.localPosition.x, cell.transform.localPosition.y, 0);
            cell.transform.localScale = Vector3.one;

            if (cell.activeSelf == false)
            {
                cell.SetActive(true);
            }

            if (refresh)
            {
                Refresh();
            }
        }

        public void AddFront(GameObject cell, bool refresh = true)
        {
            if (cell == null)
            {
                return;
            }
            _cells.Insert(0, cell);

            RectTransform rt = cell.GetComponent<RectTransform>();
            rt.sizeDelta = _cellSize;
            cell.transform.SetParent(content);
            cell.transform.SetAsFirstSibling();
            cell.transform.localPosition = new Vector3(cell.transform.localPosition.x, cell.transform.localPosition.y, 0);
            cell.transform.localScale = Vector3.one;

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
            cell.transform.SetParent(null);
            //cell.SetActive(false);

            DestroyImmediate(cell);

            if (CellSelected == cell)
            {
                CellSelected = null;
            }

            Refresh();            
        }

        public void Clear()
        {
            if (_cells.Count == 0)
            {
                return;
            }

            content.DetachChildren();

            while (_cells.Count > 0)
            {
                GameObject go = _cells[0];
                _cells.Remove(go);
                DestroyImmediate(go);
            }

            Refresh();
        }

        void Refresh()
        {
            int count = Mathf.Max(_cells.Count, 1);

            float totalSpace = 0;
            if (scrollDirection == ScrollDirection.VERTICAL)
            {
                totalSpace = _layout.spacing * (count - 1) + _layout.padding.top + _layout.padding.bottom;
                content.sizeDelta = new Vector2(content.sizeDelta.x, _cellSize.y * count + totalSpace);
            }
            else
            {
                totalSpace = _layout.spacing * (count - 1) + _layout.padding.left + _layout.padding.right;
                content.sizeDelta = new Vector2(_cellSize.x * count + totalSpace, content.sizeDelta.y);
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
                    float pos = _layout.padding.top + _cellSize.y * 0.5f + (_cellSize.y + _layout.spacing) * i;
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
                    float pos = _layout.padding.left + _cellSize.x * 0.5f + (_cellSize.x + _layout.spacing) * i;
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

        public void InitCellsOnInspector()
        {
            if (isActiveAndEnabled == false)
            {
                return;
            }

            content.DetachChildren();
            for (int i = 0; i < _cells.Count; ++i)
            {
                Add(_cells[i], false);
                /*GameObject cur = _cells[i];
                if (cur != null)
                {
                    cur.GetComponent<RectTransform>().sizeDelta = _cellSize;
                    cur.transform.parent = content;
                }*/
            }            

            Refresh();
        }
    }
}