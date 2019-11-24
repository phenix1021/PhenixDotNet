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

        public void RegisterViewUI(GameObject viewUIPrefab, Model model, View view)
        {
            if (viewUIPrefab == null)
            {
                return;
            }

            GameObject inst = GameObject.Instantiate(viewUIPrefab) as GameObject;
            //foreach (Transform child in inst.transform)
            //{
            //    if (child.GetComponent<RectTransform>() != null)
            //    {
            //        // 为每个子UI对象添加可交互组件
            //        Interactable comp = inst.AddComponent<Interactable>();
            //    }
            //}                                   
            inst.transform.SetParent(_canvas.transform);
            inst.transform.localPosition = new Vector3(inst.transform.localPosition.x, inst.transform.localPosition.y, 0);
            //(inst.transform as RectTransform).offsetMin = (inst.transform as RectTransform).offsetMax = Vector2.zero;
            (inst.transform as RectTransform).localScale = Vector3.one;            

            view.UIRoot = inst;            
            view.Controllers = _controllers;
            view.Init();

            if (model != null)
            {
                model.Controllers = _controllers;
                model.Init();
            }            

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
            View.UpdateAll();
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