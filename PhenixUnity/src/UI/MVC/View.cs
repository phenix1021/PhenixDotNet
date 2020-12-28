using System.Collections.Generic;
using UnityEngine;
using Phenix.Unity.Message;

namespace Phenix.Unity.UI
{
    public abstract class View
    {
        // ---------------- 实例部分 ---------------------
        public GameObject UIRoot { get; set; }
        public int UIID { get; private set; }
        //public MessageMgr Controllers { get; set; }
        public bool Active { get; set; }
        protected Canvas Canvas { get; private set; }

        float _closeTimer = 0;

        protected View(int uiID)
        {            
            UIID = uiID;
            Add(uiID, this);
        }

        /// <summary>
        /// 在此绑定各UI事件的处理回调函数
        /// </summary>
        public virtual void Init()
        {
            Canvas = UIRoot.GetComponentInParent<Canvas>();
        }

        public virtual void Open(bool asLastSibling = false)
        {
            Active = true;
            _closeTimer = 0;
            if (asLastSibling)
            {
                UIRoot.transform.SetAsLastSibling();
            }
        }

        public virtual void Close()
        {
            Active = false;
            _closeTimer = Time.timeSinceLevelLoad;
        }

        /// <summary>
        /// 获取已关闭时长（可以据此删除view里动态加载的内容）
        /// </summary>        
        protected float GetEllapseTimeSinceClose()
        {
            if (_closeTimer == 0)
            {
                return 0;
            }
            return Time.timeSinceLevelLoad - _closeTimer;
        }

        public virtual void OnUpdate()
        {
         
        }

        // --------------- 管理部分 -------------------
        static Dictionary<int, View> _views = new Dictionary<int, View>();

        public static void Add(int uiID, View view)
        {
            _views.Add(uiID, view);
        }

        public static void Remove(int uiID)
        {
            _views.Remove(uiID);
        }

        public static View Get(int uiID)
        {
            if (_views.ContainsKey(uiID))
            {
                return _views[uiID];
            }

            return null;
        }

        public static void UpdateAll()
        {
            foreach (var item in _views)
            {
                if (item.Value.Active)
                {
                    item.Value.OnUpdate();
                }
            }
        }
    }
}
