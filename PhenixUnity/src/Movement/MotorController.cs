using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Phenix.Unity.Utilities;
using Phenix.Unity.Mesh;

namespace Phenix.Unity.Movement
{
    [AddComponentMenu("Phenix/Movement/MotorController")]
    public class MotorController : MonoBehaviour
    {
        public class Force
        {
            public float moveTime;
            public float elapsedTime;
            public Action<Force> onDone;
            public Vector3 moveFrom;
            public Vector3 moveTo;
            //public bool forShift;
        }

        // 和target的接触面（不一定是狭义上的地面）
        public class Ground
        {
            public RaycastHit hit;
            public bool isTouching;
            public Vector3 targetOffset;  // target相对于Ground的偏移值

            public Vector3 Point { get { return hit.point; } }
            public Vector3 Normal { get { return hit.normal; } }
            public Transform Transform { get { return hit.transform; } }

            public Ground(RaycastHit hit, bool isTouching)
            {
                this.hit = hit;
                this.isTouching = isTouching;
            }
        }

        [Serializable]
        public class BoundSphere
        {
            public float offset;
            public float radius;
            public bool isFeet;
            public bool isHead;

            public BoundSphere() { }

            public BoundSphere(float offset, float radius, bool isFeet, bool isHead)
            {
                this.offset = offset;
                this.isFeet = isFeet;
                this.radius = radius;
                this.isHead = isHead;
            }
        }

        [SerializeField]
        Transform _target = null;

        public BoundSphere[] boundSpheres =
            new BoundSphere[3]
            {
                new BoundSphere(0.4f, 0.4f, true, false),
                new BoundSphere(0.75f, 0.4f, false, false),
                new BoundSphere(1.15f, 0.4f, false, true),
            };


        [SerializeField]
        float _fallClampEpsilon = 0.025f;

        [SerializeField]
        float _stepOffset = 0.2f;

        // 每帧运动模拟的间隔时长
        [SerializeField]
        float _simulationTimeOneFrame = 0.01f;

        [SerializeField]
        LayerMask _collidable = 0;        

        // 重力大小
        [SerializeField]
        Vector3 _gravityForce = new Vector3(0, -10, 0);

        [SerializeField]
        Vector3 _jumpForce = new Vector3(0, 20, 0);

        [SerializeField]
        float _jumpTime = 0.75f;

        // 是否允许二段跳
        [SerializeField]
        bool _doubleJumpEnabled = false;

        // 二段跳触发进度（即jump动作进度达到多少以上时才允许触发二段跳）
        [SerializeField]
        float _doubleJumpTriggerProgress = 0.8f;

        // 产生滑动的坡度（大于该值时target产生滑动）
        [SerializeField]
        float _slideSlopeAngle = 45f;

        [SerializeField]
        float _slideSpeedModifier = 0.5f;

        // 最小滑动距离
        [SerializeField]
        float _minSlideDistance = 0.5f;

        // 是否接受刚体碰撞反作用力
        [SerializeField]
        bool _rigidbodyForceEnabled = true;

        // 刚体碰撞反作用力时长
        [SerializeField]
        float _rigidbodyForceTime = 1f;

        // 刚体碰撞反作用力数值大小
        [SerializeField]
        float _rigidbodyForceValue = 1f;

        // 每帧输入的位移
        Vector3 _move;

        [HideInInspector]
        public float speed = 4f;

        float _lastGroundTouchTimer = 0f;   // 上一次触地时间点
        float _heightOnFall = 0;            // 坠落时的高度值

        // 过期的force列表
        HashSet<string> _expiredForces = new HashSet<string>();
        // force列表
        Dictionary<string, Force> _forces = new Dictionary<string, Force>();

        [SerializeField]
        bool displayConfigGizmos = true;

        [SerializeField]
        bool displayDebugInfo = true;

        [SerializeField]
        bool displayExtendedDebugInfo = true;

