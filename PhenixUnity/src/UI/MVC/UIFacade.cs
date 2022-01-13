using UnityEngine;
using Phenix.Unity.Message;
using UnityEngine.UI;

namespace Phenix.Unity.UI
{
    public class UIFacade
    {        
        Canvas _canvas;
        MessageMgr _msgMgr = new MessageMgr();

        public UIFacade(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void RegisterViewUI(GameObject viewUIPrefab, Model model, View view, bool subCanvas = false)
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
            float x = inst.transform.position.x;
            float y = inst.transform.position.y;
            inst.transform.SetParent(_canvas.transform);
            //inst.transform.localPosition = new Vector3(inst.transform.localPosition.x, inst.transform.localPosition.y, 0);
            inst.transform.localPosition = new Vector3(x, y, 0);
            //(inst.transform as RectTransform).offsetMin = (inst.transform as RectTransform).offsetMax = Vector2.zero;            
            (inst.transform as RectTransform).localScale = Vector3.one;
            if ((inst.transform as RectTransform).anchorMin == Vector2.zero && (inst.transform as RectTransform).anchorMax == Vector2.one)
            {
                (inst.transform as RectTransform).sizeDelta = Vector2.zero;
            }

            view.UIRoot = inst;
            //view.Controllers = _msgMgr;
            
            if (subCanvas)
            {
                view.UIRoot.AddComponent<Canvas>();
                view.UIRoot.AddComponent<GraphicRaycaster>();
            }

            view.Init();

            if (model != null)
            {
                //model.Controllers = _msgMgr;
                model.Init();
            }            

            inst.SetActive(false);
        }

        public void RegisterMessage(int msgID, MessageHandler handler)
        {
            _msgMgr.RegisterHandler(msgID, handler);
        }

        public void SendMsg(Phenix.Unity.Message.Message msg)
        {
            _msgMgr.SendMsg(msg);
        }

        public void OnUpdate()
        {
            _msgMgr.HandleMessages();
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