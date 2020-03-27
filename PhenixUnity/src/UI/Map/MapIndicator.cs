using UnityEngine;
using System.Collections.Generic;

namespace Phenix.Unity.UI
{
    /// <summary>
    /// 绑定在地图内容物对象上，可以根据具体需求自行派生扩展内容，
    /// 如mapIcon等等
    /// </summary>
    public class MapIndicator : MonoBehaviour
    {
        [HideInInspector]
        public Vector2 relativeMapPos;  // 相对center的2D位置
        [HideInInspector]
        public float offsetAngle;       // 2D平面的face偏移角度。rectTransform.eulerAngles = new Vector3(0, 0, offsetAngle)

        public static void GetIndicators(Vector3 center, Vector3 range, LayerMask layerMask,
            float scale/*比例尺*/, ref List<MapIndicator> ret)
        {
            ret.Clear();
            Collider[] colliders = Physics.OverlapBox(center, range * 0.5f, 
                Quaternion.identity, layerMask);
            HandleColliders(center, scale, colliders, ref ret);
        }

        public static void GetIndicators(Vector3 center, float radius, LayerMask layerMask,
                float scale/*比例尺*/, ref List<MapIndicator> ret)
        {
            ret.Clear();
            Collider[] colliders = Physics.OverlapSphere(center, radius, layerMask);
            HandleColliders(center, scale, colliders, ref ret);
        }

        static void HandleColliders(Vector3 center, float scale/*比例尺*/, Collider[] colliders,
            ref List<MapIndicator> ret)
        {
            foreach (var collider in colliders)
            {                
                MapIndicator mapIndicator = collider.GetComponent<MapIndicator>();
                if (mapIndicator)
                {
                    Vector3 pos = mapIndicator.transform.position - center;
                    mapIndicator.relativeMapPos.x = pos.x * scale;
                    mapIndicator.relativeMapPos.y = pos.z * scale;
                    mapIndicator.offsetAngle = Utilities.TransformTools.Instance.
                        ForwardAngle360InXZ(mapIndicator.transform);
                    ret.Add(mapIndicator);
                }
            }
        }
    }
}