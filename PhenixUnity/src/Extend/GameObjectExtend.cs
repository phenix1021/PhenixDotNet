using UnityEngine;

namespace Phenix.Unity.Extend
{
    public static class GameObjectExtend
    {
        /// <summary>
        /// 为当前GameObject及其子对象设置layer
        /// </summary>
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            SetLayerForChildren(gameObject.transform, layer);
        }

        static void SetLayerForChildren(Transform transform, int layer)
        {
            int numChildren = transform.childCount;
            if (numChildren > 0)
            {
                for (int i = 0; i < numChildren; ++i)
                {
                    Transform child = transform.GetChild(i);
                    child.gameObject.layer = layer;
                    SetLayerForChildren(child, layer);
                }
            }
        }

        /// <summary>
        /// 返回hierarchy上的根对象
        /// </summary>        
        public static GameObject Ancestor(this GameObject go)
        {
            while (go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
            }
            return go;
        }

        /// <summary>
        /// 对象全名（aa/bb/cc/dd/ee/...）
        /// </summary>
        public static string FullName(this GameObject go)
        {
            string ret = string.Concat(string.Empty, go.name);
            while (go.transform.parent != null)
            {
                GameObject parent = go.transform.parent.gameObject;
                ret = string.Concat(parent.name, "/", ret);
                go = parent;
            }
            return ret;
        }

        /// <summary>
        /// 返回forward方向在xz平面上的角度值
        /// </summary>
        public static float ForwardAngle(this GameObject go)
        {
            return ForwardRadian(go) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 返回forward方向在xz平面上的弧度值
        /// </summary>
        public static float ForwardRadian(this GameObject go)
        {
            Vector3 vec = new Vector3(go.transform.forward.x, 0, go.transform.forward.z);
            return Mathf.PI - Mathf.Atan2(vec.normalized.z, vec.normalized.x);
        }
    }
}