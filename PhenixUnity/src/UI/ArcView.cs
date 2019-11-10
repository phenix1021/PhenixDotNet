using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Phenix.Unity.Extend;

namespace Phenix.Unity.UI
{    
    [ExecuteInEditMode]
    [AddComponentMenu("Phenix/UI/ArcView")]
    public class ArcView : MonoBehaviour
    {        
        public enum FoldDirection
        {
            TO_TOP = 0,
            TO_BOTTOM,
            TO_LEFT,
            TO_RIGHT,
        }

        public RectTransform panel;     // 基座位置
        public RectTransform content;   // 容器
        public UIDragable hotSpot;      // 感应区
        public Button btnSwitch;        // 收起/展开切换按钮（位置不必非等于_center）

        public List<GameObject> cells = new List<GameObject>(); // 所容纳的对象
        List<GameObject> _toAdd = new List<GameObject>();       // 将要添加的对象
        List<GameObject> _toRemove = new List<GameObject>();    // 将要删除的对象

        public FoldDirection foldDir = FoldDirection.TO_LEFT;

        Vector2 _contentInitPos; // 中心位置

        [SerializeField]
        float _axisRadius = 185;  // 轴半径
        [SerializeField]
        float _axisOffsetDegree = -90;  // 轴偏移角度

        [SerializeField]
        float _cellSpaceDegree = 45; // cell间隔角度
        
        float _foldDistance;  // 缩放距离
        float _maxCellSize;   // 最大格子尺寸

        bool _isFolded = false;  // 圆盘是否收起
        bool _rotating = false;     // 圆盘是否正在旋转
        bool _switching = false;    // 圆盘是否正在切换（收起/展开）               

        [SerializeField]
        float _switchSpeed = 1; // 切换速度
        [SerializeField]
        float _rotateSpeed = 1; // 旋转速度

        [SerializeField]
        bool _destroyOnRemove = true;

        Vector2 _pointerPos = Vector2.zero;
        int _cellIdxOffset = 0;        

        // Use this for initialization
        void Start()
        {
            content.pivot = panel.pivot = new Vector2(0.5f, 0.5f);
            _contentInitPos = content.localPosition;
            
            btnSwitch.onClick.AddListener(Switch);
            if (_rotateSpeed == 0)
            {
                _rotateSpeed = 1;
            }
            if (_switchSpeed == 0)
            {
                _switchSpeed = 1;
            }

            if (hotSpot.onDragBegin != null)
            {
                hotSpot.onDragBegin.AddListener(OnBeginDrag);
            }
            if (hotSpot.onDragEnd != null)
            {
                hotSpot.onDragEnd.AddListener(OnEndDrag);
            }            
            hotSpot.rect = hotSpot.gameObject.transform as RectTransform;            

            Reset();            
        }

