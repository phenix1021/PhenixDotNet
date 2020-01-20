using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Phenix.Unity.Utilities;
using Phenix.Unity.Mesh;

namespace Phenix.Unity.Movement
{
    [AddComponentMenu("Phenix/Movement/Movement Controller")]
    public class MovementController : MonoBehaviour
    {
        public class Force
        {
            public float moveTime;
            public float elapsedTime;
            public Action<Force> onDone;
            public Vector3 moveFrom;
            public Vector3 moveTo;
        }

        public class Ground
        {
            public RaycastHit hit;
            public bool isTouching;
            public Vector3 offset;

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

            public BoundSphere()
            {

            }

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

        [SerializeField]
        float _maxSimulationTime = 0.01f;

        [SerializeField]
        LayerMask _collidable = 0;

        [SerializeField]
        bool _gravityEnabled = true;

        [SerializeField]
        Vector3 _gravity = new Vector3(0, -10, 0);

        [SerializeField]
        Vector3 _jumpForce = new Vector3(0, 20, 0);

        [SerializeField]
        float _jumpTime = 0.75f;

        [SerializeField]
        float _slideAngle = 45f;

        [SerializeField]
        float _slideSpeedModifier = 0.5f;

        [SerializeField]
        float _slideMagnitude = 0.5f;

        [SerializeField]
        bool _rigidbodyTransferEnabled = true;

        [SerializeField]
        float _rigidbodyTransferTime = 1f;

        [SerializeField]
        float _rigidbodyTransferAmount = 1f;

        [HideInInspector]
        public Vector3 moveInput;

        [HideInInspector]
        public float moveSpeed = 4f;

        float _lastGroundTouchTimer = 0f;

        HashSet<string> _expiredForces = new HashSet<string>();
        Dictionary<string, Force> _forces = new Dictionary<string, Force>();

        [SerializeField]
        bool displayConfigGizmos = true;

        [SerializeField]
        bool displayDebugInfo = true;

        [SerializeField]
        bool displayExtendedDebugInfo = true;

        public Action OnJump { get; set; }
        public Action OnLand { get; set; }
        public Action OnIdle { get; set; }
        public Action OnMove { get; set; }
        public Action OnFall { get; set; }
        public Action OnSlide { get; set; }

        public bool IsJumping { get; private set; }
        public float DeltaTime { get; private set; }
        public Ground CurrentGround { get; private set; }
        public bool HasGround { get { return CurrentGround != null; } }
        public bool IsSliding { get; private set; }
        public float GroundDelta { get { return Time.time - _lastGroundTouchTimer; } }
        public bool IsTouchingGround { get { return HasGround && CurrentGround.isTouching; } }

        public void Yaw(float angles)
        {
            _target.RotateAround(Vector3.up, angles);
        }

        public void SetYaw(float angles)
        {
            _target.rotation = Quaternion.Euler(0f, angles, 0f);
        }

        public void AddForce(string name, Vector3 from, Vector3 to, float time)
        {
            AddForce(name, from, to, time, null);
        }

