using System.Collections.Generic;

namespace Phenix.Unity.Message
{
    public class MessageMgr    
    {
        Queue<Message> _messages = new Queue<Message>();
        Dictionary<int, MessageHandler> _handlers = new Dictionary<int, MessageHandler>();

        public void SendMsg(Message msg, bool queued = false)
        {
            if (queued)
            {
                _messages.Enqueue(msg);
            }
            else
            {
                HandleMessage(msg);
            }
        }

        public void RegisterHandler(int id, MessageHandler handler)
        {
            _handlers.Add(id, handler);
        }

        public void HandleMessages()
        {
            while (_messages.Count > 0)
            {
                HandleMessage(_messages.Dequeue());                
            }
        }

        void HandleMessage(Message msg)
        {
            if (msg != null && _handlers.ContainsKey(msg.msgID))
            {
                _handlers[msg.msgID].OnMessage(msg);
            }
            msg.Release();
        }
    }
}