        public Action OnJump { get; set; }                          // 开始跳回调
        public Action OnDoubleJump { get; set; }                    // 开始二段跳回调
        public Action OnFall { get; set; }                          // 开始坠落回调
        public Action OnFalling { get; set; }                       // 坠落中回调
        public Action OnLand { get; set; }                          // 着陆回调
        public Action OnIdle { get; set; }
        public Action OnMove { get; set; }
        public Action OnSlide { get; set; }

        public bool IsJumping { get; private set; }
        public bool InDoubleJump { get; private set; }
        public Ground CurrentGround { get; private set; }
        public bool HasGround { get { return CurrentGround != null; } }
        public bool IsSliding { get; private set; }

        // 离开地面时长
        public float OffGroundTime { get { return Time.time - _lastGroundTouchTimer; } }
        // 已坠落的高度
        public float FallHeight { get { return _heightOnFall - _target.position.y; } }

        // 是否触地
        public bool IsOnGround { get { return HasGround && CurrentGround.isTouching; } }
        // 是否离地
        public bool IsOffGround { get { return HasGround && CurrentGround.isTouching == false; } }
        // 离地距离
        public float DistanceToGround { get { return HasGround ? Vector3.Distance(_target.position, CurrentGround.Point) : Mathf.Infinity; } }

        void Start()
        {
            if (_target == null)
            {
                _target = transform;
            }

            if (boundSpheres == null || boundSpheres.Length == 0)
            {
                enabled = false;
                Debug.LogError("[Moter] No spheres found, disabling");
            }

            bool foundFeet = false;
            bool foundHead = false;

            for (int i = 0; i < boundSpheres.Length; ++i)
            {
                if (boundSpheres[i] != null && boundSpheres[i].isFeet)
                {
                    if (foundFeet)
                    {
                        enabled = false;
                        Debug.LogError("[Moter] More then one feet sphere found, disabling");
                        return;
                    }

                    foundFeet = true;
                }

                if (boundSpheres[i] != null && boundSpheres[i].isHead)
                {
                    if (foundHead)
                    {
                        enabled = false;
                        Debug.LogError("[Moter] More then one head sphere found, disabling");
                        return;
                    }

                    foundHead = true;
                }
            }

            if (!foundFeet)
            {
                enabled = false;
                Debug.LogError("[Moter] No feet sphere found, disabling");
            }

            if (!foundHead)
            {
                enabled = false;
                Debug.LogError("[Moter] No head sphere found, disabling");
            }

            
            // 初始校正到地面上
            FindGround(_fallClampEpsilon);
            if (IsOnGround) // 如果现在在接触面
            {
                ClampToGround();             
                _lastGroundTouchTimer = Time.time;
            }
            else
            {
                //enabled = false;
                //Debug.LogError("[Moter] Fail to find ground, disabling");
                Debug.LogError("[Moter] Fail to find ground");
            }
        }

        void LateUpdate()
        {
            // If we have a ground and touching it, we should make sure to always follow it when it moves
            if (IsOnGround && CurrentGround.targetOffset != Vector3.zero)
            {
                // 使目标跟随接触面移动
                _target.position = CurrentGround.Transform.position + CurrentGround.targetOffset;
            }

            float remainSimulationTime = Time.deltaTime;

            // If delta is larger then maxSimulation time
            while (remainSimulationTime > _simulationTimeOneFrame)
            {                
                // Step simulation 根据模拟间隔单步模拟
                Step(_simulationTimeOneFrame);

                // Reduce total delta
                remainSimulationTime -= _simulationTimeOneFrame;
            }

            // If we have any remainig delta time
            if (remainSimulationTime > 0f)
            {
                // Step simulation
                Step(remainSimulationTime);
            }

            // Again, if we have a ground and we're touching it, store our offset
            if (IsOnGround)
            {
                // 重新计算目标对接触面的偏移
                CurrentGround.targetOffset = _target.position - CurrentGround.Transform.position;
            }

            // Clear input
            _move = Vector3.zero;
        }

