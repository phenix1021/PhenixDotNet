using System.Collections;
using UnityEngine;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.Movement
{
    /// <summary>
    /// 上下浮动
    /// </summary>
    [AddComponentMenu("Phenix/Movement/Floating2")]
    public class Floating2 : MonoBehaviour
    {
        [SerializeField]
        Vector3 _axis;      // 浮动轴

        [SerializeField]
        float _floatRange = 1;  // 浮动范围[-_floatRange, _floatRange]

        [SerializeField]
        float _speed = 3;   // 浮动速度

        Vector3 _top;
        Vector3 _bottom;
                
        void Awake()
        {
            _top = transform.localPosition + _axis.normalized * _floatRange;
            _bottom = transform.localPosition - _axis.normalized * _floatRange;            
        }

        private void OnEnable()
        {
            StopCoroutine("DoFloating");   
            StartCoroutine("DoFloating");  // 必须放在OnEnable，放在Start里如果对象enable关闭重开时，DoFloating不会继续
        }

        IEnumerator DoFloating()
        {
            Vector3 _tarPos = _bottom;
            Vector3 _srcPos = transform.localPosition;
            bool toUp = false;
            while (true)
            {
                if (Vector3.Distance(transform.localPosition, _tarPos) < 0.1f) // 到达
                {
                    if (toUp == false)
                    {
                        // 转向上
                        _tarPos = _top;
                        _srcPos = _bottom;
                    }
                    else
                    {
                        // 转向下
                        _tarPos = _bottom;
                        _srcPos = _top;
                    }

                    toUp = !toUp;
                }

                //transform.position = Vector3.Lerp(_srcPos, _tarPos, Time.deltaTime * _speed); 为什么这样写一动不动？
                transform.localPosition = Vector3.Lerp(transform.localPosition, _tarPos, Time.deltaTime * _speed);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}