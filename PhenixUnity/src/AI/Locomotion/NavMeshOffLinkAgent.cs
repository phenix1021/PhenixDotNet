using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Phenix.Unity.AI.Locomotion
{
    // offLink移动路径类型
    [System.Serializable]
    public enum NavMeshOffLinkMoveTrace
    {        
        LINE = 0,       // 直线
        PARABOLA,       // 抛物线
        CURVE,          // 曲线
        HIDE_TRACE,     // 隐藏轨迹（如战术游戏中npc从一楼大门进入消失，过一会儿从顶楼大门出现）
    }

    // offLink移动数据
    [System.Serializable]
    public class NavMeshOffLinkMoveData
    {
        public float speed = 1;                       // 移动速度
        public NavMeshOffLinkMoveTrace traceType;     // 轨迹类型
        public float parabolaHeight = 1;              // 仅traceType为PARABOLA时有效
        public AnimationCurve curve;                  // 仅traceType为CURVE时有效        
    }

    // offLink移动信息
    [System.Serializable]
    public class NavMeshOffLinkMoveInfo
    {
        public string areaName;                 // navmesh的area名称
        public NavMeshOffLinkMoveData data;     // offLink移动数据
    }

    /// <summary>
    /// 导航网格OffLink过程处理。依照定制数据接管offlink时的移动表现。
    /// </summary>
    [AddComponentMenu("Phenix/NavMeshOffLinkAgent")]
    [DisallowMultipleComponent]
    public class NavMeshOffLinkAgent : MonoBehaviour
    {
        public NavMeshAgent NavMeshAgent { get; private set; }

        bool _preIsOnOffMeshLink = false;
        bool _hideTrace = false;

        // 各回调接口，可用于处理相应动画调用等
        public UnityAction<OffMeshLinkData> onFall;
        public UnityAction<OffMeshLinkData> onFalling;
        public UnityAction<OffMeshLinkData> onLand;
        public UnityAction<OffMeshLinkData> onJumpBegin;
        public UnityAction<OffMeshLinkData> onJumping;
        public UnityAction<OffMeshLinkData> onJumpEnd;
        public UnityAction<OffMeshLinkData> onManualLinkBegin;      // 如爬梯
        public UnityAction<OffMeshLinkData> onManualLinkMoving;     // 如爬梯
        public UnityAction<OffMeshLinkData> onManualLinkNearEnd;    // 如爬梯
        public UnityAction<OffMeshLinkData> onManualLinkEnd;        // 如爬梯

        [SerializeField]
        NavMeshOffLinkMoveData _fallData; // 自定义的fall运动数据
        [SerializeField]
        NavMeshOffLinkMoveData _jumpData; // 自定义的jump运动数据
        [SerializeField]
        List<NavMeshOffLinkMoveInfo> _manualLinks = new List<NavMeshOffLinkMoveInfo>(); // 自定义的各种manualLink(如爬梯)运动数据

        NavMeshOffLinkMoveData _curManualLink; 

        public float minMoveTime = 0.5f; // 自定义移动最小时长（秒）
        public float maxMoveTime = 10f;  // 自定义移动最大时长（秒）

        float _moveTime;                    // offlink移动需要时长（秒）
        Vector3 _moveBeginPos, _moveEndPos; // offlink移动开始位置、停止位置
        float _moveProgress = 0.0f;         // offlink移动进度

        void Awake()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        void Start()
        {
            
        }

        void Update()
        {
            if (NavMeshAgent.autoTraverseOffMeshLink)
            {
                // 如果NavMeshAgent.autoTraverseOffMeshLink设置为true，则不处理
                return;
            }

            if (NavMeshAgent.isOnOffMeshLink)
            {
                if (_preIsOnOffMeshLink == false)
                {                    
                    OnEnterLink();
                    _preIsOnOffMeshLink = true;
                }

                OnMoveInLink();
            }
        }

        void OnEnterLink()
        {
            _moveProgress = 0;
            //_moveBeginPos = transform.position;
            _moveBeginPos = NavMeshAgent.currentOffMeshLinkData.startPos + transform.up * NavMeshAgent.baseOffset;
            _moveEndPos = NavMeshAgent.currentOffMeshLinkData.endPos + transform.up * NavMeshAgent.baseOffset;
                        
            switch (NavMeshAgent.currentOffMeshLinkData.linkType)
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

            if (_hideTrace)
            {
                var renderers = GetComponentsInChildren<Renderer>();
                foreach (var item in renderers)
                {
                    item.enabled = false;
                }
            }            
        }

        void OnMoveInLink()
        {
            switch (NavMeshAgent.currentOffMeshLinkData.linkType)
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

            if (_moveProgress == 1)
            {
                OnExitLink();                
                NavMeshAgent.CompleteOffMeshLink();   // offMeshLink移动完成
            }

            _preIsOnOffMeshLink = NavMeshAgent.isOnOffMeshLink;
        }

        void OnExitLink()
        {
            switch (NavMeshAgent.currentOffMeshLinkData.linkType)
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

            if (_hideTrace)
            {
                var renderers = GetComponentsInChildren<Renderer>();
                foreach (var item in renderers)
                {
                    item.enabled = true;
                }

                _hideTrace = false;
            }
        }

        void LineMoving()
        {
            transform.position = Vector3.Lerp(_moveBeginPos, _moveEndPos, _moveProgress);
            _moveProgress += Time.deltaTime / _moveTime;
            if (_moveProgress > 1)
            {
                _moveProgress = 1;
            }
        }

        void ParabolaMoving(float height)
        {
            float yOffset = height * 4.0f * (_moveProgress - _moveProgress * _moveProgress);
            transform.position = Vector3.Lerp(_moveBeginPos, _moveEndPos, _moveProgress) + yOffset * Vector3.up;
            _moveProgress += Time.deltaTime / _moveTime;
            if (_moveProgress > 1)
            {
                _moveProgress = 1;
            }
        }

        void CurveMoving(AnimationCurve curve)
        {
            float yOffset = curve.Evaluate(_moveProgress);
            transform.position = Vector3.Lerp(_moveBeginPos, _moveEndPos, _moveProgress) + yOffset * Vector3.up;
            _moveProgress += Time.deltaTime / _moveTime;
            if (_moveProgress > 1)
            {
                _moveProgress = 1;
            }
        }

        void OnFall()
        {
            Debug.Log("Fall");
            _moveTime = GetMoveTime(_moveBeginPos, _moveEndPos, _fallData.speed);
            _hideTrace = (_fallData.traceType == NavMeshOffLinkMoveTrace.HIDE_TRACE);

            if (onFall != null)
            {
                onFall.Invoke(NavMeshAgent.currentOffMeshLinkData);
            }
        }

        void OnFalling()
        {
            // 自定义移动路径                        
            Moving(_fallData.traceType, _fallData.parabolaHeight, _fallData.curve);

            if (onFalling != null)
            {
                onFalling.Invoke(NavMeshAgent.currentOffMeshLinkData);
            }
        }

        void OnLand()
        {
            Debug.Log("Land");            

            if (onLand != null)
            {
                onLand.Invoke(NavMeshAgent.currentOffMeshLinkData);
            }
        }

        void OnJumpBegin()
        {
            Debug.Log("JumpBegin");
            _moveTime = GetMoveTime(_moveBeginPos, _moveEndPos, _jumpData.speed);
            _hideTrace = (_jumpData.traceType == NavMeshOffLinkMoveTrace.HIDE_TRACE);

            if (onJumpBegin != null)
            {
                onJumpBegin.Invoke(NavMeshAgent.currentOffMeshLinkData);
            }
        }

        void OnJumping()
        {
            // 自定义移动路径                        
            Moving(_jumpData.traceType, _jumpData.parabolaHeight, _jumpData.curve);

            if (onJumping != null)
            {
                onJumping.Invoke(NavMeshAgent.currentOffMeshLinkData);
            }
        }

        void OnJumpEnd()
        {
            Debug.Log("JumpEnd");            

            if (onJumpEnd != null)
            {
                onJumpEnd.Invoke(NavMeshAgent.currentOffMeshLinkData);
            }
        }

        void OnManualLinkBegin()
        {
            Debug.Log("ManualLinkBegin");
            _curManualLink = null;
            foreach (var item in _manualLinks)
            {
                // 根据offlink的area数值获得对应配置数据
                if (NavMesh.GetAreaFromName(item.areaName) == NavMeshAgent.currentOffMeshLinkData.offMeshLink.area)
                {
                    _curManualLink = item.data;
                    break;
                }
            }

            if (_curManualLink == null)
            {
                Debug.LogError(string.Format("fail to find the nav mesh offlink data for area {0}", 
                    NavMeshAgent.currentOffMeshLinkData.offMeshLink.area));
                return;
            }

            // 计算移动时长
            _moveTime = GetMoveTime(_moveBeginPos, _moveEndPos, _curManualLink.speed);
            _hideTrace = (_curManualLink.traceType == NavMeshOffLinkMoveTrace.HIDE_TRACE);

            if (onManualLinkBegin != null)
            {
                onManualLinkBegin.Invoke(NavMeshAgent.currentOffMeshLinkData);
            }
        }

        void OnManualLinkMoving()
        {
            // 自定义移动路径                        
            Moving(_curManualLink.traceType, _curManualLink.parabolaHeight, _curManualLink.curve);
            if (onManualLinkMoving != null)
            {
                onManualLinkMoving.Invoke(NavMeshAgent.currentOffMeshLinkData);
            }
        }

        void OnManualLinkEnd()
        {
            Debug.Log("ManualLinkEnd");            

            if (onManualLinkEnd != null)
            {
                onManualLinkEnd.Invoke(NavMeshAgent.currentOffMeshLinkData);
            }
        }

        void FlashMoving()
        {
            _moveProgress += Time.deltaTime / _moveTime;
            if (_moveProgress > 1)
            {
                _moveProgress = 1;
            }
        }

        void Moving(NavMeshOffLinkMoveTrace traceType, float parabolaHeight, AnimationCurve curve)
        {
            if (_moveProgress < 1.0f)
            {
                switch (traceType)
                {
                    case NavMeshOffLinkMoveTrace.LINE:
                        LineMoving();
                        break;
                    case NavMeshOffLinkMoveTrace.PARABOLA:
                        ParabolaMoving(parabolaHeight);
                        break;
                    case NavMeshOffLinkMoveTrace.CURVE:
                        CurveMoving(curve);
                        break;
                    case NavMeshOffLinkMoveTrace.HIDE_TRACE:
                        FlashMoving();
                        break;
                    default:
                        break;
                }
            }
        }

        float GetMoveTime(Vector3 beginPos, Vector3 endPos, float speed)
        {
            float ret = Vector3.Distance(beginPos, endPos) / speed;
            return Mathf.Min(Mathf.Max(ret, minMoveTime), maxMoveTime);
        }       
    }
}