        public void Move(Vector3 move)
        {
            _move = move;
        }

        public void Move(float x, float y, float z)
        {
            _move.x = x;
            _move.y = y;
            _move.z = z;
        }

        // 在XZ平面上绕Y轴旋转angle角度
        public void Rotate(float angle)
        {
            _target.Rotate(Vector3.up, angle);
        }

        // 设置XZ平面绕Y轴的旋转角度为angle
        public void SetRotationAngle(float angle)
        {
            _target.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        //public void Shift(Vector3 dstPos/*目标位置*/, float time/*移动时间*/)
        //{
        //    AddForce("Shift", _target.position, dstPos, time, true);
        //}

        //public void Shift(Vector3 dir/*移动方向*/, float distance/*移动距离*/, float time/*移动时间*/)
        //{
        //    AddForce("Shift", _target.position, _target.position + dir.normalized * distance, time, true);
        //}

        // 施加力
        public void AddForce(string name, Vector3 from/*初始大小*/, Vector3 to/*结束大小*/, float time/*力持续时长*//*, bool forShift = false*/)
        {
            AddForce(name, from, to, time, null/*, forShift*/);
        }

        // 施加力
        public void AddForce(string name, Vector3 from, Vector3 to, float time, Action<Force> onDone/*, bool forShift = false*/)
        {
            if (!String.IsNullOrEmpty(name))
            {
                _forces[name] = new Force { moveFrom = from, moveTo = to, moveTime = time, elapsedTime = 0, onDone = onDone/*, forShift = forShift*/ };
                // We need to remove any pending expires on this force
                _expiredForces.Remove(name);
            }
        }

        public void RemoveForce(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                _expiredForces.Add(name);
            }
        }

        public Force GetForce(string name)
        {
            Force force;

            if (_forces.TryGetValue(name, out force))
            {
                return force;
            }

            return null;
        }

        public bool Jump()
        {
            if (IsOnGround && IsJumping == false/* && IsSliding == false*/)
            {
                // So we can't jump untill we're done
                IsJumping = true;

                // Add jumping force
                AddForce("Jump", _jumpForce, Vector3.zero, _jumpTime, JumpDone);

                // If we are moving, we should add a movement force also
                if (_move != Vector3.zero)
                {
                    AddForce("JumpMomentum", _move.normalized * speed, _move.normalized * speed, _jumpTime);
                }

                Callback(OnJump);

                return true;
            }
            else if (_doubleJumpEnabled && IsJumping && InDoubleJump == false)
            {                
                Force jump = GetForce("Jump");
                float progress = jump.elapsedTime / jump.moveTime;                
                if (progress >= _doubleJumpTriggerProgress)
                {
                    InDoubleJump = true;

                    // 二段跳
                    AddForce("Jump", _jumpForce, Vector3.zero, _jumpTime, JumpDone);

                    Force jumpMomentum = GetForce("JumpMomentum");
                    if (jumpMomentum != null)
                    {
                        AddForce("JumpMomentum", jumpMomentum.moveFrom, jumpMomentum.moveFrom, _jumpTime);
                    }                    

                    Callback(OnDoubleJump);

                    return true;
                }
            }

            return false;
        }

        // 跳到最高点时的回调
        void JumpDone(Force force)
        {
            //Debug.LogWarning("JumpDone");
            Force jumpMomentum = GetForce("JumpMomentum");

            IsJumping = false;
            InDoubleJump = false;
            RemoveForce("Jump");
            RemoveForce("JumpMomentum");
            
            if (IsOnGround == false)
            {
                // 记录开始fall时的高度
                _heightOnFall = _target.position.y;
                //Debug.LogWarning("OnFall");
                Callback(OnFall);

                if (jumpMomentum != null)
                {
                    // 跳到最高点后的坠落前冲力
                    AddForce("FallMomentum", jumpMomentum.moveFrom, Vector3.zero, 1f);
                }
            }            
        }

