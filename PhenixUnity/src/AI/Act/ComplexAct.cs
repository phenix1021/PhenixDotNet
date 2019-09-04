using UnityEngine;

namespace Phenix.Unity.AI
{
    public abstract class ComplexActEvent
    {
        public int eventCode;
    }

    public interface IComplexAct
    {
        void OnBegin(ComplexActEvent actEvent = null);
        void OnUpdate();
        void OnEnd();
        bool OnEvent(ComplexActEvent actEvent);
    }

    public abstract class ComplexAct : IComplexAct
    {
        public int ResultCode { get; set; }
        public virtual void OnBegin(ComplexActEvent actEvent = null) { }        
        public virtual void OnEnd() { }        
        public virtual bool OnEvent(ComplexActEvent actEvent) { return false; }
        public virtual void OnUpdate() { }
    }
}