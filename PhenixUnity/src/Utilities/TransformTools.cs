﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phenix.Unity.Pattern;
using Phenix.Unity.Extend;

namespace Phenix.Unity.Utilities
{
    public class TransformTools : StandAloneSingleton<TransformTools>
    {
        /// <summary>
        /// 获取对象的bounds
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public Bounds GetBounds(GameObject go)
        {
            Bounds? bounds = null;
            foreach (Renderer mesh in go.GetComponentsInChildren<MeshRenderer>())
            {
                if (mesh.enabled == false || mesh.gameObject.activeInHierarchy == false)
                {
                    continue;
                }
                if (mesh.bounds.size.x == 0 || mesh.bounds.size.y == 0 || mesh.bounds.size.z == 0)
                {
                    continue;
                }
                if (bounds.HasValue == false)
                {
                    bounds = mesh.bounds;
                    continue;
                }
                bounds.Value.Encapsulate(mesh.bounds);
            }

            return bounds.Value;
        }

        /// <summary>
        /// 获取对象（们）的bounds
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public Bounds GetBounds(params GameObject[] goes)
        {
            Bounds? bounds = null;
            foreach (var item in goes)
            {
                if (bounds.HasValue == false)
                {
                    bounds = GetBounds(item);
                    continue;
                }
                bounds.Value.Encapsulate(GetBounds(item));
            }
            return bounds.Value;
        }

        /// <summary>
        /// 贴地移动
        /// </summary>        
        /// <returns>返回是否接触地面</returns>
        public bool MoveOnGround(CharacterController cc, Vector3 velocity)
        {
            return cc.SimpleMove(velocity); // CharacterController的SimpleMove自带重力，不受velocity的y值影响
        }

