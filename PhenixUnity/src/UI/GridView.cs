using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Phenix.Unity.UI
{
    [ExecuteInEditMode]
    [AddComponentMenu("Phenix/UI/GridView")]    
    public class GridView : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> _cells = new List<GameObject>();

        public RectTransform content;

        [SerializeField]
        bool _destroyOnRemove = true;

        GridLayoutGroup _layout;

        private void Start()
        {
            _layout = GetComponentInChildren<GridLayoutGroup>();
        }

        public void InitCellsOnInspector()
        {
            content.DetachChildren();            
            for (int i = 0; i < _cells.Count; ++i)
            {
                GameObject cur = _cells[i];
                if (cur != null)
                {
                    cur.transform.parent = content;
                }                
            }
            RefreshContentHeight();
        }

        public GameObject GetCell(int idx)
        {
            if (idx >= 0 && idx < _cells.Count)
            {
                return _cells[idx];
            }
            return null;
        }

        public GameObject GetCell(string name)
        {
            foreach (var item in _cells)
            {
                if (item.name == name)
                {
                    return item;
                }
            }
            return null;
        }

        public void Add(GameObject cell)
        {
            if (cell == null)
            {
                return;
            }

            _cells.Add(cell);
            cell.transform.parent = content;
            RefreshContentHeight();
        }

        public void Remove(GameObject cell)
        {
            if (cell == null)
            {
                return;
            }
            _cells.Remove(cell);
            cell.transform.parent = null;
            if (_destroyOnRemove)
            {
                DestroyImmediate(cell);
            }
            RefreshContentHeight();
        }

        void RefreshContentHeight()
        {
            float top = _layout.padding.top;
            float bottom = _layout.padding.bottom;
            float left = _layout.padding.left;
            float right = _layout.padding.right;
            float spaceX = _layout.spacing.x;
            float spaceY = _layout.spacing.y;

            int cellCountPerRow = (int)((content.rect.width - left - right) / (_layout.cellSize.x + spaceX));
            int needRowCount = Mathf.CeilToInt(_cells.Count * 1f / cellCountPerRow);
            content.sizeDelta = new Vector2(content.sizeDelta.x, top + bottom + needRowCount * (_layout.cellSize.y + spaceY));
        }
    }
}
