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
        public MessageMgr Controllers { get; set; }
        public bool Visible { get; set; }

        protected View(int uiID)
        {            
            UIID = uiID;
            Add(uiID, this);
        }

        /// <summary>
        /// 在此绑定各UI事件的处理回调函数
        /// </summary>
        public virtual void Init() { }

        public virtual void Open()
        {
            Visible = true;
        }

        public virtual void Close()
        {
            Visible = false;
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
    }
}
