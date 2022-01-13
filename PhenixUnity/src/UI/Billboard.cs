using UnityEngine;

namespace Phenix.Unity.UI
{
    /// <summary>
    /// 广告牌效果（多用于场景对象头顶HUD，如血条等。确保UI始终对着主相机。注意：头顶HUD的canvas的Render Mode是World Space）
    /// </summary>        
    [AddComponentMenu("Phenix/UI/Billboard")]
    public class Billboard : MonoBehaviour
    {
        UnityEngine.Camera _mainCam;

        private void Awake()
        {
            _mainCam = UnityEngine.Camera.main;
        }

        private void Update()
        {
            Rot2();
        }

        // 方法1：获得对应屏幕上（z = 0）或屏幕后（z < 0）的点，然后LookAt该点
        void Rot1()
        {
            Vector3 screenPos = _mainCam.WorldToScreenPoint(transform.position);
            screenPos.z = -1000; // 必须小于等于0
            transform.LookAt(_mainCam.ScreenToWorldPoint(screenPos));
        }

        // 方法2：往camera所在平面投射，LookAt和该平面的交点
        void Rot2()
        {
            if (_mainCam == null)
            {
                _mainCam = UnityEngine.Camera.main;
                if (_mainCam == null)
                {
                    return;
                }
            }

            Plane camPlane = new Plane(_mainCam.transform.forward, _mainCam.transform.position);
            float dis;
            Vector3 tar = _mainCam.transform.position;
            if (camPlane.Raycast(new Ray(transform.position, -_mainCam.transform.forward), out dis) == true)
            {
                tar = transform.position + (-_mainCam.transform.forward * dis);
            }

            transform.LookAt(tar, Vector3.up);
        }
    }
}