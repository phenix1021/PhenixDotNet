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

        public static GameObject ancestor(this GameObject go)
        {
            while (go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
            }
            return go;
        }
    }
}