        void Callback(Action action)
        {
            if (action != null)
            {
                action();
            }
        }

        void Callback<T>(Action<T> action, T param)
        {
            if (action != null)
            {
                action(param);
            }
        }

        void RemoveExpiredForces()
        {
            foreach (string name in _expiredForces)
            {
                if (_forces.Remove(name) && displayDebugInfo)
                {
                    Debug.Log("[Moter] Removing Force: " + name);
                }
            }

            _expiredForces.Clear();
        }

        void OnTriggerEnter(Collider collider)
        {
            HandleRigidForce(collider);
        }

        void OnTriggerStay(Collider collider)
        {
            HandleRigidForce(collider);
        }

        void HandleRigidForce(Collider collider)
        {
            if (_rigidbodyForceEnabled == false)
            {
                return;
            }

            if (collider.GetComponent<Rigidbody>() == null)
            {
                return;
            }

            if (((1 << collider.gameObject.layer) & (int)_collidable) != 0)
            {
                Vector3 rigidBodyVel = collider.GetComponent<Rigidbody>().velocity.normalized;
                Vector3 relativePos = (_target.position - collider.transform.position).normalized;

                // 碰撞的角度
                float colliderAngle = Vector3.Angle(rigidBodyVel, relativePos);

                if (colliderAngle < 90f)
                {
                    float t = (90f - colliderAngle) / 90f;
                    float m = collider.GetComponent<Rigidbody>().velocity.magnitude;

                    // 产生刚体碰撞反作用力
                    AddForce("Rigidbody#" + collider.GetComponent<Rigidbody>().GetInstanceID(), m * relativePos * _rigidbodyForceValue * t, Vector3.zero, _rigidbodyForceTime);
                }
            }
        }                    

        void OnDrawGizmos()
        {
            if (displayConfigGizmos)
            {
                if (boundSpheres != null)
                {
                    for (int i = 0; i < boundSpheres.Length; ++i)
                    {
                        Gizmos.color = boundSpheres[i].isFeet ? Color.green : (boundSpheres[i].isHead ? Color.yellow : Color.cyan);
                        Gizmos.DrawWireSphere(OffsetPosition(boundSpheres[i].offset), boundSpheres[i].radius);
                    }
                }
            }
        }

        void StopSliding()
        {
            if (IsSliding)
            {
                IsSliding = false;

                if (displayDebugInfo)
                {
                    Debug.Log("[Moter] Sliding Stopped");
                }
            }
        }

        // 处理沿接触面滑动，返回是否滑动
        bool HandleSlide(float time)
        {
            Vector3 normal = CurrentGround.Normal; // 接触面法线
            float angle = Vector3.Angle(normal, Vector3.up); // 和up轴的夹角            

            // If we're standing on a surface with an angle of more then 
            // slideAngle compared to the up vector we should slide downwards
            // 夹角大于滑动角度，引起滑动
            if (angle > _slideSlopeAngle)
            {                
                Vector3 tangent = Vector3.Cross(normal, Vector3.down);
                tangent = Vector3.Cross(tangent, normal);

                bool slide = true;

                RaycastHit hit;
                // 非滑动时，沿着接触点切线方向发射射线（滑动时则不再发射）
                if (IsSliding == false && Physics.Raycast(_target.position, tangent, out hit))
                {
                    if ((_target.position - hit.point).magnitude <= _minSlideDistance)
                    {
                        // 滑动距离太短，不滑动
                        StopSliding();
                        slide = false;
                    }
                }
                
                if (slide)
                {
                    // 计算滑动引起的位移
                    _target.position += (tangent * _gravityForce.magnitude * time * _slideSpeedModifier);

                    ClampToGround(_stepOffset);
                    Callback(OnSlide);

                    if (!IsSliding)
                    {
                        IsSliding = true;

                        if (displayDebugInfo)
                        {
                            Debug.Log("[Moter] Sliding Started");
                        }
                    }
                }
            }
            else
            {
                StopSliding();
            }

            return IsSliding;
        }

