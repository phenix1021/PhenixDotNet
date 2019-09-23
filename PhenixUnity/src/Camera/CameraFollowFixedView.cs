using UnityEngine;

namespace Phenix.Unity.Camera
{
    public class CameraFollowFixedView : CameraFollow
    {
        [SerializeField]
        Vector3 _offset = new Vector3(0, 8, -6.4f);               

        protected override void LateUpdate()
        {
            if (camera == null)
            {
                return;
            }

            Follow();
        }

        void Follow()
        {
            Vector3 destPos = GetDestPos();
            camera.transform.position = Vector3.Lerp(camera.transform.position, destPos, Time.deltaTime * speed);
            
            Vector3 finalDir = target.position - camera.transform.position;
            finalDir.Normalize();

            camera.transform.forward = Vector3.Lerp(camera.transform.forward, finalDir, Time.deltaTime * rotateSpeed);
        }

        Vector3 GetDestPos()
        {
            if (target == null)
            {
                return camera.transform.position;
            }

            return target.position + _offset;
        }
    }
}