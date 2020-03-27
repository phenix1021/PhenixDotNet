using UnityEngine;
using Phenix.Unity.Pattern;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Phenix.Unity.Utilities
{
    public class MeshTools : StandAloneSingleton<MeshTools>
    {
        /// <summary>
        /// 绘制mesh顶点。由于使用了Gizmos，所以需要在OnDrawGizmos或OnDrawGizmosSelected中执行
        /// </summary>
        public void DrawVerticles(Color color, float radius)
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf == null)
            {
                return;
            }
            Gizmos.color = color;
            foreach (var vert in mf.mesh.vertices)
            {
                // mesh顶点时局部坐标，这里需要转为世界坐标
                Vector3 worldPos = transform.TransformPoint(vert);
                Gizmos.DrawSphere(vert, radius);
            }                        
        }

        public void MakeMesh(UnityEngine.Mesh mesh, Vector3[] verts, int[] triangles, Vector2[] uvs = null, Color[] colors = null)
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf == null)
            {
                return;
            }

            if (verts.Length < 3)
            {
                return;
            }

            if (triangles.Length < 1)
            {
                return;
            }
            
            mesh.vertices = verts;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.colors = colors;

            mesh.RecalculateBounds();       // 重算bounds，bounds的size、extent、center常用
            mesh.RecalculateNormals();      // 重算法线
            mesh.RecalculateTangents();     // 重算切线

            mf.mesh = mesh;
        }

        public void DrawCurve(LineRenderer lineRenderer, List<Vector3> points,
            bool collisionAllowed = true)
        {
            lineRenderer.positionCount = points.Count;
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 curPos = points[i];
                lineRenderer.SetPosition(i, curPos);

                if (collisionAllowed && i+1 < points.Count)
                {
                    Vector3 nextPos = points[i+1];
                    Vector3 dir = (nextPos - curPos).normalized;
                    float distance = Vector3.Distance(nextPos, curPos);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(curPos, dir, out hitInfo, distance))
                    {
                        lineRenderer.SetPosition(i+1, hitInfo.point);
                        lineRenderer.positionCount = i+1;
                        break;
                    }
                }                         
            }
        }
    }
}