        void Step(float time)
        {
            HandleForces(time); // 计算施加在对象上的所有“力”使物体产生的位移

            if (IsOnGround) // 在接触面上
            {
                if (HandleSlide(time) == false) // 如果不在滑动状态
                {
                    if (IsJumping == false && _move != Vector3.zero)
                    {
                        HandleMove(time); // 处理移动（走、跑等输入）
                        Callback(OnMove);
                        // 如果Move后离地
                        if (IsOffGround)
                        {
                            // 移动中的坠落前冲力（比如从高地凌空走出）
                            AddForce("FallMomentum", _move.normalized * speed, Vector3.zero, 1f);
                            _heightOnFall = _target.position.y;
                            //Debug.LogWarning("OnFall");
                            Callback(OnFall);
                        }
                    }
                    else
                    {
                        if (IsJumping == false)
                        {
                            // 待机
                            Callback(OnIdle);
                        }

                        ClampToGround(_fallClampEpsilon);
                    }                    
                }
            }
            else // 离开接触面
            {
                // 处理重力影响
                //HandleGravity(time);

                //ClampToGround(_fallClampEpsilon);
                // 如果依旧无法触地
                if (IsOnGround == false)
                {
                    if (IsJumping == false)
                    {
                        // 处理重力影响
                        HandleGravity(time);                        
                        ClampToGround(_fallClampEpsilon);                        
                        Callback(OnFalling);
                    }                    
                }
            }

            FeedBack();
        }

