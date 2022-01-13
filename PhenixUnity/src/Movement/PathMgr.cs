using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.Utilities;

namespace Phenix.Unity.Movement
{
    /// <summary>
    /// 道路模型
    /// </summary>
    [System.Serializable]
    public class RoadMeshData
    {
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        [Header("道路宽度")]
        public float roadWidth;
        [Header("道路分段数")]
        public int roadSegmentCount;
    }


    /// <summary>
    /// 路径管理（CatmullRom样条曲线）
    /// </summary>
    public class PathMgr : MonoBehaviour
    {
        public List<Vector3/*相对位置*/> points = new List<Vector3>(); // 路径点，注意存储的是相对transform的坐标 
        public bool loop;  // 路径是否闭合循环

        public float PathLength { get; private set; }   // 路径总长

        List<float> _lengthOfCurves = new List<float>(); // 路径相邻节点间curve长度列表
                
        public RoadMeshData roadMeshData = new RoadMeshData();        

        /*private void Start()
        {
            roadMeshData.meshFilter = GetComponent<MeshFilter>();
            if (roadMeshData.meshFilter == null)
            {
                roadMeshData.meshFilter = gameObject.AddComponent<MeshFilter>();
            }
            roadMeshData.meshRenderer = GetComponent<MeshRenderer>();
            if (roadMeshData.meshRenderer == null)
            {
                roadMeshData.meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }
        }*/

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

        // 构建道路模型
        public void BuildRoad()
        {
            if (roadMeshData == null || roadMeshData.meshFilter == null || roadMeshData.meshRenderer == null
                || roadMeshData.roadWidth <= 0 || roadMeshData.roadSegmentCount < 1)
            {
                return;
            }

            List<Vector3> posList = new List<Vector3>();
            List<Vector3> dirList = new List<Vector3>();
            for (int i = 0; i < roadMeshData.roadSegmentCount; i++)
            {
                Vector3 pos;
                Vector3 moveDir;
                GetPathPosAndMoveDir(i * 1.0f / (roadMeshData.roadSegmentCount - 1), out pos, out moveDir);
                posList.Add(pos);
                dirList.Add(moveDir);
            }

            MakeMesh(posList, dirList);
        }

        void MakeMesh(List<Vector3> posList, List<Vector3> dirList)
        {           
            Vector3[] vertices = new Vector3[posList.Count * 2];
            Vector2[] uv = new Vector2[posList.Count * 2];

            // Use matrix instead of transform.TransformPoint for performance reasons
            Matrix4x4 localSpaceTransform = transform.worldToLocalMatrix;

            // 创建顶点、UV
            for (var i = 0; i < posList.Count; i++)
            {
                Vector3 pos = posList[i];
                Vector3 right = Vector3.Cross(Vector3.up, dirList[i]).normalized;
                
                vertices[i * 2 + 0] = localSpaceTransform.MultiplyPoint(pos -
                    right * roadMeshData.roadWidth * 0.5f);
                vertices[i * 2 + 1] = localSpaceTransform.MultiplyPoint(pos +
                    right * roadMeshData.roadWidth * 0.5f);

                uv[i * 2 + 0] = new Vector2(0, i);
                uv[i * 2 + 1] = new Vector2(1, i);                
            }

            // 创建三角形
            int triangleCount = (posList.Count - 1) * 2;    // 三角形数量
            int trianglesVerticeCount = triangleCount * 3;  // 三角形顶点数量
            int[] triangles = new int[trianglesVerticeCount];
            int groupCount = triangles.Length / 6;
            for (int i = 0; i < groupCount; i++)
            {
                triangles[i * 6 + 0] = i * 2;
                triangles[i * 6 + 1] = i * 2 + 2;
                triangles[i * 6 + 2] = i * 2 + 1;

                triangles[i * 6 + 3] = i * 2 + 2;
                triangles[i * 6 + 4] = i * 2 + 3;
                triangles[i * 6 + 5] = i * 2 + 1;
            }

            MeshTools.MakeMesh(roadMeshData.meshFilter, vertices, triangles, uv);
        }
    }
}