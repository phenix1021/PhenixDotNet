using UnityEngine;

namespace Phenix.Unity.Extend
{

    public static class Vector3Extend
    {
        /// <summary>
        /// 围绕axis旋转angle角度
        /// </summary>
        /// <param name="vec3"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 Rotate(this Vector3 vec3, Vector3 axis, float angle)
        {
            return Quaternion.AngleAxis(angle, axis) * vec3;
        }

    }
}