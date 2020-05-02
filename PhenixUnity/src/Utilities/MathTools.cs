using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phenix.Unity.Utilities
{

    public class MathTools
    {
        public static float Hermite(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
        }

        public static Vector3 Hermite(Vector3 start, Vector3 end, float value)
        {
            return new Vector3(Hermite(start.x, end.x, value), 
                Hermite(start.y, end.y, value), 
                Hermite(start.z, end.z, value));
        }


        public static float Sinerp(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
        }

        public static Vector3 Sinerp(Vector3 start, Vector3 end, float value)
        {
            return new Vector3(Sinerp(start.x, end.x, value), 
                Sinerp(start.y, end.y, value), 
                Sinerp(start.z, end.z, value));
        }

        public static float Coserp(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
        }
        public static Vector3 Coserp(Vector3 start, Vector3 end, float value)
        {
            return new Vector3(Coserp(start.x, end.x, value), 
                Coserp(start.y, end.y, value), 
                Coserp(start.z, end.z, value));
        }

        public static float Berp(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * 
                Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }

        public static float SmoothStep(float x, float min, float max)
        {
            x = Mathf.Clamp(x, min, max);
            float v1 = (x - min) / (max - min);
            float v2 = (x - min) / (max - min);
            return -2 * v1 * v1 * v1 + 3 * v2 * v2;
        }

        public static float Lerp(float start, float end, float value)
        {
            return ((1.0f - value) * start) + (value * end);
        }

        public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
            float closestPoint = Vector3.Dot((lineStart - point), lineDirection) / 
                Vector3.Dot(lineDirection, lineDirection);
            return lineStart + (closestPoint * lineDirection);
        }

        /// <summary>
        /// 获得某点和线段的距离（即垂线长度）
        /// </summary>        
        public static float DistanceFromPointToVector(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            //Vector3 online = NearestPoint(lineStart, lineEnd, point);
            Vector3 online = NearestPointStrict(lineStart, lineEnd, point);
            /*Debug.DrawLine(lineStart, lineEnd, Color.red);
            Debug.DrawLine(lineStart, point, Color.green);
            Debug.DrawLine(lineEnd, point, Color.green);
            Debug.DrawLine(online, point, Color.yellow);
            Debug.Log("the len:" + (point - online).magnitude);
            Debug.Break();*/
            return (point - online).magnitude;
        }

        public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 fullDirection = lineEnd - lineStart;
            Vector3 lineDirection = Vector3.Normalize(fullDirection);
            float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / 
                Vector3.Dot(lineDirection, lineDirection);
            return lineStart + (Mathf.Clamp(closestPoint, 0.0f, Vector3.Magnitude(fullDirection)) * lineDirection);
        }

        public static float Bounce(float x)
        {
            return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
        }

        // test for value that is near specified float (due to floating point inprecision)
        // all thanks to Opless for this!
        public static bool Approx(float val, float about, float range)
        {
            return ((Mathf.Abs(val - about) < range));
        }

        // test if a Vector3 is close to another Vector3 (due to floating point inprecision)
        // compares the square of the distance to the square of the range as this 
        // avoids calculating a square root which is much slower than squaring the range
        public static bool Approx(Vector3 val, Vector3 about, float range)
        {
            return ((val - about).sqrMagnitude < range * range);
        }

        /*
          * CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
          * This is useful when interpolating eulerAngles and the object
          * crosses the 0/360 boundary.  The standard Lerp function causes the object
          * to rotate in the wrong direction and looks stupid. Clerp fixes that.
          */
        public static float Clerp(float start, float end, float value)
        {
            float min = 0.0f;
            float max = 360.0f;
            float half = Mathf.Abs((max - min) / 2.0f);//half the distance between min and max
            float retval = 0.0f;
            float diff = 0.0f;

            if ((end - start) < -half)
            {
                diff = ((max - start) + end) * value;
                retval = start + diff;
            }
            else if ((end - start) > half)
            {
                diff = -((max - end) + start) * value;
                retval = start + diff;
            }
            else retval = start + (end - start) * value;
            
            return retval;
        }

        public static Vector3 BezierSplinePos(Vector3[] path, float progress/*[0,1]*/)
        {
            float tmp = progress * path.Length;
            int curPathIdx = Mathf.Clamp(Mathf.FloorToInt(tmp), 0, path.Length - 1);
            float t = tmp - curPathIdx;

            Vector3 start;
            Vector3 end;
            Vector3 ctrl;

            if (curPathIdx == 0)
            {
                start = path[0];
                end = (path[1] + path[0]) * 0.5f;
                return Vector3.Lerp(start, end, t);
            }
            else if (curPathIdx == (path.Length - 1))
            {
                int pBound = path.Length - 1;
                start = (path[pBound - 1] + path[pBound]) * 0.5f;
                end = path[pBound];
                return Vector3.Lerp(start, end, t);
            }
            else
            {
                start = (path[curPathIdx - 1] + path[curPathIdx]) * 0.5f;
                end = (path[curPathIdx + 1] + path[curPathIdx]) * 0.5f;
                ctrl = path[curPathIdx];
                return BezierInterp(start, end, ctrl, t);
            }
        }

        public static Vector3 BezierSplineDerivative(Vector3[] path, float progress)
        {
            float tmp = progress * path.Length;
            int curPathIdx = Mathf.Clamp(Mathf.FloorToInt(tmp), 0, path.Length - 1);
            float t = tmp - curPathIdx;

            Vector3 start;
            Vector3 end;
            Vector3 ctrl;

            if (curPathIdx == 0)
            {
                start = path[0];
                end = (path[1] + path[0]) * 0.5f;
                return end - start;
            }
            else if (curPathIdx == (path.Length - 1))
            {
                int pBound = path.Length - 1;
                start = (path[pBound - 1] + path[pBound]) * 0.5f;
                end = path[pBound];
                return end - start;
            }
            else
            {
                start = (path[curPathIdx - 1] + path[curPathIdx]) * 0.5f;
                end = (path[curPathIdx + 1] + path[curPathIdx]) * 0.5f;
                ctrl = path[curPathIdx];
                return BezierDerivative(start, end, ctrl, t);
            }
        }

        static Vector3 BezierInterp(Vector3 start, Vector3 end, Vector3 ctrl, float t)
        {
            float d = 1.0f - t;
            return d * d * start + 2.0f * d * t * ctrl + t * t * end;
        }
        static Vector3 BezierDerivative(Vector3 start, Vector3 end, Vector3 ctrl, float t)
        {
            return (2.0f * start - 4.0f * ctrl + 2.0f * end) * t + 2.0f * ctrl - 2.0f * start;
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            return Mathf.Clamp(NormalizeAngle(angle), min, max);
        }

        public static float NormalizeAngle(float val)
        {
            while (val < -359)
            {
                val += 360;
            }
            while (val > 359)
            {
                val -= 360;
            }
            return val;
        }

        /// <summary>
        ///  获得在与axis垂直平面上，向量v1到v2的角度。返回值范围：（-180, 180]
        /// </summary>
        public static float SignedAngle(Vector3 v1, Vector3 v2, Vector3 axis)
        {
            return (float)Mathf.Atan2(Vector3.Dot(axis, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
        }

        /// <summary>
        /// 获得在与axis垂直平面上，向量v1到v2的角度。返回值范围：[0，360）
        /// </summary>
        public static float Angle360(Vector3 v1, Vector3 v2, Vector3 axis)
        {            
            return Mathf.Repeat(360f + SignedAngle(v1, v2, axis), 360f);
        }

        /// <summary>
        /// 四舍五入（指定小数位数）
        /// </summary>
        public static float Round(float value, int digits)
        {
            float mult = Mathf.Pow(10.0f, (float)digits);
            return Mathf.Round(value * mult) / mult;
        }

        public static void GetCatmullRomSplineFullPathPoints(List<Vector3> path, bool loop, 
            ref List<Vector3> fullPathPoints, int segmentCount = 100/*相邻路径节点之间的等分片段个数*/)
        {
            fullPathPoints.Clear();
            if (path.Count < 2 || segmentCount == 0)
            {
                return;
            }
            
            Vector3 retPos, retDir;
            int max = (loop ? path.Count : path.Count - 1);
            for (int i = 0; i < max; i++)
            {
                for (int ii = 0; ii < segmentCount; ii++)
                {
                    bool ret = CatmullRomSpline(path, loop, i, (ii * 1f) / segmentCount, out retPos, out retDir);
                    if (ret)
                    {
                        fullPathPoints.Add(retPos);
                    }
                }
            }
        }

        public static void DrawCatmullRomSpline(List<Vector3> path, bool loop, 
            LineRenderer lineRenderer, int segmentCount = 100/*相邻路径节点之间的等分片段个数*/)
        {
            if (path.Count < 2 || segmentCount == 0)
            {
                return;
            }

            List<Vector3> points = new List<Vector3>();
            Vector3 retPos, retDir;
            int max = (loop ? path.Count : path.Count - 1);
            for (int i = 0; i < max; i++)
            {
                for (int ii = 0; ii < segmentCount; ii++)
                {
                    bool ret = CatmullRomSpline(path, loop, i, (ii * 1f) / segmentCount, out retPos, out retDir);
                    if (ret)
                    {
                        points.Add(retPos);
                    }
                }
            }            

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }

        /// <summary>
        /// 获得CatmullRom样条各路径点之间的曲线长度和总长度
        /// </summary>
        /// <param name="path"></param>
        /// <param name="curveLength">各路径点之间的曲线长度</param>
        /// <returns></returns>
        public static float GetCatmullRomSplineLength(List<Vector3> path, bool loop, 
            ref List<float> curveLengthList)
        {            
            if (path.Count < 2)
            {
                return 0;
            }

            int segmentCount = 100;/*相邻路径节点之间的等分片段个数*/
            float totalLength = 0;
            curveLengthList.Clear();
            List<Vector3> points = new List<Vector3>();
            Vector3 retPos, retDir;
            int max = (loop ? path.Count : path.Count - 1);
            for (int i = 0; i < max; i++)
            {
                // 遍历每个路径点
                float curveLength = 0; // 每个curve的长度（由segmentCount个片段组成）
                Vector3 prePos = Vector3.zero;
                for (int ii = 0; ii <= segmentCount; ii++)
                {
                    // 遍历每个curve片段
                    CatmullRomSpline(path, loop, i, (ii * 1f) / segmentCount, out retPos, out retDir);
                    if (ii > 0)
                    {
                        curveLength += Vector3.Distance(prePos, retPos);
                    }
                    prePos = retPos;
                }

                curveLengthList.Add(curveLength);
                totalLength += curveLength;
            }

            return totalLength;
        }

        /// <summary>
        /// 求CatmullRom样条曲线上某点的位置和方向（即切线）
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="loop">是否循环</param>
        /// <param name="curPathIndex">当前所处节点序号</param>
        /// <param name="progressStartEnd">要获取的点在当前节点和下一节点之间的进度</param>
        public static bool CatmullRomSpline(List<Vector3> path, bool loop, int curPathIndex,
            float progressStartEnd/*[0, 1]*/, out Vector3 retPos, out Vector3 retDerivative)
        {
            // Get correct spline points
            Vector3 prev, start, end, next;
            bool ret = GetCatmullRomSplinePoints(path, curPathIndex, loop, out prev,
                out start, out end, out next);
            if (ret == false)
            {
                retPos = retDerivative = Vector3.zero;
                return false;
            }

            retPos = CatmullRomSplinePos(prev, start, end, next, progressStartEnd);
            retDerivative = CatmullRomSplineDerivative(prev, start, end, next, progressStartEnd);

            return true;
        }

        public static bool GetCatmullRomSplineProgress(List<Vector3> path, bool loop, float progress/*[0, 1]*/,
            out int pathIdx, out float progressStartEnd/*[0, 1]*/)
        {
            pathIdx = 0;
            progressStartEnd = 0;
            if (progress < 0 || progress > 1)
            {
                return false;
            }
            List<float> curveLengthList = new List<float>();
            float totalLength = GetCatmullRomSplineLength(path, loop, ref curveLengthList);
            if (totalLength == 0)
            {                
                return false;
            }

            float elapse = totalLength * progress;
            int i = 0;
            while (i < curveLengthList.Count && elapse > curveLengthList[i])
            {
                elapse -= curveLengthList[i];
                ++i;
            }

            if (i == curveLengthList.Count)
            {
                return false;
            }

            pathIdx = i;
            progressStartEnd = elapse / curveLengthList[i];

            return true;
        }

        /// <summary>
        /// 求CatmullRom样条曲线上某点的位置和方向（即切线）
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="loop">是否循环</param>
        /// <param name="progress">要获取的点在整个path中所处的长度进度</param>
        public static bool CatmullRomSpline(List<Vector3> path, bool loop, float progress/*[0, 1]*/, 
            out Vector3 retPos, out Vector3 retDerivative)
        {
            retPos = retDerivative = Vector3.zero;
            int pathIdx;
            float progressStartEnd;
            bool ret = GetCatmullRomSplineProgress(path, loop, progress, out pathIdx, out progressStartEnd);
            if (ret == false)
            {
                return false;
            }            
            return CatmullRomSpline(path, loop, pathIdx, progressStartEnd, out retPos, out retDerivative);            
        }

        /// <summary>
        /// 已知路径和当前所在路径节点序号，求计算CatmullRom需要的prev、start、end、next位置
        /// </summary>
        public static bool GetCatmullRomSplinePoints(List<Vector3> path, int curIndex, bool loop/*是否循环*/,
            out Vector3 prev, out Vector3 start, out Vector3 end, out Vector3 next)
        {
            int numPoints = path.Count;
            prev = start = end = next = Vector3.zero;
            if (numPoints < 1 || curIndex < 0 || curIndex >= numPoints)
            {
                return false;
            }

            start = path[curIndex];
            if (numPoints == 1)
            {
                // 如果只有一个路径点
                prev = next = end = start;
                return true;
            }

            if (loop) // ----------------- 循环曲线 -----------------------
            {
                prev = curIndex > 0/*当前不是第一个？*/ ? path[curIndex - 1] : path[numPoints - 1];
                end = curIndex < numPoints - 1/*当前不是最后一个？*/ ? path[curIndex + 1] : path[0];
                if (curIndex < numPoints - 2)
                {
                    // 如果当前小于倒数第二个
                    next = path[curIndex + 2];
                }
                else if (curIndex < numPoints - 1)
                {
                    // 如果当前位于倒数第二个
                    next = path[0];
                }
                else
                {
                    // 当前位于最后一个
                    next = path[1];
                }
            }
            else //---------------------- 非循环曲线 ----------------------------- 
            {
                if (curIndex == path.Count - 1)
                {
                    return false;
                }

                prev = (curIndex == 0 ? start : path[curIndex - 1]);                
                end = path[curIndex + 1];
                next = (curIndex < path.Count - 2 ? path[curIndex + 2] : end);
            }

            return true;
        }

        /// <summary>
        /// 获取CatmullRom样条曲线序列点中位于区间start、end之间、进度normalizedProgress的点位置
        /// </summary>
        /// <param name="previous">start之前的点</param>
        /// <param name="start">区间起点</param>
        /// <param name="end">区间终点，注意不一定是样条序列终点，可以是序列上任一点</param>
        /// <param name="next">end之后的点</param>
        /// <param name="progressStartEnd">start和end之间的进度，范围[0,1]</param>
        /// <returns>位置点</returns>
        public static Vector3 CatmullRomSplinePos(Vector3 previous, Vector3 start, Vector3 end, 
            Vector3 next, float progressStartEnd/*[0,1]*/)
        {
            // r = 0.5
            float progressSquared = progressStartEnd * progressStartEnd;
            float progressCubed = progressSquared * progressStartEnd;

            Vector3 result = previous * (-0.5f * progressCubed + progressSquared + -0.5f * progressStartEnd);
            result += start * (1.5f * progressCubed + -2.5f * progressSquared + 1.0f);
            result += end * (-1.5f * progressCubed + 2.0f * progressSquared + 0.5f * progressStartEnd);
            result += next * (0.5f * progressCubed + -0.5f * progressSquared);

            return result;
        }

        /// <summary>
        /// 获取CatmullRom样条曲线序列点中位于区间start、end之间、进度normalizedProgress的点方向（即该点沿曲线的切线）
        /// </summary>
        /// <param name="previous">start之前的点</param>
        /// <param name="start">区间起点</param>
        /// <param name="end">区间终点，注意不一定是样条序列终点，可以是序列上任一点</param>
        /// <param name="next">end之后的点</param>
        /// <param name="progressStartEnd">start和end之间的进度，范围[0,1]</param>
        /// <returns>位置点的方向（即切线方向）</returns>
        public static Vector3 CatmullRomSplineDerivative(Vector3 previous, Vector3 start, Vector3 end, 
            Vector3 next, float progressStartEnd/*[0,1]*/)
        {
            float progressSquared = progressStartEnd * progressStartEnd;

            Vector3 result = previous * (-1.5f * progressSquared + 2.0f * progressStartEnd + -0.5f);
            result += start * (4.5f * progressSquared + -5.0f * progressStartEnd);
            result += end * (-4.5f * progressSquared + 4.0f * progressStartEnd + 0.5f);
            result += next * (1.5f * progressSquared - progressStartEnd);

            return result;
        }
    }
}