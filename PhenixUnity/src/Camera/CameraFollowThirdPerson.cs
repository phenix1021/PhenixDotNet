using UnityEngine;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.Camera
{
    [AddComponentMenu("Phenix/Camera/CameraFollowThirdPerson")]
    public class CameraFollowThirdPerson : CameraFollow
    {
        public Transform lookAt;    // 相机旋转、移动的参照原点位置
        public float distance;             

        public float minVerticalRotAngle = -60;
        public float maxVerticalRotAngle = 80;
        public float minDistance = 2;
        public float maxDistance = 8;

        public float cameraSpeed = 20;

        public LayerMask collisionLayerMask;

        float _eulerAnglesX = 0;
        float _eulerAnglesY = 0;

        Vector3 _initLookAtPos;
        float _initMinVerticalRotAngle;
        float _initMaxVerticalRotAngle;
        float _initMinDistance;
        float _initMaxDistance;

        public void Reset()
        {
            lookAt.localPosition = _initLookAtPos;
            minVerticalRotAngle = _initMinVerticalRotAngle;
            maxVerticalRotAngle = _initMaxVerticalRotAngle;
            minDistance = _initMinDistance;
            maxDistance = _initMaxDistance;
            distance = (minDistance + maxDistance) * 0.5f;
            cam.transform.forward = lookAt.forward;
        }

        protected override void Start()
        {
            base.Start();
            if (lookAt == null)
            {
                lookAt = target;
            }
            _initLookAtPos = lookAt.localPosition;
            _initMinVerticalRotAngle = minVerticalRotAngle;
            _initMaxVerticalRotAngle = maxVerticalRotAngle;
            _initMinDistance = minDistance;
            _initMaxDistance = maxDistance;
            distance = (minDistance + maxDistance) * 0.5f;
            cam.transform.forward = lookAt.forward;
        }

        // 拉近拉远
        public void Zoom(float val)
        {
            if (lookAt == null || val == 0)
            {
                return;
            }
            distance -= val;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // 水平旋转
        public void RotateH(float val)
        {
            if (lookAt == null || val == 0)
            {
                return;
            }
            _eulerAnglesY += val;
            _eulerAnglesY = MathTools.NormalizeAngle(_eulerAnglesY);
        }

        // 垂直旋转
        public void RotateV(float val)
        {
            if (lookAt == null || val == 0)
            {
                return;
            }
            _eulerAnglesX -= val;            
            _eulerAnglesX = MathTools.ClampAngle(_eulerAnglesX, minVerticalRotAngle, 
                maxVerticalRotAngle);
        }

        protected override void Follow()
        {
            cam.transform.rotation = Quaternion.Euler(_eulerAnglesX, _eulerAnglesY, 0);
            Vector3 position = lookAt.position + cam.transform.rotation *
                (-Vector3.forward) * distance;

            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Linecast(lookAt.position, position, out hitInfo, collisionLayerMask))
            {                
                position = hitInfo.point;
            }

            cam.transform.position = Vector3.Lerp(cam.transform.position, position, Time.deltaTime * cameraSpeed);
        }
    }
}