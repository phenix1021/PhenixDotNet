using UnityEngine;
using System.Collections;

namespace Phenix.Unity.AI
{
    [AddComponentMenu("Phenix/Behavior Tree Agent")]
    [DisallowMultipleComponent]
    public class BehaviorTreeAgent : MonoBehaviour
    {
        [SerializeField]
        BehaviorTreeAsset _btAsset;

        public BehaviorTreeAsset BTAsset { get { return _btAsset; } }

        void Awake()
        {
            if (_btAsset == null)
            {
                return;
            }

            BehaviorTreeAsset tmp = ScriptableObject.Instantiate<BehaviorTreeAsset>(_btAsset);
            if (Application.isEditor)
            {
                tmp.MonitorAsset = _btAsset;
            }
            if (tmp.Deserialize(transform) == false)
            {
                Debug.LogError(string.Format("deserialization failure."), this);
                return;
            }
            _btAsset = tmp;

            _btAsset.BT.OnAwake();
        }

        void Start()
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnStart();
        }

        // Update is called once per frame
        void Update()
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnUpdate();
        }

        /*private void FixedUpdate()
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnFixedUpdate();
        }

        private void LateUpdate()
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnLateUpdate();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnAnimatorIK(layerIndex);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnCollisionEnter(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnCollisionStay(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnCollisionExit(collision);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnCollisionEnter2D(collision);
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnCollisionStay2D(collision);
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnCollisionExit2D(collision);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnControllerColliderHit(hit);
        }

        private void OnDrawGizmos()
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnDrawGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnDrawGizmosSelected();
        }

        private void OnApplicationPause(bool pause)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnApplicationPause(pause);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnTriggerEnter(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnTriggerStay(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnTriggerExit(other);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnTriggerEnter2D(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnTriggerStay2D(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_btAsset == null)
            {
                return;
            }

            _btAsset.BT.OnTriggerExit2D(collision);
        }*/
    }
}