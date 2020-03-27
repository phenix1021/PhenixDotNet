using UnityEngine;

namespace Phenix.Unity.Anim.SMB
{
    public abstract class SMBBase : StateMachineBehaviour
    {

    }

    public abstract class SMBBase<T> : SMBBase where T : SMBDataBase
    {
        protected T data;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            data = animator.transform.GetComponentInParent<T>();
        }
    }
}