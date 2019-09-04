using UnityEngine;

namespace Phenix.Unity.Extend
{

    public static class Vector2Extend
    {
        /// <summary>
        /// 旋转一定角度
        /// </summary>
        /// <param name="v"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 vec2, float degrees)
        {
            float rads = degrees * Mathf.Deg2Rad;

            float cos = Mathf.Cos(rads);
            float sin = Mathf.Sin(rads);

            float vx = vec2.x;
            float vy = vec2.y;

            return new Vector2(cos * vx - sin * vy, sin * vx + cos * vy);
        }
    }
}