        public void AddForce(string name, Vector3 from, Vector3 to, float time, Action<Force> onDone)
        {
            if (!String.IsNullOrEmpty(name))
            {
                _forces[name] = new Force { moveFrom = from, moveTo = to, moveTime = time, elapsedTime = 0, onDone = onDone };

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
            Force f;

            if (_forces.TryGetValue(name, out f))
            {
                return f;
            }

            return null;
        }

        public bool Jump()
        {
            if (IsTouchingGround && !IsJumping && !IsSliding)
            {
                // So we can't jump untill we're done
                IsJumping = true;

                // Add jumping force
                AddForce("Jump", _jumpForce, Vector3.zero, _jumpTime, JumpDone);

                // If we are moving, we should add a movement force also
                if (moveInput != Vector3.zero)
                {
                    AddForce("JumpMomentum", moveInput.normalized * 4f, moveInput.normalized * 4f, _jumpTime);
                }

                InvokeCallback(OnJump);

                return true;
            }

            return false;
        }

        void InvokeCallback(Action action)
        {
            if (action != null)
            {
                action();
            }
        }

        void RemoveExpiredForces()
        {
            foreach (string name in _expiredForces)
            {
                if (_forces.Remove(name) && displayDebugInfo)
                {
                    Debug.Log("[RPGMotor] Removing Force: " + name);
                }
            }

            _expiredForces.Clear();
        }

        void JumpDone(Force f)
        {
            Force jumpMomentum = GetForce("JumpMomentum");

            IsJumping = false;
            RemoveForce("Jump");
            RemoveForce("JumpMomentum");

            if ((!HasGround || !CurrentGround.isTouching) && jumpMomentum != null)
            {
                AddForce("FallMomentum", jumpMomentum.moveFrom, Vector3.zero, 1f);
            }
        }

        void OnTriggerStay(Collider c)
        {
            OnTriggerEnter(c);
        }

        void OnTriggerEnter(Collider c)
        {
            if (_rigidbodyTransferEnabled)
            {
                if (c.GetComponent<Rigidbody>() == null)
                {
                    return;
                }

                if (((1 << c.gameObject.layer) & (int)_collidable) != 0)
                {
                    Vector3 v = c.GetComponent<Rigidbody>().velocity.normalized;
                    Vector3 d = (_target.position - c.transform.position).normalized;

                    float a = Vector3.Angle(v, d);

                    if (a < 90f)
                    {
                        float t = (90f - a) / 90f;
                        float m = c.GetComponent<Rigidbody>().velocity.magnitude;

                        AddForce("Rigidbody#" + c.GetComponent<Rigidbody>().GetInstanceID(), m * d * _rigidbodyTransferAmount * t, Vector3.zero, _rigidbodyTransferTime);
                    }
                }
            }
        }

        void Start()
        {
            if (_target == null)
            {
                _target = transform;
            }

            if (boundSpheres == null || boundSpheres.Length == 0)
            {
                enabled = false;
                Debug.LogError("[RPGMotor] No spheres found, disabling");
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
                        Debug.LogError("[RPGMotor] More then one feet sphere found, disabling");
                        return;
                    }

                    foundFeet = true;
                }

                if (boundSpheres[i] != null && boundSpheres[i].isHead)
                {
                    if (foundHead)
                    {
                        enabled = false;
                        Debug.LogError("[RPGMotor] More then one head sphere found, disabling");
                        return;
                    }

                    foundHead = true;
                }
            }

            if (!foundFeet)
            {
                enabled = false;
                Debug.LogError("[RPGMotor] No feet sphere found, disabling");
            }

            if (!foundHead)
            {
                enabled = false;
                Debug.LogError("[RPGMotor] No head sphere found, disabling");
            }
        }

