using UnityEngine;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.Movement
{
    /// <summary>
    /// 自旋
    /// </summary>
    [AddComponentMenu("Phenix/Movement/Spin")]
    public class Spin : MonoBehaviour
    {
        [SerializeField]
        Vector3 _axis = Vector3.up;      // 旋转轴
        
        [SerializeField]
        float _speed = 10;   // 旋转速度

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(_axis, Time.deltaTime * _speed);
        }
    }
}