        /// <summary>
        /// 贴地移动
        /// </summary>        
        /// <returns>当侧面发生碰撞（可选）或离地时返回false</returns>        
        public bool MoveOnGround(Transform transform, CharacterController cc, Vector3 velocity, 
            bool allowSideCollision = false )
        {
            Vector3 old = transform.position;

            // -----为了保证始终和地面接触begin------
            transform.position += Vector3.up * Time.deltaTime;
            velocity.y -= 9 * Time.deltaTime;
            // -----为了保证始终和地面接触end------

            CollisionFlags flags = cc.Move(velocity); // CharacterController.Move无视重力，velocity的xyz轴均起作用

            if (allowSideCollision == false && (flags & CollisionFlags.Sides) != 0) // （flags & CollisionFlags.Sides) != 0 侧面碰撞
            {
                transform.position = old;
                return false;
            }

            if ((flags & CollisionFlags.Below) == 0) // (flags & CollisionFlags.Below) == 0 没有和地面碰撞
            {
                RaycastHit hit;
                // 向地面发射线，如果有碰撞，则下帧调用MoveOnGround会被velocity强制拉回地面
                if (Physics.Raycast(transform.position, -Vector3.up, out hit, 3) == false)
                {
                    transform.position = old;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 在XZ平面上，transform.forward与Vector3.forward的角度。返回值范围：[0, 360)
        /// </summary>        
        public float ForwardAngle360InXZ(Transform self)
        {            
            return self.forward.Angle360(Vector3.forward, Vector3.up);
        }

        /// <summary>
        /// 在XZ平面上，transform.forward与Vector3.forward的角度。返回值范围：（-180, 180]
        /// </summary>        
        public float ForwardSignedAngleInXZ(Transform self)
        {
            return self.forward.SignedAngle(Vector3.forward, Vector3.up);
        }

        /// <summary>
        /// target相对self朝向的角度
        /// </summary>        
        /*public float ForwardAngleToTarget(Transform self, Transform target)
        {
            if (self != null && target != null)
            {
                return Vector3.Angle(self.forward, target.position - self.position);
            }
            else
            {
                return Mathf.Infinity;
            }
        }*/

        /// <summary>
        /// forward相对forward的角度
        /// </summary>        
        public float ForwardAngleToForward(Transform self, Transform target)
        {
            if (self != null && target != null)
            {
                return Vector3.Angle(self.forward, target.forward);
            }
            else
            {
                return Mathf.Infinity;
            }
        }

        /// <summary>
        /// self是否朝向target
        /// </summary>        
        /*public bool IsLookingAtTarget(Transform self, Transform target)
        {
            return ForwardAngleToTarget(self, target) <= 10;
        }*/

        /// <summary>
        /// target朝向相对self的角度
        /// </summary>        
        /*public float AngleToTargetForward(Transform self, Transform target)
        {
            if (self != null && target != null)
            {
                return Vector3.Angle(target.position - self.position, target.forward);
            }
            else
            {
                return Mathf.Infinity;
            }
        }*/

        /// <summary>
        /// 是否面对面
        /// </summary>        
        public bool FaceToFace(Transform self, Transform target)
        {
            return IsAheadOfTarget(self, target) &&
                Vector3.Angle(self.forward, target.forward) >= 120;
        }

        /// <summary>
        /// 是否面对背
        /// </summary>        
        public bool FaceToBack(Transform self, Transform target)
        {
            return IsBehindTarget(self, target) &&
                Vector3.Angle(self.forward, target.forward) <= 60;
        }

        /// <summary>
        /// 是否面对侧
        /// </summary>        
        public bool FaceToSide(Transform self, Transform target)
        {
            float angle = Vector3.Angle(self.forward, target.forward);
            return IsBesideTarget(self, target) &&
                angle > 60 && angle < 120;
        }

        /// <summary>
        /// self是否位于target正面（无论self朝向如何）
        /// </summary>        
        public bool IsAheadOfTarget(Transform self, Transform target)
        {
            return Vector3.Angle(self.position - target.position, target.forward) <= 60;
            /*if (self == null || target == null)
                return false;

            float angle = AngleToTargetForward(self, target);
            return angle > 135 && angle < 225;*/
        }

        /// <summary>
        /// self是否位于target侧面（无论self朝向如何）
        /// </summary>
        public bool IsBesideTarget(Transform self, Transform target)
        {
            float angle = Vector3.Angle(self.position - target.position, target.forward);
            return angle > 60 && angle < 120;
        }

        /// <summary>
        /// self是否位于target背面（无论self朝向如何）
        /// </summary>        
        public bool IsBehindTarget(Transform self, Transform target)
        {
            return Vector3.Angle(self.position - target.position, target.forward) >= 120;
            /*if (self == null || target == null)
                return false;

            float angle = AngleToTargetForward(self, target);
            return angle > 315 || angle < 45;*/
        }

        public bool InRange(Transform trans, Vector3 center, float radius)
        {
            return (trans.position - center).sqrMagnitude < radius * radius;
        }

        public bool IsGroundThere(Vector3 pos, string layerName)
        {
            return Physics.Raycast(pos + Vector3.up, -Vector3.up, 5, 1 << LayerMask.NameToLayer(layerName));
        }

        public bool IsNearToLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd, float maxDist)
        {
            return MathTools.DistanceFromPointToVector(lineStart, lineEnd, point) <= maxDist;
        }

        /// <summary>
        /// 震动(适用于静止对象)
        /// </summary>        
        /// <param name="delay">延时</param>
        /// <param name="time">持续时长(0表示持久)</param>
        /// <param name="dir">振动方向</param>
        /// <param name="freq">震动频度</param>
        /// <param name="smooth">震动幅度</param>        
        public Coroutine Shake(Transform trans, Vector3 shakeDir, float delay, float time, float freq, float smooth)
        {
            if (trans == null || freq == 0 || smooth == 0 || trans.gameObject.activeInHierarchy == false)
            {
                return null;
            }
            return StartCoroutine(ShakeImpl(trans, shakeDir, delay, time, freq, smooth));
        }

        IEnumerator ShakeImpl(Transform trans, Vector3 shakeDir, float delay, float time, float freq, float smooth)
        {
            yield return new WaitForSecondsRealtime(delay);

            Vector3 oriPos = trans.localPosition;
            shakeDir = shakeDir.normalized;
            float extireTimer = (time == 0 ? 0 : Time.realtimeSinceStartup + time);

            while (extireTimer ==0 || Time.realtimeSinceStartup < extireTimer)
            {
                if (trans == null || trans.gameObject.activeInHierarchy == false)
                {
                    yield break;
                }

                float xNoise = (Mathf.PerlinNoise((Time.realtimeSinceStartup + 0) * 10 * freq, Time.smoothDeltaTime) * 2 - 1) * smooth;
                float yNoise = (Mathf.PerlinNoise((Time.realtimeSinceStartup + 50) * 10 * freq, Time.smoothDeltaTime) * 2 - 1) * smooth;
                float zNoise = (Mathf.PerlinNoise((Time.realtimeSinceStartup + 100) * 10 * freq, Time.smoothDeltaTime) * 2 - 1) * smooth;

                Vector3 offset = Vector3.Scale(shakeDir, new Vector3(xNoise, yNoise, zNoise));
                trans.localPosition = oriPos + offset;

                yield return new WaitForEndOfFrame();
            }

            trans.position = oriPos;
        }

        public void StopShake(Transform trans, Coroutine coroutine, Vector3 retorePos)
        {
            StopCoroutine(coroutine);
            trans.position = retorePos;
        }

        public Coroutine Blink(Transform trans, Vector3 deltaScalePercent, float speed, float delay = 0)
        {
            if (trans == null || speed == 0 || trans.gameObject.activeInHierarchy == false)
            {
                return null;
            }

            if (deltaScalePercent.x < 0 || trans.localScale.x < deltaScalePercent.x)
            {
                return null;
            }
            if (deltaScalePercent.y < 0 || trans.localScale.y < deltaScalePercent.y)
            {
                return null;
            }
            if (deltaScalePercent.z < 0 || trans.localScale.z < deltaScalePercent.z)
            {
                return null;
            }

            return StartCoroutine(BlinkImpl(trans, deltaScalePercent, speed, delay));
        }

        IEnumerator BlinkImpl(Transform trans, Vector3 deltaScalePercent, float speed, float delay)
        {
            yield return new WaitForSeconds(delay);
            float step = 0;
            Vector3 baseScale = trans.localScale;
            while (true)
            {
                if (trans == null || trans.gameObject.activeInHierarchy == false)
                {
                    yield break;
                }

                step += speed * Time.deltaTime;
                trans.localScale = baseScale + deltaScalePercent * Mathf.Sin(step * Mathf.PI);
                yield return new WaitForEndOfFrame();
            }            
        }

        /// <summary>
        /// 自旋
        /// </summary>
        /// <returns></returns>
        public Coroutine Spin(Transform trans, Vector3 axis/*旋转轴*/, float speed)
        {
            if (trans == null || trans.gameObject.activeInHierarchy == false)
            {
                return null;
            }

            return StartCoroutine(SpinImpl(trans, axis, speed));
        }

        IEnumerator SpinImpl(Transform trans, Vector3 axis, float speed)
        {
            while (true)
            {
                if (trans == null || trans.gameObject.activeInHierarchy == false)
                {
                    yield break;
                }

                trans.Rotate(axis, speed * Time.deltaTime * 10);

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// 获取CapsuleCollider上距离target最近的点
        /// </summary>        
        public Vector3 ClosestPoint(CapsuleCollider collider, Vector3 target)
        {            
            return collider.ClosestPointOnBounds(target);
        }

        /// <summary>
        /// 获取MeshCollider上距离target最近的点（不能用！没效果）
        /// </summary>        
        public Vector3 ClosestPoint(MeshCollider collider, Vector3 target)
        {
            return collider.ClosestPointOnBounds(target); // （不能用！没效果）
        }

        /// <summary>
        /// 获取BoxCollider上距离target最近的点
        /// </summary>        
        public Vector3 ClosestPoint(BoxCollider collider, Vector3 target)
        {            
            if (collider.transform.rotation == Quaternion.identity)
            {
                // AABB类型BoxCollider（即rotation的xyz都为0）
                return collider.ClosestPointOnBounds(target);
            }            
            // OBB类型碰撞盒
            return ClosestPointOBB(collider, target);
        }

        /// <summary>
        /// 获取SphereCollider上距离target最近的点
        /// </summary>        
        public Vector3 ClosestPoint(SphereCollider collider, Vector3 target)
        {
            Vector3 p;

            p = target - collider.transform.position;
            p.Normalize();

            p *= collider.radius * collider.transform.localScale.x;
            p += collider.transform.position;

            return p;
        }

        Vector3 ClosestPointOBB(BoxCollider collider, Vector3 target)
        {
            // Cache the collider transform
            var ct = collider.transform;

            // Firstly, transform the point into the space of the collider
            var local = ct.InverseTransformPoint(target);

            // Now, shift it to be in the center of the box
            local -= collider.center;

            // Inverse scale it by the colliders scale
            var localNorm =
            new Vector3(
            Mathf.Clamp(local.x, -collider.size.x * 0.5f, collider.size.x * 0.5f),
            Mathf.Clamp(local.y, -collider.size.y * 0.5f, collider.size.y * 0.5f),
            Mathf.Clamp(local.z, -collider.size.z * 0.5f, collider.size.z * 0.5f)
            );

            // Now we undo our transformations
            localNorm += collider.center;

            // Return resulting point
            return ct.TransformPoint(localNorm);
        }

        /*Vector3 ClosestPointOBB(BoxCollider collider, Vector3 target)这个版本的函数有bug，无效果
        {
            // Cache the collider transform
            var ct = collider.transform;

            // Firstly, transform the point into the space of the collider
            var local = ct.worldToLocalMatrix.MultiplyPoint3x4(target);

            // Now, shift it to be in the center of the box
            local -= collider.center;

            // Inverse scale it by the colliders scale
            var localNorm =
                new Vector3(
                    Mathf.Clamp(local.x, -collider.size.x, collider.size.x),
                    Mathf.Clamp(local.y, -collider.size.y, collider.size.y),
                    Mathf.Clamp(local.z, -collider.size.z, collider.size.z)
                );

            // Now we undo our transformations
            localNorm += collider.center;

            // Return resulting point
            return ct.localToWorldMatrix.MultiplyPoint3x4(localNorm);
        }*/

        /// <summary>
        /// 获得pointer指向的对象
        /// </summary>
        public GameObject GetPointerObject(LayerMask layerMask)
        {
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, layerMask))
            {
                return hitInfo.collider.gameObject;
            }
            return null;
        }

        /// <summary>
        /// 要将目标tar移动到pointer位置所获得的坐标值（世界坐标）
        /// </summary>        
        public Vector3 MoveToPointerPos(Transform tar)
        {
            return UnityEngine.Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                UnityEngine.Camera.main.WorldToScreenPoint(tar.position).z));
        }

