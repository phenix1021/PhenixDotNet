using UnityEngine;

namespace Phenix.Unity.Camera
{
    [AddComponentMenu("Phenix/Camera/CameraFollowFixedView")]
    public class CameraFollowFixedView : CameraFollow
    {
        [SerializeField]
        Vector3 _offset = new Vector3(0, 8, -6.4f);

        [SerializeField]
        float _speed = 4;          // 平移速度

        [SerializeField]
        float _rotateSpeed = 4;    // 旋转速度

        Vector3 GetDestDir()
        {
            return target.position - cam.transform.position;
        }

        Vector3 GetDestPos()
        {
            if (target == null)
            {
                return cam.transform.position;
            }

            return target.position + _offset;
        }

        protected override void Follow()
        {
            Vector3 destPos = GetDestPos();
            cam.transform.position = Vector3.Lerp(cam.transform.position, 
                destPos, Time.deltaTime * _speed);

            Vector3 finalDir = GetDestDir();
            finalDir.Normalize();

            cam.transform.forward = Vector3.Lerp(cam.transform.forward, finalDir, 
                Time.deltaTime * _rotateSpeed);
        }
    }
}