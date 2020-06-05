using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.Movement
{
    /// <summary>
    /// 路径管理（CatmullRom样条曲线）
    /// </summary>
    public class PathMgr : MonoBehaviour
    {
        public List<Vector3/*相对位置*/> points = new List<Vector3>(); // 路径点，注意存储的是相对transform的坐标 
        public bool loop;  // 路径是否闭合循环

        public float PathLength { get; private set; }   // 路径总长

        List<float> _lengthOfCurves = new List<float>(); // 路径相邻节点间curve长度列表

        /// <summary>
        /// 添加路径点
        /// </summary>        
        public void AddPoint(Vector3 point/*世界坐标*/)
        {
            point = transform.InverseTransformPoint(point);
            points.Add(point);
            PathLength = MathTools.GetCatmullRomSplineLength(points, loop, ref _lengthOfCurves);
        }

        /// <summary>
        /// 移除路径点
        /// </summary>
        public void RemovePoint(int idx)
        {
            if (idx >= points.Count || idx < 0)
            {
                return;
            }
            points.RemoveAt(idx);
            PathLength = MathTools.GetCatmullRomSplineLength(points, loop, ref _lengthOfCurves);
        }

        /// <summary>
        /// 清空路径点
        /// </summary>
        public void Clear()
        {
            points.Clear();
            PathLength = 0;
            _lengthOfCurves.Clear();
        }

        /// <summary>
        /// 获得指定进度下的运动位置和运动方向
        /// </summary>
        public bool GetPathPosAndMoveDir(float pathProgress, out Vector3 pos, out Vector3 moveDir)
        {
            if (MathTools.CatmullRomSpline(points, loop, pathProgress, out pos, out moveDir))
            {
                pos = transform.TransformPoint(pos);
                moveDir = transform.TransformDirection(moveDir);
                return true;
            }

            return false;
        }
    }
}