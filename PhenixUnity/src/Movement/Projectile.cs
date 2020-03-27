using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Phenix.Unity.Movement
{
    // 预测击中回调
    [System.Serializable]
    public class PredictTargetHitEvent : UnityEvent<Projectile, GameObject/*击中的对象*/, Vector3/*击中点的世界坐标*/> { }

    // 击中回调
    [System.Serializable]
    public class TargetHitEvent : UnityEvent<Projectile, GameObject/*击中的对象*/, Vector3/*击中点的世界坐标*/> { }

    // 过期回调
    [System.Serializable]
    public class ExpireEvent : UnityEvent<Projectile> { }

    // 超出射程回调
    [System.Serializable]
    public class MaxRangeEvent : UnityEvent<Projectile> { }
    

    /// <summary>
    /// 投掷物，如子弹、导弹、炮弹等
    /// </summary>
    [AddComponentMenu("Phenix/Movement/Projectile")]
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        bool _active = false;
        Rigidbody _rigidBody;

        public float maxRange = 1000;   // 最大射程（米）
        //public float life = 0;          // 子弹持续时间(0表示持久)
        //float _expireTimer = 0;         // 过期时刻
        
        public LayerMask colliderLayerMask;   // 子弹碰撞只会检测该layerMask的layer              

        // 出膛速度
        public float muzzleSpeed = 0;

        // 出膛方向
        Vector3 _muzzleDirection = Vector3.zero;
        // 出膛位置
        Vector3 _muzzlePos = Vector3.zero;
        // 前一帧的位置
        Vector3 _prePos = Vector3.zero;

        // 击中时允许的最大偏移角度（由于重力弹道会往下坠引起与出膛角度的偏移值）
        // 角度偏转必须小于_maxAngleOffsetOnHit角度，否则视为无杀伤力，命中无效
        public float maxAngleOffsetOnHit = 60;

        // 风（风向(vector) * 风力(float)）
        public Vector3 wind = Vector3.zero;

        public PredictTargetHitEvent onPredictTargetHit;        // 预测命中的回调
        public TargetHitEvent onTargetHit;                      // 命中目标的回调
        //public ExpireEvent onExpire;                            // 子弹超时的回调
        public MaxRangeEvent onMaxRange;                        // 子弹超出射程的回调

        // 是否在发射前预测弹道
        public bool predictTrajectoryOnFire = true;
        // 预测弹道次数（相当于米）
        public int predictTrajectorySteps = 2000;
        // 是否在gizmos中绘制弹道轨迹
        public bool drawTrajectory = true;

        public Vector3 MuzzlePos { get { return _muzzlePos; } }

        List<Vector3> _track = new List<Vector3>();
        List<Vector3> _predictTrajectoryTrace = new List<Vector3>();

        Vector3 _hitPoint = Vector3.zero; // 击中的位置        

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (_active == false)
            {
                return;
            }

            // 添加风对刚体速度的额外作用
            _rigidBody.velocity += (wind * Time.fixedDeltaTime);
            // 将前进方向修正为刚体前进方向
            transform.forward = _rigidBody.velocity.normalized;
        }

        private void Update()
        {
            if (_active == false)
            {
                return;
            }

            // 是否在射程内
            if (Vector3.Distance(transform.position, _muzzlePos) > maxRange)
            {
                if (onMaxRange != null)
                {
                    // 触发子弹overRange的回调
                    onMaxRange.Invoke(this);
                }
                _active = false;
                ClearTrack();
                return;
            }

            // 是否过期
            //if (_expireTimer > 0 && Time.timeSinceLevelLoad > _expireTimer)
            //{
            //    if (onExpire != null)
            //    {
            //        // 触发子弹过期的回调
            //        onExpire.Invoke(this);
            //    }
            //    _active = false;
            //    ClearTrack();
            //    return;
            //}

            RaycastHit hitInfo;
            if (CheckHit(_prePos, transform.position, out hitInfo))
            {
                // 下坠角度校验
                if (CheckAngleOffset(hitInfo.point, _muzzlePos, _muzzleDirection, maxAngleOffsetOnHit))
                {
                    _hitPoint = hitInfo.point;
                    if (onTargetHit != null)
                    {
                        // 触发击中目标的回调
                        onTargetHit.Invoke(this, hitInfo.collider.gameObject, _hitPoint);
                    }
                    _active = false;
                    return;
                }                
            }

            _prePos = transform.position;            

            if (drawTrajectory)
            {
                AddToTrack();
            }            
        }

        public void Fire()
        {
            _active = true;
            //_expireTimer = life > 0 ? Time.timeSinceLevelLoad + life : 0;
            _prePos = _muzzlePos = transform.position;
            _muzzleDirection = transform.forward;
            _track.Clear();
            if (drawTrajectory)
            {
                AddToTrack();
            }

            if (predictTrajectoryOnFire)
            {
                // 预测弹道
                GameObject hitObj;
                Vector3 hitPoint;
                PredictTrajectory(out hitObj, out hitPoint);
            }

            // 刚体速度
            _rigidBody.velocity = _muzzleDirection * muzzleSpeed/* * Time.deltaTime * 62*/; // 发射            
        }

        /// <summary>
        /// 预测弹道是否击中，用于“子弹时间”特效的触发以及射击前瞄准镜中的辅助显示
        /// </summary>
        public static bool PredictTrajectory(Vector3 shootPos, Vector3 shootDir, float shootSpeed,
            bool useGravity, Vector3 wind, LayerMask colliderLayerMask, int predictTrajectorySteps, 
            float maxRange, float maxAngleOffsetOnHit, out GameObject hitObj, out Vector3 hitPoint,
            ref List<Vector3> trace)
        {
            Vector3 gravity = useGravity ? Physics.gravity : Vector3.zero;
            int numSteps = predictTrajectorySteps;
            float timeDelta = 1.0f / shootSpeed; // 移动1米的时间，numSteps即移动numSteps米

            Vector3 lastpos = shootPos;
            Vector3 position = shootPos;
            Vector3 velocity = shootDir * shootSpeed;

            trace.Clear();
            for (int i = 0; i < numSteps; ++i)
            {
                trace.Add(position);
                // 中学位移公式 s = v0t + at*t/2
                position += velocity * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
                velocity += (gravity * timeDelta) + (wind * timeDelta);
                if (RayPrediction(lastpos, position, colliderLayerMask, maxRange, shootPos,
                    shootDir, maxAngleOffsetOnHit, out hitObj, out hitPoint))
                {
                    trace.Add(hitPoint);
                    return true;
                }
                lastpos = position;
            }

            hitObj = null;
            hitPoint = Vector3.zero;
            return false;
        }

        static bool RayPrediction(Vector3 lastpos, Vector3 currentpos, LayerMask colliderLayerMask,
            float maxRange, Vector3 shootPos, Vector3 shootDir, float maxAngleOffsetOnHit,
            out GameObject hitObj, out Vector3 hitPoint)
        {
            Vector3 dir = (currentpos - lastpos);
            dir.Normalize();
            RaycastHit hitInfo;
            if (Physics.Raycast(lastpos, dir, out hitInfo, 1, colliderLayerMask))
            {
                // 是否射程之内
                if (Vector3.Distance(hitInfo.point, shootPos) <= maxRange)
                {
                    // 倾斜角是否过大
                    if (CheckAngleOffset(hitInfo.point, shootPos, shootDir, maxAngleOffsetOnHit))
                    {
                        hitObj = hitInfo.collider.gameObject;
                        hitPoint = hitInfo.point;
                        return true;
                    }                    
                }
            }

            hitObj = null;
            hitPoint = Vector3.zero;
            return false;
        }

        void PredictTrajectory(out GameObject hitObj, out Vector3 hitPoint)
        {
            bool ret = PredictTrajectory(_muzzlePos, _muzzleDirection, muzzleSpeed,
            _rigidBody.useGravity, wind, colliderLayerMask, predictTrajectorySteps, 
            maxRange, maxAngleOffsetOnHit, out hitObj, out hitPoint, ref _predictTrajectoryTrace);

            if (ret)
            {
                if (onPredictTargetHit != null)
                {
                    onPredictTargetHit.Invoke(this, hitObj, hitPoint);
                }
            }
        }

        /*
        /// <summary>
        /// 预测弹道是否击中，方便“子弹时间”特效的显示
        /// </summary>
        public void PredictTrajectory(out GameObject hitObj, out Vector3 hitPoint)
        {
            Vector3 gravity = Vector3.zero;
            if (_rigidBody.useGravity)
            {
                gravity = Physics.gravity;
            }
            int numSteps = predictTrajectorySteps;
            float timeDelta = 1.0f / muzzleSpeed; // 移动1米的时间，numSteps即移动numSteps米

            Vector3 lastpos = _muzzlePos;
            Vector3 position = _muzzlePos;
            Vector3 velocity = _muzzleDirection * muzzleSpeed;            
            
            for (int i = 0; i < numSteps; ++i)
            {
                // 中学位移公式 s = v0t + at*t/2
                position += velocity * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
                velocity += (gravity * timeDelta) + (wind * timeDelta);
                if (RayPrediction(lastpos, position, out hitObj, out hitPoint))
                {
                    return;
                }                
                lastpos = position;
            }

            hitObj = null;
            hitPoint = Vector3.zero;
        }

        bool RayPrediction(Vector3 lastpos, Vector3 currentpos,
            out GameObject hitObj, out Vector3 hitPoint)
        {   
            Vector3 dir = (currentpos - lastpos);
            dir.Normalize();
            RaycastHit hitInfo;
            if (Physics.Raycast(lastpos, dir, out hitInfo, 1, _colliderLayerMask))
            {
                // 是否射程之内
                if (Vector3.Distance(hitInfo.point, _muzzlePos) <= maxRange)
                {
                    hitObj = hitInfo.collider.gameObject;
                    hitPoint = hitInfo.point;
                    if (onPredictTargetHit != null)
                    {
                        onPredictTargetHit.Invoke(this, hitInfo.collider.gameObject, hitInfo.point);
                    }
                    return true;
                }                
            }

            hitObj = null;
            hitPoint = Vector3.zero;
            return false;
        }*/

        // 添加进弹道轨迹
        void AddToTrack()
        {
            _track.Add(transform.position);
        }

        void ClearTrack()
        {
            _track.Clear();
        }

        // 是否碰撞
        bool CheckHit(Vector3 from, Vector3 to, out RaycastHit hitInfo)
        {
            hitInfo = default(RaycastHit);
            float range = Vector3.Distance(from, to);
            if (range < 0.001f)
            {                
                return false;
            }
            
            return (Physics.Raycast(from, (to - from).normalized, out hitInfo, range, colliderLayerMask) &&
                hitInfo.collider.gameObject != gameObject);   
        }

        // 检测子弹向下的角度偏转
        static bool CheckAngleOffset(Vector3 pos, Vector3 shootPos, Vector3 shootDir,
            float maxAngleOffsetOnHit)
        {
            /*子弹向下的角度偏转必须小于_maxAngleOffsetOnHit角度，否则视为无杀伤力，命中无效*/
            return Vector3.Dot((pos - shootPos).normalized, shootDir) >
                Mathf.Cos(maxAngleOffsetOnHit);
        }

        
        // 检测子弹向下的角度偏转
        //bool CheckAngleOffset(Vector3 pos)
        //{
        //    /*子弹向下的角度偏转必须小于_maxAngleOffsetOnHit角度，否则视为无杀伤力，命中无效*/
        //    return Vector3.Dot((pos - _muzzlePos).normalized, _muzzleDirection) > 
        //        Mathf.Cos(_maxAngleOffsetOnHit);
        //}

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;            
            for (int i = 0; i < _track.Count - 1; i++)
            {                
                // 绘制弹道轨迹
                Gizmos.DrawLine(_track[i], _track[i + 1]);
            }

            if (_hitPoint != Vector3.zero)
            {
                // 绘制命中的目标点
                Gizmos.DrawWireSphere(_hitPoint, 0.1f);
            }
        }
    }
}
