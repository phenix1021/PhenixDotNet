using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    //[TaskIcon("TaskIcons/CloseBT.png")]
    public class CloseBT : Action<CloseBTImpl> { }

    [System.Serializable]
    public class CloseBTImpl : ActionImpl
    {
        public override TaskStatus Run()
        {
            Transform.GetComponent<BehaviorTreeAgent>().enabled = false;
            return TaskStatus.SUCCESS;
        }
    }
}