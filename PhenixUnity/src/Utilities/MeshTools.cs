using UnityEngine;
using Phenix.Unity.Pattern;
using System.Collections;
using UnityEngine.Events;

namespace Phenix.Unity.Utilities
{
    public class MeshTools : StandAloneSingleton<TimeTools>
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
    }
}