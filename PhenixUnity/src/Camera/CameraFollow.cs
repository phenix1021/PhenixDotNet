using UnityEngine;

namespace Phenix.Unity.Camera
{
    public abstract class CameraFollow : MonoBehaviour
    {
        public UnityEngine.Camera cam;
        public Transform target;

        [SerializeField]
        protected float speed = 4;          // 平移速度

        [SerializeField]
        protected float rotateSpeed = 4;    // 旋转速度

        protected virtual void Awake()
        {
            cam = UnityEngine.Camera.main;
        }

        protected virtual void Start() { }
        protected virtual void LateUpdate() { }
    }
}