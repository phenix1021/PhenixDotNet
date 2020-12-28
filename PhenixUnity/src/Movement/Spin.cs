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
        public Vector3 axis = Vector3.up;      // 旋转轴
        
        [SerializeField]
        float _speed = 10;   // 旋转速度

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(axis, Time.deltaTime * _speed);
        }
    }
}