        /// <summary>
        /// 世界坐标 => GUI坐标。注意：GUI坐标系是指GUI.Label等GUI系统的坐标，以左上角为远点。不同于UGUI坐标系的以pivot为原点。
        /// 补：屏幕坐标（左下角原点）、视口坐标（左下角原点且坐标值标准化）、GUI坐标（左上角原点）、UGUI坐标（pivot原点）
        /// </summary>        
        public Vector3 GetGUIPoint(Vector3 worldPos)
        {
            Vector3 screenPoint = UnityEngine.Camera.main.WorldToScreenPoint(worldPos);
            Vector3 guiPoint = new Vector3(screenPoint.x, UnityEngine.Camera.main.pixelHeight/*用Screen.height也行*/ - screenPoint.y, 
                screenPoint.z/*也可直接取0*/);            
            return guiPoint;
        }

        /// <summary>
        /// 获取球体表面随机点
        /// </summary>
        public Vector3 GetRandomPointOnSphere(Vector3 center, float radius)
        {
            return center + UnityEngine.Random.insideUnitSphere * radius;
        }

        /// <summary>
        /// 获取圆周上随机点
        /// </summary>
        public Vector2 GetRandomPointOnCircle(Vector2 center, float radius)
        {
            return center + UnityEngine.Random.insideUnitCircle * radius;
        }        
    }
}