        void FeedBack()
        {
            if (boundSpheres != null)
            {
                // 遍历自身所有碰撞球，若与外物有碰撞，则计算“力”反馈后的位置
                for (int i = 0; i < boundSpheres.Length; ++i)
                {
                    BoundSphere sphere = boundSpheres[i];

                    if (sphere == null)
                        continue;

                    foreach (Collider collider in Physics.OverlapSphere(OffsetPosition(sphere.offset), sphere.radius, _collidable))
                    {
                        Vector3 position = OffsetPosition(sphere.offset);
                        Vector3 contactPoint = Vector3.zero;                        

                        if (collider.gameObject == _target.gameObject)
                        {
                            continue; // 排除自身
                        }

                        if (collider.isTrigger)
                        {
                            continue;
                        }

                        if (collider is BoxCollider)
                        {
                            contactPoint = TransformTools.Instance.ClosestPoint((BoxCollider)collider, position);
                        }
                        else if (collider is SphereCollider)
                        {
                            contactPoint = TransformTools.Instance.ClosestPoint((SphereCollider)collider, position);
                        }                        
                        else if (collider is MeshCollider)
                        {
                            TriangleTreeMgr ttm = collider.GetComponent<TriangleTreeMgr>();
                            if (ttm == null)
                            {
                                ttm = collider.gameObject.AddComponent<TriangleTreeMgr>();                                
                            }                            
                            if (ttm != null)
                            {
                                contactPoint = ttm.ClosestPointOn(position, sphere.radius, displayDebugInfo, displayExtendedDebugInfo);
                            }
                            else
                            {
                                // Make last ditch try for convex colliders
                                MeshCollider mc = (MeshCollider)collider;
                                if (mc.convex)
                                {
                                    contactPoint = mc.ClosestPointOnBounds(position);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else if (collider is CapsuleCollider)
                        {                            
                            contactPoint = TransformTools.Instance.ClosestPoint((CapsuleCollider)collider, position);
                        }
                        else if (collider is TerrainCollider)
                        {
                            // 无须处理，地面和foot的碰撞检测在其它模块
                        }
                        else if (collider is WheelCollider)
                        {
                            Debug.LogWarning("[Moter] WheelColliders not supported");
                        }
                        else
                        {
                            continue;
                        }

                        if (contactPoint != Vector3.zero)
                        {
                            // If the sphere is feet
                            // We have a ground and we're touching
                            // And the sphere position is above the contact position
                            // We should ignore it
                            if (sphere.isFeet && IsOnGround && position.y > contactPoint.y)
                            {
                                continue;
                            }

                            // If this is the head sphere
                            // And we're jumping
                            // And we hit the "top" of the sphere, abort jumping
                            if (sphere.isHead && IsJumping && contactPoint.y > (position.y + sphere.radius * 0.25f))
                            {
                                JumpDone(null);
                            }

                            // Vector from contact point to position
                            Vector3 v = position - contactPoint;

                            // Draw debug line
                            if (displayDebugInfo)
                            {
                                Debug.DrawLine(position, contactPoint, Color.red);
                            }

                            // Display extend debug info
                            if (displayExtendedDebugInfo)
                            {
                                Debug.Log("[Moter] Contact point " + contactPoint);
                            }

                            // Move object away from collision
                            _target.position += Vector3.ClampMagnitude(v, Mathf.Clamp(sphere.radius - v.magnitude, 0, sphere.radius));
                        }
                    }
                }
            }
        }

        void HandleForces(float time)
        {
            RemoveExpiredForces();

            foreach (KeyValuePair<string, Force> kvp in _forces.Select(x => x).ToList())
            {
                // 
                Force force = kvp.Value;

                // Increase elapsed time
                force.elapsedTime += time;

                // If more then .Time has passed, we're done
                if (force.elapsedTime / force.moveTime >= 1f)
                {
                    if (force.onDone != null)
                    {
                        force.onDone(force);
                    }

                    _expiredForces.Add(kvp.Key);
                }
                //else if (force.forShift)
                //{
                //    _target.position = Vector3.Lerp(force.moveFrom, force.moveTo, force.elapsedTime / force.moveTime); // 会造成穿墙，why？？？
                //}
                else
                {
                    // Move our target
                    _target.Translate(Vector3.Lerp(force.moveFrom, force.moveTo, force.elapsedTime / force.moveTime) * time, Space.World);
                }
            }
        }

        void HandleGravity(float time)
        {
            _target.Translate(_gravityForce * time, Space.World);
        }

        void HandleMove(float time)
        {
            Vector3 oldPosition = _target.position;

            // Movement distance
            float moveDist = time * speed;

            // Move our character
            _target.Translate(_move.normalized * moveDist, Space.World);

            // Find a new ground
            ClampToGround(_stepOffset);

            // Make sure we still have a ground
            if (!HasGround)
            {
                return;
            }

            // The real movement
            Vector3 realMovement = _target.position - oldPosition;

            // If we need to clamp movement
            if (realMovement.magnitude > moveDist)
            {
                // Clamp our real movement
                realMovement = Vector3.ClampMagnitude(realMovement, moveDist);

                // Move our character our clamped distance
                _target.position = oldPosition + realMovement;

                // Fiind new ground
                ClampToGround(_stepOffset);
            }
        }

        void ClampToGround()
        {
            if (displayDebugInfo && displayExtendedDebugInfo && (_target.position.y - CurrentGround.Point.y) > 0)
            {
                Debug.Log("[Moter] Clamping to ground, distance: " + (_target.position.y - CurrentGround.Point.y));
            }

            _target.position = new Vector3(_target.position.x, CurrentGround.Point.y, _target.position.z);
        }

        bool FindGround(float clampEpsilon)
        {
            RaycastHit[] above;
            RaycastHit[] below;

            // 往上下两边寻找碰撞对象
            FindGroundPoints(_target.position, out above, out below);

            // 上边查找
            if (HasGround && above.Length > 0)
            {
                for (int i = ReferenceEquals(above[0].transform, CurrentGround.Transform) ? 1 : 0; i < above.Length; ++i)
                {
                    if ((_target.position - above[i].point).magnitude <= clampEpsilon)
                    {
                        // 只需要找到第一个足够近的碰撞物就跳出
                        CurrentGround = new Ground(above[i], true);
                        return true;
                    }
                }

                for (int i = 0; i < above.Length; ++i)
                {
                    // This means we have stepped through or fallen through a piece of ground
                    if (ReferenceEquals(above[i].transform, CurrentGround.Transform) && (_target.position - above[i].point).magnitude < 0.5)
                    {
                        CurrentGround = new Ground(above[i], true);
                        return true;
                    }
                }
            }

            // 下边查找
            if (below.Length > 0)
            {
                CurrentGround = new Ground(below[0], (_target.position - below[0].point).magnitude <= clampEpsilon);
                return true;
            }
            else
            {
                CurrentGround = null;
                return false;
            }
        }

        void ClampToGround(float clampEpsilon)
        {
            bool wasOffGround = IsOffGround;

            FindGround(clampEpsilon);

            if (IsOnGround) // 如果现在在接触面
            {
                if (wasOffGround) // 如果刚才在空中
                {
                    //Debug.LogWarning("OnLand");
                    Callback(OnLand);
                }

                if (IsJumping)
                {
                    // 如果还在跳跃阶段，强制完成
                    JumpDone(null);
                }

                ClampToGround();
                RemoveForce("FallMomentum");
                _lastGroundTouchTimer = Time.time;
            }
        }

        bool FindGroundPoints(Vector3 point, out RaycastHit[] above, out RaycastHit[] below)
        {
            Vector3 o = new Vector3(point.x, 2048f, point.z);
            Vector3 d = Vector3.down;

            RaycastHit hit;
            RaycastHit[] all = Physics.RaycastAll(o, d, 4096, _collidable);

            if (all.Length > 0)
            {
                List<RaycastHit> hits = new List<RaycastHit>();

                int maxIndex = all.Length - 1;

                for (int i = 0; i < all.Length; ++i)
                {
                    hit = all[i];
                    hits.Add(hit);

                    while (true)
                    {
                        o = hit.point;
                        o.y -= 0.1f;

                        if (i < maxIndex && all[i + 1].point.y > o.y)
                        {
                            break;
                        }

                        if (Physics.Raycast(o, d, out hit, 4096, _collidable))
                        {
                            if (i < maxIndex && all[i + 1].point.y >= hit.point.y && hit.transform == all[i + 1].transform)
                            {
                                break;
                            }

                            hits.Add(hit);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                above = hits.Where(x => x.point.y > point.y).OrderBy(x => (x.point - point).magnitude).ToArray();
                below = hits.Where(x => x.point.y <= point.y).OrderBy(x => (x.point - point).magnitude).ToArray();

                if (displayDebugInfo)
                {
                    Vector3 b = Vector3.back * 0.25f;
                    Vector3 f = Vector3.forward * 0.25f;

                    foreach (RaycastHit h in above)
                    {
                        Debug.DrawLine(h.point + b, h.point + f, Color.yellow);
                    }

                    foreach (RaycastHit h in below)
                    {
                        Debug.DrawLine(h.point + b, h.point + f, Color.green);
                    }
                }

                return hits.Count > 0;
            }

            above = new RaycastHit[0];
            below = new RaycastHit[0];
            return false;
        }

        Vector3 OffsetPosition(float y)
        {
            Vector3 p;

            if (_target == null)
            {
                p = transform.position;
            }
            else
            {
                p = _target.position;
            }

            p.y += y;

            return p;
        }

        public void Reset() // add by phenix
        {
            _forces.Clear();
            CurrentGround = null;
            _move = Vector3.zero;
        }
    }
}