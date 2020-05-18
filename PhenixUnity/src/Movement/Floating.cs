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

        Vector3 _basePos;   // 基准位置

        // Use this for initialization
        void Start()
        {
            _basePos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 tarPos = Mathf.Sin(Time.time) * _floatRange * _axis.normalized + _basePos;
            transform.position = MathTools.Hermite(transform.position, tarPos, Time.deltaTime * _speed);
        }
    }
}