using UnityEngine;
using Phenix.Unity.Message;

namespace Phenix.Unity.UI
{
    public class UIFacade
    {        
        Canvas _canvas;
        MessageMgr _controllers = new MessageMgr();

        public UIFacade(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void RegisterUI(GameObject uiRootPrefab, Model model, View view)
        {
            if (uiRootPrefab == null)
            {
                return;
            }

            GameObject inst = GameObject.Instantiate(uiRootPrefab) as GameObject;
            //foreach (Transform child in inst.transform)
            //{
            //    if (child.GetComponent<RectTransform>() != null)
            //    {
            //        // 为每个子UI对象添加可交互组件
            //        Interactable comp = inst.AddComponent<Interactable>();
            //    }
            //}            
            inst.transform.parent = _canvas.transform;
            (inst.transform as RectTransform).offsetMin = (inst.transform as RectTransform).offsetMax = Vector2.zero;
            (inst.transform as RectTransform).localScale = Vector3.one;

            view.UIRoot = inst;            
            view.Controllers = _controllers;
            view.Init();
            
            model.Controllers = _controllers;

            inst.SetActive(false);
        }

        public void RegisterMessage(int msgID, MessageHandler handler)
        {
            _controllers.RegisterHandler(msgID, handler);
        }

        public void SendMsg(Phenix.Unity.Message.Message msg)
        {
            _controllers.SendMsg(msg);
        }

        public void OnUpdate()
        {
            _controllers.HandleMessages();
            Model.UpdateAll();
        }      

        public void StartUp(int uiID)
        {
            View view = View.Get(uiID);
            if (view == null)
            {
                return;
            }

            view.Open();
        }
    }
}