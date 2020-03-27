using UnityEngine;

namespace Phenix.Unity.Camera
{
    public abstract class CameraFollow : MonoBehaviour
    {
        public UnityEngine.Camera cam;
        public Transform target;

        protected virtual void Awake()
        {
            cam = UnityEngine.Camera.main;
        }

        protected virtual void Start() { }

        protected virtual void LateUpdate()        
        {
            if (cam == null)
            {
                return;
            }

            Follow();
        }

        protected abstract void Follow();
    }
}