        public void Reset()
        {
            if (content == null)
            {
                return;
            }

            content.DetachChildren();
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i] == null)
                {
                    continue;
                }
                RectTransform rectTransform = cells[i].transform as RectTransform;
                Vector2 pos = GetCellInitPos(i);
                rectTransform.parent = content;
                rectTransform.localPosition = new Vector3(pos.x, pos.y);
            }
            _maxCellSize = GetMaxCellSize();
        }

        void RefreshCells()
        {
            int offset = 0;
            if (_cellIdxOffset > 0)
            {
                offset = _cellIdxOffset % cells.Count;
            }
            else if (_cellIdxOffset < 0)
            {
                offset = -1 * ((-_cellIdxOffset) % cells.Count);
                offset = cells.Count + offset;
            }

            if (offset > 0)
            {
                List<GameObject> tmp = new List<GameObject>(cells);
                for (int i = 0; i < cells.Count; i++)
                {
                    tmp[(i + offset) % cells.Count] = cells[i];
                }
                cells = tmp;
            }

            _cellIdxOffset = 0;
        }

        public void Add(GameObject cell)
        {
            if (_rotating || _switching)
            {
                _toAdd.Add(cell);
            }
            else
            {
                AddImpl(cell);
            }
        }

        void AddImpl(GameObject cell)
        {
            RefreshCells();

            cells.Add(cell);
            Vector2 pos = GetCellInitPos(cells.Count - 1);
            cell.transform.parent = content;
            cell.transform.localPosition = new Vector3(pos.x, pos.y);
            _maxCellSize = GetMaxCellSize();            
        }

        public void Remove(GameObject cell)
        {
            if (_rotating || _switching)
            {
                _toRemove.Add(cell);
            }
            else
            {
                RemoveImpl(cell);
            }
        }

        void RemoveImpl(GameObject cell)
        {
            RefreshCells();

            bool found = false;
            for (int i = 0; i < cells.Count; i++)
            {
                if (cell != cells[i])
                {
                    continue;
                }

                found = true;
                for (int ii = cells.Count - 1; ii > i; ii--)
                {
                    cells[ii].transform.localPosition = cells[ii - 1].transform.localPosition;
                }
            }

            if (found)
            {
                cell.transform.parent = null;
                cells.Remove(cell);
                if (_destroyOnRemove)
                {
                    DestroyImmediate(cell);
                }                
                _maxCellSize = GetMaxCellSize();
            }            
        }

        // 获得格子初始位置
        Vector2 GetCellInitPos(int idx)
        {
            Vector2 pos = Vector2.zero;
            pos.x = content.localPosition.x + _axisRadius;
            pos.y = content.localPosition.y;
            return pos.Rotate(idx * _cellSpaceDegree + _axisOffsetDegree);                        
        }

        float GetMaxCellSize()
        {
            float max = 0;
            foreach (var item in cells)
            {
                if (item == null)
                {
                    continue;
                }
                RectTransform trans = item.transform as RectTransform;
                float size = Mathf.Max(trans.rect.width, trans.rect.height);
                if (size > max)
                {
                    max = size;
                }
            }
            return max;
        }

        void Switch()
        {
            if (_rotating)
            {
                return;
            }

            _isFolded = !_isFolded;
            if (_isFolded)
            {
                // 收起圆盘
                Fold();
            }
            else
            {
                // 展开圆盘
                Unfold();
            }            
        }

        // 收起
        void Fold()
        {
            hotSpot.gameObject.SetActive(false);
            StartCoroutine("SwitchMoveImpl");
        }

        // 展开
        void Unfold()
        {            
            StartCoroutine("SwitchMoveImpl");
        }

        Vector2 GetSwitchMoveDest()
        {
            if (_isFolded == false)
            {
                return _contentInitPos;
            }

            Vector2 tarPos = Vector2.zero;
            switch (foldDir)
            {
                case FoldDirection.TO_TOP:
                    _foldDistance = _axisRadius - (content.position.y - Screen.height) + _maxCellSize;
                    tarPos.x = _contentInitPos.x;
                    tarPos.y = _contentInitPos.y + _foldDistance;
                    break;
                case FoldDirection.TO_BOTTOM:
                    _foldDistance = _axisRadius + content.position.y + _maxCellSize;
                    tarPos.x = _contentInitPos.x;
                    tarPos.y = _contentInitPos.y - _foldDistance;
                    break;
                case FoldDirection.TO_LEFT:
                    _foldDistance = _axisRadius + content.position.x + _maxCellSize;
                    tarPos.x = _contentInitPos.x - _foldDistance;
                    tarPos.y = _contentInitPos.y;
                    break;
                case FoldDirection.TO_RIGHT:
                    _foldDistance = _axisRadius - (content.position.x - Screen.width) + _maxCellSize;
                    tarPos.x = _contentInitPos.x + _foldDistance;
                    tarPos.y = _contentInitPos.y;
                    break;
                default:
                    break;
            }
            return tarPos;
        }

        IEnumerator SwitchMoveImpl()
        {
            CancelInvoke("SwitchMoveImpl");

            _switching = true;
            Vector2 tarPos = GetSwitchMoveDest();

            float switchTotalTime = 1 / _switchSpeed;
            float elapseTime = 0;

            yield return new WaitForEndOfFrame();

            while ((elapseTime += Time.deltaTime) < switchTotalTime)
            {
                content.transform.localPosition = Vector3.Lerp(content.transform.localPosition, tarPos, Mathf.Min(elapseTime / switchTotalTime, 1));
                yield return new WaitForEndOfFrame();
            }

            _switching = false;

            if (_isFolded == false)
            {
                hotSpot.gameObject.SetActive(true);
            }

            HandleToAddToRemove();
        }

        public void OnBeginDrag(Vector2 pos)
        {
            _pointerPos = pos;
        }

        public void OnEndDrag(GameObject obj, Vector2 pos)
        {
            Rotate(pos);
        }

        //public void OnBeginDrag(PointerEventData eventData)
        //{
        //    _pointerPos = eventData.position;
        //}

        //public void OnEndDrag(PointerEventData eventData)
        //{
        //    Rotate(eventData.position);
        //}

        // 旋转圆盘
        void Rotate(Vector2 pointerPosOnEndDrag)
        {
            if (_switching)
            {
                return;
            }            

            StartCoroutine(RotateMoveImpl(_pointerPos, pointerPosOnEndDrag));
        }

        IEnumerator RotateMoveImpl(Vector2 pointerPosStart, Vector2 pointerPosEnd)
        {
            CancelInvoke("RotateMoveImpl");
            _rotating = true;
            float rotateTotalTime = 1 / _rotateSpeed;
            float elapseTime = 0;            

            bool clockWise = false;
            switch (foldDir)
            {
                case FoldDirection.TO_TOP:
                    clockWise = pointerPosEnd.x < pointerPosStart.x;
                    break;
                case FoldDirection.TO_BOTTOM:
                    clockWise = pointerPosEnd.x > pointerPosStart.x;
                    break;
                case FoldDirection.TO_LEFT:
                    clockWise = pointerPosEnd.y < pointerPosStart.y;
                    break;
                case FoldDirection.TO_RIGHT:
                    clockWise = pointerPosEnd.y > pointerPosStart.y;
                    break;
                default:
                    break;
            }

            if (clockWise)
            {
                --_cellIdxOffset;
            }
            else
            {
                ++_cellIdxOffset;
            }

            List<Vector3> tarPosList = new List<Vector3>();
            Vector3 tarPos = new Vector3();
            for (int i = 0; i < cells.Count; i++)
            {
                if (clockWise)
                {
                    // 从后端向前端移动
                    if (i == 0)
                    {
                        tarPos.x = cells[cells.Count - 1].transform.localPosition.x;
                        tarPos.y = cells[cells.Count - 1].transform.localPosition.y;
                    }
                    else
                    {
                        tarPos.x = cells[i - 1].transform.localPosition.x;
                        tarPos.y = cells[i - 1].transform.localPosition.y;
                    }
                }
                else
                {
                    // 从前端向后端移动
                    tarPos.x = cells[(i + 1) % cells.Count].transform.localPosition.x;
                    tarPos.y = cells[(i + 1) % cells.Count].transform.localPosition.y;                    
                }

                tarPosList.Add(tarPos);
            }                     

            yield return new WaitForEndOfFrame();
            
            while ((elapseTime += Time.deltaTime) < rotateTotalTime)
            {                
                for (int i = 0; i < cells.Count; i++)
                {
                    Transform trans = cells[i].transform;
                    trans.localPosition = Vector3.Slerp(trans.localPosition, tarPosList[i], Mathf.Min(elapseTime / rotateTotalTime, 1));
                }
                yield return new WaitForEndOfFrame();
            }

            _rotating = false;

            HandleToAddToRemove();
        }

        void HandleToAddToRemove()
        {
            foreach (var item in _toAdd)
            {
                AddImpl(item);
            }

            foreach (var item in _toRemove)
            {
                RemoveImpl(item);
            }

            _toAdd.Clear();
            _toRemove.Clear();
        }
    }
}