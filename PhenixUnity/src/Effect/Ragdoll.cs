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
        bool _playOnAwake = false;

        [SerializeField]
        float _minSpeed = 1;

        [SerializeField]
        Transform _pelvisBone; // 根骨骼
        [SerializeField]
        Animator _animator;
        [SerializeField]
        Collider _collider;    // 本身的collider，并非ragdoll里的collider 

        List<Rigidbody> _ragdollRigidbodys = new List<Rigidbody>();
        List<Collider> _ragdollColliders = new List<Collider>();

        bool _active = false;

        private void Awake()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            if (_collider == null)
            {
                _collider = GetComponent<Collider>();
            }
            if (_pelvisBone == null && _animator)
            {
                _pelvisBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
            }
        }

        private void Start()
        {
            foreach (var rigidbody in _pelvisBone.GetComponentsInChildren<Rigidbody>())
            {
                _ragdollRigidbodys.Add(rigidbody);
                _ragdollColliders.Add(rigidbody.gameObject.GetComponent<Collider>());
            }

            if (_playOnAwake)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        public void Open()
        {
            // 开启布娃娃中所有Rigidbody和Collider
            for (int i = 0; i < _ragdollRigidbodys.Count; i++)
            {
                _ragdollRigidbodys[i].isKinematic = false;
                _ragdollColliders[i].isTrigger = false;
            }

            if (_collider)
            {
                // 关闭自身Collider
                _collider.enabled = false;
            }            

            if (_animator)
            {
                // 关闭Animator
                _animator.enabled = false;
            }

            _active = true;
        }

        public void Close()
        {
            // 关闭布娃娃中所有Rigidbody和Collider
            for (int i = 0; i < _ragdollRigidbodys.Count; i++)
            {
                _ragdollRigidbodys[i].isKinematic = true;
                _ragdollColliders[i].isTrigger = true;
            }

            if (_collider)
            {
                // 开启自身Collider
                _collider.enabled = true;
            }

            if (_animator)
            {
                // 开启自身Animator
                _animator.enabled = true;
            }

            _active = false;
        }

        public Rigidbody GetRagdollBone(HumanBodyBones bone)
        {
            if (_animator == null)
            {
                return null;
            }

            Transform boneTran = _animator.GetBoneTransform(bone);
            if (boneTran == null)
            {
                return null;
            }

            return boneTran.GetComponent<Rigidbody>();
        }
    }
}