        void LateUpdate()
        {
            // If we have a ground and touching it, we should make sure to always follow it when it moves
            if (IsTouchingGround && CurrentGround.offset != Vector3.zero)
            {
                _target.position = CurrentGround.Transform.position + CurrentGround.offset;
            }

            float deltaTime = Time.deltaTime;

            // If delta is larger then maxSimulation time
            while (deltaTime > _maxSimulationTime)
            {
                // Set delta to max step
                DeltaTime = _maxSimulationTime;

                // Step simulation
                Step();

                // Reduce total delta
                deltaTime -= _maxSimulationTime;
            }

            // If we have any remainig delta time
            if (deltaTime > 0f)
            {
                // Set delta to remainder
                DeltaTime = deltaTime;

                // Step simulation
                Step();
            }

            // Again, if we have a ground and we're touching it, store our offset
            if (IsTouchingGround)
            {
                CurrentGround.offset = _target.position - CurrentGround.Transform.position;
            }

            // Clear input
            moveInput = Vector3.zero;
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
                    Debug.Log("[RPGMotor] Sliding Stopped");
                }
            }
        }

        bool CalculateSlide()
        {
            Vector3 normal = CurrentGround.Normal; // 接触面法线
            float angle = Vector3.Angle(normal, Vector3.up); // 和up轴的夹角
            bool slide = true;

            // If we're standing on a surface with an angle of more then 
            // slideAngle compared to the up vector we should slide downwards
            if (angle > _slideAngle)
            {
                // 夹角大于滑动角度，引起滑动
                Vector3 v = Vector3.Cross(normal, Vector3.down);
                v = Vector3.Cross(v, normal);

                RaycastHit hit;

                if (Physics.Raycast(_target.position, v, out hit) && !IsSliding)
                {
                    if ((_target.position - hit.point).magnitude <= _slideMagnitude)
                    {
                        // 距离太短，不滑动
                        StopSliding();
                        slide = false;
                    }
                }

                if (slide)
                {
                    // 计算滑动引起的位移
                    _target.position += (v * _gravity.magnitude * DeltaTime * _slideSpeedModifier);

                    FindGround(_stepOffset);
                    InvokeCallback(OnSlide);

                    if (!IsSliding)
                    {
                        IsSliding = true;

                        if (displayDebugInfo)
                        {
                            Debug.Log("[RPGMotor] Sliding Started");
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

        void Step()
        {
            CalculateForces(); // 计算施加在对象上的所有“力”使物体产生的位移

            if (IsTouchingGround)
            {
                if (!CalculateSlide())
                {
                    if (!IsJumping && moveInput != Vector3.zero)
                    {
                        CalculateMovement(); // 移动
                        InvokeCallback(OnMove);

                        if (IsFalling())
                        {
                            // 坠落
                            AddForce("FallMomentum", moveInput.normalized * moveSpeed, Vector3.zero, 1f);
                        }
                    }
                    else
                    {
                        InvokeCallback(OnIdle);
                        FindGround(_fallClampEpsilon);
                    }
                }
            }
            else // 离开接触面
            {
                if (_gravityEnabled)
                {
                    CalculateGravity();
                }

                FindGround(_fallClampEpsilon);

                if (!HasGround || !CurrentGround.isTouching)
                {
                    InvokeCallback(OnFall);
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
                            //Debug.LogWarning("[RPGMotor] CapsuleCollider not supported");
                            contactPoint = TransformTools.Instance.ClosestPoint((CapsuleCollider)collider, position);
                        }
                        else if (collider is TerrainCollider)
                        {
                            // 无须处理，地面和foot的碰撞检测在其它模块
                        }
                        else if (collider is WheelCollider)
                        {
                            Debug.LogWarning("[RPGMotor] WheelColliders not supported");
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
                            if (sphere.isFeet && IsTouchingGround && position.y > contactPoint.y)
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
                                Debug.Log("[RPGMotor] Contact point " + contactPoint);
                            }

                            // Move object away from collision
                            _target.position += Vector3.ClampMagnitude(v, Mathf.Clamp(sphere.radius - v.magnitude, 0, sphere.radius));
                        }
                    }
                }
            }
        }

        void CalculateForces()
        {
            RemoveExpiredForces();

            foreach (KeyValuePair<string, Force> kvp in _forces.Select(x => x).ToList())
            {
                // 
                Force f = kvp.Value;

                // Increase elapsed time
                f.elapsedTime += DeltaTime;

                // If more then .Time has passed, we're done
                if (f.elapsedTime / f.moveTime >= 1f)
                {
                    if (f.onDone != null)
                    {
                        f.onDone(f);
                    }

                    _expiredForces.Add(kvp.Key);
                }
                else
                {
                    // Move our target
                    _target.Translate(Vector3.Lerp(f.moveFrom, f.moveTo, f.elapsedTime / f.moveTime) * DeltaTime);
                }
            }
        }

        void CalculateGravity()
        {
            _target.Translate(_gravity * DeltaTime);
        }

        void CalculateMovement()
        {
            Vector3 oldPosition = _target.position;

            // Movement distance
            float moveDist = DeltaTime * moveSpeed;

            // Move our character
            _target.Translate(moveInput.normalized * moveDist);

            // Find a new ground
            FindGround(_stepOffset);

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
                FindGround(_stepOffset);
            }
        }

        void ClampToGround()
        {
            if (displayDebugInfo && displayExtendedDebugInfo && (_target.position.y - CurrentGround.Point.y) > 0)
            {
                Debug.Log("[RPGMotor] Clamping to ground, distance: " + (_target.position.y - CurrentGround.Point.y));
            }

            _target.position = new Vector3(_target.position.x, CurrentGround.Point.y, _target.position.z);
        }

        bool IsFalling()
        {
            return HasGround && !CurrentGround.isTouching;
        }

        void FindGround(float clampEpsilon)
        {
            bool wasFalling = IsFalling();

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
                        goto done;
                    }
                }

                for (int i = 0; i < above.Length; ++i)
                {
                    // This means we have stepped through or fallen through a piece of ground
                    if (ReferenceEquals(above[i].transform, CurrentGround.Transform) && (_target.position - above[i].point).magnitude < 0.5)
                    {
                        CurrentGround = new Ground(above[i], true);
                        goto done;
                    }
                }
            }

            // 下边查找
            if (below.Length > 0)
            {
                CurrentGround = new Ground(below[0], (_target.position - below[0].point).magnitude <= clampEpsilon);
            }
            else
            {
                CurrentGround = null;
            }

            done:

            if (IsTouchingGround)
            {
                if (wasFalling && OnLand != null)
                {
                    OnLand();
                }

                if (IsJumping)
                {
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
    }
}