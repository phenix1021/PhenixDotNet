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

        List<GameObject> _images = new List<GameObject>();

        public ulong Value { get { return _val; } set { SetValue(value); } }
                
        [SerializeField]
        NumFontAsset _numFontAsset;

        void SetValue(ulong val)
        {
            _val = val;
            foreach (var img in _images)
            {
                img.transform.parent = null;
                img.SetActive(false);
                _pool.Collect(img);// 池回收

            }
            _images.Clear();
            CreateImage(_val);
        }

        void CreateImage(ulong val)
        {
            ulong num = val % 10;
            if (val >= 10)
            {
                CreateImage(val / 10);
            }

            UnityEngine.Sprite sprite = _numFontAsset.numSprites[num];
            if (sprite)
            {
                GameObject go = _pool.Get();// 池分配
                Image img = go.GetComponent<Image>();
                if (img == null)
                {
                    img = go.AddComponent<Image>();
                }                
                img.sprite = sprite;                
                go.hideFlags = HideFlags.HideAndDontSave;
                go.transform.parent = transform;
                go.SetActive(true);
                _images.Add(go);
            }
        }
    }
}