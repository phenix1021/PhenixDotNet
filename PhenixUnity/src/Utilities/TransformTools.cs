using UnityEngine;
using Phenix.Unity.Pattern;
using System.Collections;

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
        /// target相对self朝向的角度
        /// </summary>        
        public float ForwardAngleToTarget(Transform self, Transform target)
        {
            if (self != null && target != null)
            {
                return Vector3.Angle(self.forward, target.position - self.position);
            }
            else
            {
                return Mathf.Infinity;
            }
        }

        /// <summary>
        /// self是否朝向target
        /// </summary>        
        public bool IsLookingAtTarget(Transform self, Transform target)
        {
            return ForwardAngleToTarget(self, target) <= 10;
        }

        /// <summary>
        /// target朝向相对self的角度
        /// </summary>        
        public float AngleToTargetForward(Transform self, Transform target)
        {
            if (self != null && target != null)
            {
                return Vector3.Angle(target.position - self.position, target.forward);
            }
            else
            {
                return Mathf.Infinity;
            }
        }

        /// <summary>
        /// target是否面向self（无论self朝向如何）
        /// </summary>        
        public bool IsAheadOfTarget(Transform self, Transform target)
        {
            if (self == null || target == null)
                return false;

            float angle = AngleToTargetForward(self, target);
            return angle > 135 && angle < 225;
        }

        /// <summary>
        /// target是否背对self（无论self朝向如何）
        /// </summary>        
        public bool IsBehindTarget(Transform self, Transform target)
        {
            if (self == null || target == null)
                return false;

            float angle = AngleToTargetForward(self, target);
            return angle > 315 || angle < 45;
        }

        public bool InRange(Transform trans, Vector3 center, float radius)
        {
            return (trans.position - center).sqrMagnitude < radius * radius;
        }

        public bool IsGroundThere(Vector3 pos)
        {
            return Physics.Raycast(pos + Vector3.up, -Vector3.up, 5, 1 << 10);
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
            if (trans == null || freq == 0 || smooth == 0)
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
            if (trans == null || speed == 0)
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
                step += speed * Time.deltaTime;
                trans.localScale = baseScale + deltaScalePercent * Mathf.Sin(step * Mathf.PI);
                yield return new WaitForEndOfFrame();
            }            
        }
    }
}