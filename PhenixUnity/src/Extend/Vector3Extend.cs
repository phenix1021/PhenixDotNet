using UnityEngine;

namespace Phenix.Unity.Extend
{

    public static class Vector3Extend
    {
        /// <summary>
        /// 围绕axis旋转angle角度
        /// </summary>
        public static Vector3 Rotate(this Vector3 vec3, Vector3 axis, float angle)
        {
            return Quaternion.AngleAxis(angle, axis) * vec3;
        }

        /// <summary>
        /// 返回Vector3在xz平面上的弧度值
        /// </summary>
        public static float RadianInXZ(this Vector3 vec3)
        {
            Vector3 v = Vector3.forward;
            Vector3 vec = new Vector3(vec3.x, 0, vec3.z);
            return Mathf.PI - Mathf.Atan2(vec.normalized.z, vec.normalized.x);
        }
    }
}