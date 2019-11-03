using UnityEngine;

namespace Phenix.Unity.Camera
{
    [AddComponentMenu("Phenix/Camera/CameraFollowFixedView")]
    public class CameraFollowFixedView : CameraFollow
    {
        [SerializeField]
        Vector3 _offset = new Vector3(0, 8, -6.4f);               

        protected override void LateUpdate()
        {
            if (cam == null)
            {
                return;
            }

            Follow();
        }

        void Follow()
        {
            Vector3 destPos = GetDestPos();
            cam.transform.position = Vector3.Lerp(cam.transform.position, destPos, Time.deltaTime * speed);
            
            Vector3 finalDir = target.position - cam.transform.position;
            finalDir.Normalize();

            cam.transform.forward = Vector3.Lerp(cam.transform.forward, finalDir, Time.deltaTime * rotateSpeed);
        }

        Vector3 GetDestPos()
        {
            if (target == null)
            {
                return cam.transform.position;
            }

            return target.position + _offset;
        }
    }
}