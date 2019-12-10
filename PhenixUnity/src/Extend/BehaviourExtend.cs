using UnityEngine;

namespace Phenix.Unity.Extend
{
    public static class BehaviourExtend
    {
        public static bool IsActive(this Behaviour behavior)
        {            
            return behavior.enabled && behavior.gameObject.activeInHierarchy;
        }
    }
}