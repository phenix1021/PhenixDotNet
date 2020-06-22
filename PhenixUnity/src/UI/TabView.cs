using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Phenix.Unity.UI
{
    [System.Serializable]
    public class UnityEventTabChanged : UnityEvent<int, string> { }

    [ExecuteInEditMode]
    [AddComponentMenu("Phenix/UI/TabView")]
    [RequireComponent(typeof(ToggleGroup))]
    public class TabView : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> _tabs = new List<GameObject>();

        public RectTransform contents;
        ToggleGroup _toggleGroup;

        public UnityEventTabChanged onTabChanged;

        int _curTabIdx = 0;

        [SerializeField]
        bool _destroyOnRemove = true;

        public GameObject CurTab { get { return _tabs.Count > 0 ? _tabs[_curTabIdx] : null; } }

        public void InitTabsOnInspetor()
        {
            if (isActiveAndEnabled == false)
            {
                return;
            }

            contents.DetachChildren();
            GameObject pre = null;
            for (int i = 0; i < _tabs.Count; ++i)
            {
                GameObject cur = _tabs[i];
                if (cur != null && cur != pre)
                {
                    cur.transform.SetParent(contents);
                    pre = cur;                    
                    continue;
                }

                pre = _tabs[i] = CreateTabOnInspector();
            }
        }

        GameObject CreateTabOnInspector()
        {
            GameObject tabOff = new GameObject("New Tab");
            Toggle toggle = tabOff.AddComponent<Toggle>();
            Image targetGraphic = tabOff.AddComponent<Image>();
            GameObject tabOn = new GameObject("SwitchOn");
            Image graphic = tabOn.AddComponent<Image>();            
            tabOn.transform.SetParent(tabOff.transform);

            toggle.targetGraphic = targetGraphic;
            toggle.graphic = graphic;
            toggle.group = _toggleGroup;
            toggle.onValueChanged.AddListener(OnValueChanged);

            tabOff.transform.SetParent(contents);

            (tabOn.transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);
            (tabOn.transform as RectTransform).anchorMin = Vector2.zero;
            (tabOn.transform as RectTransform).anchorMax = Vector2.one;
            (tabOn.transform as RectTransform).offsetMin = Vector2.zero;
            (tabOn.transform as RectTransform).offsetMax = Vector2.one;            

            return tabOff;
        }

        public void AddTab(string name, UnityEngine.Sprite switchOff, UnityEngine.Sprite switchOn)
        {
            GameObject tabOff = new GameObject(name);
            Image targetGraphic = tabOff.AddComponent<Image>();
            targetGraphic.sprite = switchOff;
            Toggle toggle = tabOff.AddComponent<Toggle>();            

            GameObject tabOn = new GameObject("SwitchOn");
            Image graphic = tabOn.AddComponent<Image>();
            graphic.sprite = switchOn;
            tabOn.transform.SetParent(tabOff.transform);

            toggle.targetGraphic = targetGraphic;
            toggle.graphic = graphic;
            toggle.group = _toggleGroup;
            toggle.onValueChanged.AddListener(OnValueChanged);

            tabOff.transform.SetParent(contents);

            (tabOff.transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);
            (tabOff.transform as RectTransform).anchorMin = Vector2.zero;
            (tabOff.transform as RectTransform).anchorMax = Vector2.one;
            (tabOff.transform as RectTransform).offsetMin = Vector2.zero;
            (tabOff.transform as RectTransform).offsetMax = Vector2.one;
        }

        public GameObject GetTab(string name)
        {
            foreach (var tab in _tabs)
            {
                if (tab.name == name)
                {                    
                    return tab;
                }
            }
            return null;
        }

        public void RemoveTab(GameObject tab)
        {
            _tabs.Remove(tab);
            tab.transform.SetParent(null);
            if (_destroyOnRemove)
            {
                DestroyImmediate(tab);
            }            
            RefreshTabs();
        }

        void OnValueChanged(bool on)
        {
            if (on)
            {
                RefreshTabs();
            }

            if (_curTabIdx >= 0)
            {
                OnTabChanged(_curTabIdx, _tabs[_curTabIdx].name);
            }
        }

        void OnTabChanged(int idx, string name)
        {
            if (onTabChanged != null)
            {
                onTabChanged.Invoke(idx, name);
            }            
        }

        // Use this for initialization
        void Start()
        {
            _toggleGroup = GetComponent<ToggleGroup>();
            RefreshTabs();
        }

        public void RefreshTabs()
        {
            _curTabIdx = -1;
            for (int i = 0; i < _tabs.Count; ++i)
            {
                GameObject tab = _tabs[i];
                if (tab == null)
                {
                    continue;
                }

                Toggle toggle = tab.GetComponent<Toggle>();
                if (toggle.isOn)
                {
                    _curTabIdx = i;
                    break;
                }
            }
        }
    }
}
