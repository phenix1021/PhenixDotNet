using UnityEngine;
using System.Collections.Generic;

namespace Phenix.Unity.Effect
{
    /// <summary>
    /// 布娃娃管理
    /// </summary>
    [AddComponentMenu("Phenix/Effect/RagDoll")]
    public class RagDoll : MonoBehaviour
    {        
        [SerializeField]
        Animator _animator;

        [SerializeField]
        Transform _pelvisBone; // 根骨骼        

        [SerializeField]
        float _ragDollTime = 2;         // 持续时间（秒）, 小于等于0表示持续生效

        List<Rigidbody> _ragdollRigidbodys = new List<Rigidbody>(); // 所有骨骼对应的刚体

        //[SerializeField]
        //bool _keepPelVisOffset = true;   // 根骨骼是否保持和host的相对位置

        //Vector3 _pelvisOffset;

        public bool Enabled { get; private set; }

        private void Awake()
        {
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }

            if (_pelvisBone == null)
            {
                _pelvisBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
            }
        }

        private void Start()
        {
            foreach (var rigidbody in _pelvisBone.GetComponentsInChildren<Rigidbody>())
            {
                _ragdollRigidbodys.Add(rigidbody);
            }

            DisableRagDoll();

            /*if (_keepPelVisOffset)
            {
                _pelvisOffset = gameObject.transform.position - GetRagdollBone(HumanBodyBones.Hips).transform.position;
            } */           
        }

        public void EnableRagDoll()
        {
            DoRagDoll();
            if (_ragDollTime > 0)
            {
                Invoke("DisableRagDoll", _ragDollTime);
            }            
        }

        void DoRagDoll()
        {
            // 开启布娃娃中所有Rigidbody和Collider
            for (int i = 0; i < _ragdollRigidbodys.Count; i++)
            {
                _ragdollRigidbodys[i].isKinematic/*是否动力学*/ = false; // 即关闭运动学（如动画、脚本），而使用物理模拟控制刚体
                //_ragdollRigidbodys[i].detectCollisions = true;
            }

            Enabled = true;
        }

        public void DisableRagDoll()
        {
            // 关闭布娃娃中所有Rigidbody和Collider
            for (int i = 0; i < _ragdollRigidbodys.Count; i++)
            {
                /* 此时Forces, collisions or joints will not affect the rigidbody anymore. 
                 The rigidbody will be under full control of animation or script control 
                by changing transform.position*/
                _ragdollRigidbodys[i].isKinematic/*是否动力学*/ = true;// 即开启运动学（如动画、脚本）控制刚体，从而关闭物理模拟
                //_ragdollRigidbodys[i].detectCollisions = false;
            }

            Enabled = false;
            /*if (_keepPelVisOffset)
            {
                Vector3 pos = GetRagdollBone(HumanBodyBones.Hips).transform.position;
                GetRagdollBone(HumanBodyBones.Hips).transform.position = gameObject.transform.position - _pelvisOffset;
                gameObject.transform.position = pos;
            }*/
        }

        public Rigidbody GetRagdollBone(HumanBodyBones bone)
        {
            Transform boneTransform = _animator.GetBoneTransform(bone);
            if (boneTransform == null)
            {
                return null;
            }

            return boneTransform.GetComponent<Rigidbody>();
        }        
    }
}