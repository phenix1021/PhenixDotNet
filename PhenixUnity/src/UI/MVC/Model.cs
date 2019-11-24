using System.Collections.Generic;
using Phenix.Unity.Message;

namespace Phenix.Unity.UI
{
    public abstract class Model
    {
        // ----------------------- 实例部分 -------------------------
        //public object Data { get; private set; }
        public int UIID { get; private set; }
        public MessageMgr Controllers { get; set; }

        protected Model(/*object data, */int uiID)
        {
            //Data = data;
            UIID = uiID;
            Add(uiID, this);
        }

        public bool IsViewVisible()
        {
            return View.Get(UIID).Visible;
        }

        /// <summary>
        /// 在此进行数据初始化
        /// </summary>
        public virtual void Init() { }

        /// <summary>
        /// 可在此校验数据变化，通知controller
        /// </summary>
        public virtual void OnUpdate()
        {

        }

        // ----------------------- 管理部分 ------------------------
        static Dictionary<int, Model> _models = new Dictionary<int, Model>();

        public static void Add(int uiID, Model model)
        {
            _models.Add(uiID, model);
        }

        public static void Remove(int uiID)
        {
            _models.Remove(uiID);
        }

        public static Model Get(int uiID)
        {
            if (_models.ContainsKey(uiID))
            {
                return _models[uiID];
            }

            return null;
        }

        public static void UpdateAll()
        {
            foreach (var item in _models)
            {
                if (item.Value.IsViewVisible())
                {
                    item.Value.OnUpdate();
                }
            }
        }
    }
}
