using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Phenix.Unity.AI.Locomotion
{
    // 自定义offLink时的移动路径
    [System.Serializable]
    public enum CustomedMoveTrace
    {
        NONE = 0,
        LINE,       // 直线
        PARABOLA,   // 抛物线
        CURVE,      // 曲线
        FLASH,      // 瞬移（如战术游戏中npc从一楼大门进入消失，过一会儿从顶楼大门出现）
    }

    [System.Serializable]
    public class CustomedMoveData
    {
        public float speed = 1;
        public CustomedMoveTrace traceType = CustomedMoveTrace.LINE;
        public float parabolaHeight = 1; // traceType为PARABOLA时有效
        public AnimationCurve curve; // traceType为CURVE时有效
        public bool faceToTrace = false; // 移动时是否面朝轨迹（如上下楼梯时为true）
    }

    // 针对不同导航网格area的自定义移动数据
    [System.Serializable]
    public class CustomedAreaMoveData
    {
        public int area; // navmesh的area下标
        public CustomedMoveData data;
    }

    public class NavMeshOffLink : MonoBehaviour
    {
        public NavMeshAgent NavMeshAgent { get; private set; }

        bool _preIsOnOffMeshLink = false;
        OffMeshLinkData _offData = default(OffMeshLinkData);

        public UnityAction<OffMeshLinkData> onFall;
        public UnityAction<OffMeshLinkData> onFalling;
        public UnityAction<OffMeshLinkData> onLand;
        public UnityAction<OffMeshLinkData> onJumpBegin;
        public UnityAction<OffMeshLinkData> onJumping;
        public UnityAction<OffMeshLinkData> onJumpEnd;
        public UnityAction<OffMeshLinkData> onManualLinkBegin; // 如爬梯
        public UnityAction<OffMeshLinkData> onManualLinkMoving;// 如爬梯
        public UnityAction<OffMeshLinkData> onManualLinkEnd;   // 如爬梯

        [SerializeField]
        bool _isCustomedMove = true;    // 是否自定义offLink时的运动轨迹
        [SerializeField]
        CustomedMoveData _customedFall; // 自定义的fall运动数据
        [SerializeField]
        CustomedMoveData _customedJump; // 自定义的jump运动数据
        [SerializeField]
        List<CustomedAreaMoveData> _customedManualLinks = new List<CustomedAreaMoveData>();

        CustomedMoveData _customedManualLink; // 自定义的manualLink(如爬梯)运动数据

        public float minCustomedMoveTime = 0.5f; // 自定义移动最小时长（秒）
        public float maxCustomedMoveTime = 10f;  // 自定义移动最大时长（秒）

        float _moveTimeForCustomed;
        Vector3 _beginPosForCustomed, _endPosForCustomed;
        float _progressForCustomed = 0.0f;

        void Awake()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        void Start()
        {
            NavMeshAgent.autoTraverseOffMeshLink = !_isCustomedMove;
        }

        void Update()
        {
            if (NavMeshAgent.isOnOffMeshLink)
            {
                if (_preIsOnOffMeshLink == false)
                {
                    _offData = NavMeshAgent.currentOffMeshLinkData;
                    OnEnterLink();
                    _preIsOnOffMeshLink = true;
                }

                OnMoveInLink();
            }
            /*else
            {
                if (_preIsOnOffMeshLink)
                {
                    OnExitLink();                
                    _offData = default(OffMeshLinkData);
                }            
            }

            _preIsOnOffMeshLink = NavMeshAgent.isOnOffMeshLink;*/
        }

        void OnEnterLink()
        {
            if (_isCustomedMove)
            {
                _progressForCustomed = 0;
                _beginPosForCustomed = transform.position;
                _endPosForCustomed = _offData.endPos + Vector3.up * NavMeshAgent.baseOffset;
            }

            switch (_offData.linkType)
            {
                case OffMeshLinkType.LinkTypeDropDown:
                    OnFall();
                    break;
                case OffMeshLinkType.LinkTypeJumpAcross:
                    OnJumpBegin();
                    break;
                case OffMeshLinkType.LinkTypeManual:
                    OnManualLinkBegin();
                    break;
                default:
                    break;
            }
        }

        void OnMoveInLink()
        {
            switch (_offData.linkType)
            {
                case OffMeshLinkType.LinkTypeDropDown:
                    OnFalling();
                    break;
                case OffMeshLinkType.LinkTypeJumpAcross:
                    OnJumping();
                    break;
                case OffMeshLinkType.LinkTypeManual:
                    OnManualLinkMoving();
                    break;
                default:
                    break;
            }

            if (_isCustomedMove)
            {
                if (_progressForCustomed == 1)
                {
                    // 自定义移动完成
                    NavMeshAgent.CompleteOffMeshLink();
                    OnExitLink();
                    _offData = default(OffMeshLinkData);
                }
            }

            _preIsOnOffMeshLink = NavMeshAgent.isOnOffMeshLink;
        }

        void OnExitLink()
        {
            switch (_offData.linkType)
            {
                case OffMeshLinkType.LinkTypeDropDown:
                    OnLand();
                    break;
                case OffMeshLinkType.LinkTypeJumpAcross:
                    OnJumpEnd();
                    break;
                case OffMeshLinkType.LinkTypeManual:
                    OnManualLinkEnd();
                    break;
                default:
                    break;
            }
        }

        void CustomedLineMoving()
        {
            transform.position = Vector3.Lerp(_beginPosForCustomed, _endPosForCustomed,
                _progressForCustomed);
            _progressForCustomed += Time.deltaTime / _moveTimeForCustomed;
            if (_progressForCustomed > 1)
            {
                _progressForCustomed = 1;
            }
        }

        void CustomedParabolaMoving(float height)
        {
            float yOffset = height * 4.0f * (_progressForCustomed -
                _progressForCustomed * _progressForCustomed);
            transform.position = Vector3.Lerp(_beginPosForCustomed, _endPosForCustomed,
                _progressForCustomed) + yOffset * Vector3.up;
            _progressForCustomed += Time.deltaTime / _moveTimeForCustomed;
            if (_progressForCustomed > 1)
            {
                _progressForCustomed = 1;
            }
        }

        void CustomedCurveMoving(AnimationCurve curve)
        {
            float yOffset = curve.Evaluate(_progressForCustomed);
            transform.position = Vector3.Lerp(_beginPosForCustomed, _endPosForCustomed,
                _progressForCustomed) + yOffset * Vector3.up;
            _progressForCustomed += Time.deltaTime / _moveTimeForCustomed;
            if (_progressForCustomed > 1)
            {
                _progressForCustomed = 1;
            }
        }

        void OnFall()
        {
            Debug.Log("Fall");
            _moveTimeForCustomed = GetCustomedMoveTime(_beginPosForCustomed,
                _endPosForCustomed, _customedFall.speed);

            if (_customedFall.traceType == CustomedMoveTrace.FLASH)
            {
                // 如果是瞬移
                GetComponent<Renderer>().enabled = false;
            }

            if (onFall != null)
            {
                onFall.Invoke(_offData);
            }
        }

        void OnFalling()
        {
            if (_isCustomedMove)
            {
                // 自定义移动路径                        
                CustomedMoving(_customedFall.parabolaHeight, _customedFall.curve);
            }

            if (onFalling != null)
            {
                onFalling.Invoke(_offData);
            }
        }

        void OnLand()
        {
            Debug.Log("Land");
            if (_customedFall.traceType == CustomedMoveTrace.FLASH)
            {
                // 如果是瞬移
                GetComponent<Renderer>().enabled = true;
            }

            if (onLand != null)
            {
                onLand.Invoke(_offData);
            }
        }

        void OnJumpBegin()
        {
            Debug.Log("JumpBegin");
            _moveTimeForCustomed = GetCustomedMoveTime(_beginPosForCustomed,
                _endPosForCustomed, _customedJump.speed);

            if (_customedJump.traceType == CustomedMoveTrace.FLASH)
            {
                // 如果是瞬移
                GetComponent<Renderer>().enabled = false;
            }

            if (onJumpBegin != null)
            {
                onJumpBegin.Invoke(_offData);
            }
        }

        void OnJumping()
        {
            if (_isCustomedMove)
            {
                // 自定义移动路径                        
                CustomedMoving(_customedJump.parabolaHeight, _customedJump.curve);
            }

            if (onJumping != null)
            {
                onJumping.Invoke(_offData);
            }
        }

        void OnJumpEnd()
        {
            Debug.Log("JumpEnd");
            if (_customedJump.traceType == CustomedMoveTrace.FLASH)
            {
                // 如果是瞬移
                GetComponent<Renderer>().enabled = true;
            }

            if (onJumpEnd != null)
            {
                onJumpEnd.Invoke(_offData);
            }
        }

        void OnManualLinkBegin()
        {
            Debug.Log("ManualLinkBegin");
            // 根据offMeshLink的area获得_customedManualLink        
            foreach (var manualLink in _customedManualLinks)
            {
                if (manualLink.area == _offData.offMeshLink.area)
                {
                    _customedManualLink = manualLink.data;
                    break;
                }
            }

            // 计算移动时长
            _moveTimeForCustomed = GetCustomedMoveTime(_beginPosForCustomed,
                _endPosForCustomed, _customedManualLink.speed);

            if (_customedManualLink.traceType == CustomedMoveTrace.FLASH)
            {
                // 如果是瞬移
                GetComponent<Renderer>().enabled = false;
            }

            if (onManualLinkBegin != null)
            {
                onManualLinkBegin.Invoke(_offData);
            }
        }

        void OnManualLinkMoving()
        {
            if (_isCustomedMove)
            {
                // 自定义移动路径                        
                CustomedMoving(_customedManualLink.parabolaHeight, _customedManualLink.curve);
            }

            if (onManualLinkMoving != null)
            {
                onManualLinkMoving.Invoke(_offData);
            }
        }

        void OnManualLinkEnd()
        {
            Debug.Log("ManualLinkEnd");
            if (_customedManualLink.traceType == CustomedMoveTrace.FLASH)
            {
                // 如果是瞬移
                GetComponent<Renderer>().enabled = true;
            }

            if (onManualLinkEnd != null)
            {
                onManualLinkEnd.Invoke(_offData);
            }
        }

        void CustomedFlashMoving()
        {
            _progressForCustomed += Time.deltaTime / _moveTimeForCustomed;
            if (_progressForCustomed > 1)
            {
                _progressForCustomed = 1;
            }
        }

        void CustomedMoving(float parabolaHeight, AnimationCurve curve)
        {
            if (_progressForCustomed < 1.0f)
            {
                switch (_customedFall.traceType)
                {
                    case CustomedMoveTrace.LINE:
                        CustomedLineMoving();
                        break;
                    case CustomedMoveTrace.PARABOLA:
                        CustomedParabolaMoving(parabolaHeight);
                        break;
                    case CustomedMoveTrace.CURVE:
                        CustomedCurveMoving(curve);
                        break;
                    case CustomedMoveTrace.FLASH:
                        CustomedFlashMoving();
                        break;
                    default:
                        break;
                }
            }
        }

        float GetCustomedMoveTime(Vector3 beginPos, Vector3 endPos, float speed)
        {
            float ret = Vector3.Distance(beginPos, endPos) / speed;
            return Mathf.Min(Mathf.Max(ret, minCustomedMoveTime), maxCustomedMoveTime);
        }

        private void LateUpdate()
        {
            // 可在此调整动画动作
            //if (NavMeshAgent.isOnOffMeshLink)
            //{
            //    _customedManualLink.faceToTrace
            //}
        }

        private void OnAnimatorIK(int layerIndex)
        {
            // 可在此调整动画动作
        }
    }
}