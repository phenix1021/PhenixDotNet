using UnityEngine;

namespace Phenix.Unity.AI.BT
{
    //[TaskIcon("TaskIcons/Switch.png")]
    public class Switch : Conditional<SwitchImpl> { }

    [System.Serializable]
    public class SwitchImpl : ConditionalImpl
    {
        public bool ret = false;
        
        public override bool Check()
        {   
            return ret;
        }
    }
}