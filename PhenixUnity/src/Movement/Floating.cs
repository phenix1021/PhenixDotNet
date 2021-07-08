using UnityEngine;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.Movement
{
    /// <summary>
    /// 上下浮动
    /// </summary>
    [AddComponentMenu("Phenix/Movement/Floating")]
    public class Floating : MonoBehaviour
    {
        [SerializeField]
        Vector3 _axis;      // 浮动轴

        [SerializeField]
        float _floatRange;  // 浮动范围[-_floatRange, _floatRange]

        [SerializeField]
        float _speed = 1;   // 浮动速度

        public Vector3 basePos;   // 基准位置
        public Transform baseObj; // 基准方位

        // Use this for initialization
        void Start()
        {
            if (baseObj == null)
            {
                basePos = transform.position;
            }            
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 tarPos;

            if (baseObj)
            {
                tarPos = Mathf.Sin(Time.time) * _floatRange * _axis.normalized + baseObj.position;
            }
            else
            {
                tarPos = Mathf.Sin(Time.time) * _floatRange * _axis.normalized + basePos;
            }

            //transform.position = MathTools.Hermite(transform.position, tarPos, Time.deltaTime * _speed);
            transform.position = Vector3.Lerp(transform.position, tarPos, Time.deltaTime * _speed);
        }
    }
}