namespace Phenix.Unity.Message
{
    public abstract class Message
    {
        public int msgID;                

        public virtual void Release() { }
    }

    public interface IMessageHandler
    {
        void OnMessage(Message msg);
    }

    public abstract class MessageHandler : IMessageHandler
    {
        public virtual void OnMessage(Message msg) { }
    }
}
