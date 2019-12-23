using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Phenix.Unity.Collection;

namespace Phenix.Unity.UI
{ 
    [CreateAssetMenu(fileName = "NumFontAsset", menuName = "Phenix/UI/NumFont")]
    public class NumFontAsset : ScriptableObject
    {
        public UnityEngine.Sprite[] numSprites = new UnityEngine.Sprite[10];
    }

    /// <summary>
    /// 数字图形字体
    /// </summary>
    [AddComponentMenu("Phenix/UI/NumFont")]
    public class NumFont : MonoBehaviour
    {
        static Pool<GameObject> _pool = new Pool<GameObject>(10);

        [SerializeField]
        ulong _val = 0;        

        public ulong Value
        {
            get
            {
                return _val;
            }

            set
            {
                if (CheckVal() == false)
                {
                    return;
                }

                if (_val == value)
                {
                    return;
                }

                SetValue(value);
            }
        }
                
        [SerializeField]
        NumFontAsset _numFontAsset;

        public int fixedNumLength = 0;      // 固定数据长度（0表示不固定）
        public bool paddingByZero = false;  // 当数据位数长度小于固定数据长度时，是否用0填充空余数位

        bool CheckVal()
        {
            if (fixedNumLength == 0)
            {
                return true;
            }

            if (fixedNumLength < 0)
            {
                return false;
            }

            return _val < Mathf.Pow(10, fixedNumLength);
        }

        private void Start()
        {
            if (CheckVal() == false)
            {
                return;
            }
            foreach (Transform numObj in transform)
            {
                DestroyImmediate(numObj.gameObject);
            }            
            CreateImage(_val);
        }

        public void RefreshOnInspector()
        {
            if (isActiveAndEnabled == false)
            {
                return;
            }

            if (CheckVal() == false)
            {
                return;
            }

            SetValue(_val);
        }

        void SetValue(ulong val)
        {
            _val = val;
            foreach (Transform numObj in transform)
            {
                if (numObj == null)
                {
                    continue;
                }                
                numObj.gameObject.SetActive(false);
                _pool.Collect(numObj.gameObject);// 池回收
            }
            transform.DetachChildren();
            CreateImage(_val);            
        }

        void AddNumObj(UnityEngine.Sprite sprite)
        {
            GameObject go = _pool.Get();// 池分配
            go.layer = gameObject.layer;
            go.name = "Num";
            //go.hideFlags = HideFlags.HideAndDontSave;
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
            go.transform.localScale = Vector3.one;            
            Image img = go.GetComponent<Image>();
            if (img == null)
            {
                img = go.AddComponent<Image>();
            }
            img.sprite = sprite;
            if (sprite == null)
            {
                go.transform.SetAsFirstSibling();
                // 不足数位填0
                img.sprite = _numFontAsset.numSprites[0];
                if (paddingByZero == false)
                {
                    // 如果不用0填充，则透明处理
                    img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
                }
            }
            else
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
            }
            go.SetActive(true);            
        }

        void CreateImage(ulong val, bool first = true/*递归首轮*/)
        {
            ulong num = val % 10;
            if (val >= 10)
            {
                CreateImage(val / 10, false);
            }

            UnityEngine.Sprite sprite = _numFontAsset.numSprites[num];
            if (sprite)
            {
                AddNumObj(sprite);
            }

            if (first)
            {
                if (transform.childCount < fixedNumLength)
                {
                    int paddingCount = fixedNumLength - transform.childCount;
                    for (int i = 0; i < paddingCount; i++)
                    {
                        AddNumObj(null);
                    }
                }
            }
        }
    }
}