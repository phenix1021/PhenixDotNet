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

        public static void MakeMesh(UnityEngine.Mesh mesh, Vector3[] verts,
            int[] triangles, Vector2[] uvs = null, Color[] colors = null)
        {
            if (mesh == null)
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

            mesh.Clear();

            mesh.vertices = verts;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.colors = colors;

            mesh.RecalculateBounds();       // 重算bounds，bounds的size、extent、center常用
            mesh.RecalculateNormals();      // 重算法线
            mesh.RecalculateTangents();     // 重算切线
        }

        public static void MakeMesh(MeshFilter meshFilter, Vector3[] verts, 
            int[] triangles, Vector2[] uvs = null, Color[] colors = null)
        {
            if (meshFilter == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                if (meshFilter.mesh == null)
                {
                    meshFilter.mesh = new UnityEngine.Mesh();
                }
                MakeMesh(meshFilter.mesh, verts, triangles, uvs, colors);
            }
            else
            {
                if (meshFilter.sharedMesh == null)
                {
                    meshFilter.sharedMesh = new UnityEngine.Mesh();
                }
                MakeMesh(meshFilter.sharedMesh, verts, triangles, uvs, colors);
            }
        }

        /// <summary>
        /// 创建曲线
        /// </summary>
        /// <param name="lineRenderer"></param>
        /// <param name="points"></param>
        /// <param name="checkCollision"></param>
        public void DrawCurve(LineRenderer lineRenderer, List<Vector3> points,
            bool checkCollision = true)
        {
            lineRenderer.positionCount = points.Count;
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 curPos = points[i];
                lineRenderer.SetPosition(i, curPos);

                if (checkCollision && i+1 < points.Count)
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

        /// <summary>
        /// 创建直线
        /// </summary>
        /// <param name="lineRenderer"></param>
        /// <param name="points"></param>
        /// <param name="checkCollision"></param>
        public void DrawLine(LineRenderer lineRenderer, Vector3 start, Vector3 end, int pointCount, bool checkCollision = true)
        {
            if (pointCount < 2)
            {
                pointCount = 2;
            }

            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < pointCount; i++)
            {
                points.Add(Vector3.Lerp(start, end, i * 1f / (pointCount - 1)));
            }            

            lineRenderer.positionCount = points.Count;
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 curPos = points[i];
                lineRenderer.SetPosition(i, curPos);

                if (checkCollision && i + 1 < points.Count)
                {
                    Vector3 nextPos = points[i + 1];
                    Vector3 dir = (nextPos - curPos).normalized;
                    float distance = Vector3.Distance(nextPos, curPos);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(curPos, dir, out hitInfo, distance))
                    {
                        lineRenderer.SetPosition(i + 1, hitInfo.point);
                        lineRenderer.positionCount = i + 1;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 创建圆锥体
        /// </summary>
        public UnityEngine.Mesh MakeConeByAngle(                        
            float openingAngle = 30f,   // 圆锥体顶角
            float length = 1f,          // 底面到顶点的长度
            int numVertices = 10,       // 底面顶点数（越大越接近圆）            
            bool outside = true,
            bool inside = false)
        {
            float radiusBottom = length * Mathf.Tan(openingAngle * Mathf.Deg2Rad / 2);
            return MakeConeOrFrustum(numVertices, 0, radiusBottom, length, openingAngle, outside, inside);
        }

        /// <summary>
        /// 创建圆锥体
        /// </summary>
        public UnityEngine.Mesh MakeConeByRadius(
            float bottomRadius = 1f,          // 底面半径
            float length = 1f,          // 底面到顶点的长度            
            int numVertices = 10,       // 底面顶点数（越大越接近圆）            
            bool outside = true,
            bool inside = false)
        {
            return MakeConeOrFrustum(numVertices, 0, bottomRadius, length, 0, outside, inside);
        }

        /// <summary>
        /// 创建截锥体
        /// </summary>
        public UnityEngine.Mesh MakeFrustum(
            int numVertices = 10,       // 上下截面各自顶点数
            float radiusTop = 1f,       // 截锥体上截面半径
            float radiusBottom = 1f,    // 截锥体下截面半径
            float length = 1f,          // 上下截面之间的长度            
            bool outside = true,
            bool inside = false)
        {
            return MakeConeOrFrustum(numVertices, radiusTop, radiusBottom, length, 0, outside, inside);
        }

        /// <summary>
        /// 创建圆锥体/截锥体
        /// </summary>
        public static UnityEngine.Mesh MakeConeOrFrustum(
            int numVertices = 10,       // 上下截面各自顶点数
            float radiusTop = 0f,       // 截锥体上截面半径
            float radiusBottom = 1f,    // 截锥体下截面半径
            float length = 1f,          // 圆锥体/截锥体的长度
            float openingAngle = 0f,    // if > 0, create a cone with this angle by setting radiusTop to 0, and adjust radiusBottom according to length;
            bool outside = true,
            bool inside = false)
        {
            UnityEngine.Mesh mesh = new UnityEngine.Mesh();
            
            int multiplier = (outside ? 1 : 0) + (inside ? 1 : 0);
            int offset = (outside && inside ? 2 * numVertices : 0);
            Vector3[] vertices = new Vector3[2 * multiplier * numVertices]; // 0..n-1: top, n..2n-1: bottom
            Vector3[] normals = new Vector3[2 * multiplier * numVertices];
            Vector2[] uvs = new Vector2[2 * multiplier * numVertices];
            int[] tris;
            float slope = Mathf.Atan((radiusBottom - radiusTop) / length); // (rad difference)/height
            float slopeSin = Mathf.Sin(slope);
            float slopeCos = Mathf.Cos(slope);
            int i;

            for (i = 0; i < numVertices; i++)
            {
                float angle = 2 * Mathf.PI * i / numVertices;
                float angleSin = Mathf.Sin(angle);
                float angleCos = Mathf.Cos(angle);
                float angleHalf = 2 * Mathf.PI * (i + 0.5f) / numVertices; // for degenerated normals at cone tips
                float angleHalfSin = Mathf.Sin(angleHalf);
                float angleHalfCos = Mathf.Cos(angleHalf);

                vertices[i] = new Vector3(radiusTop * angleCos, radiusTop * angleSin, 0);
                vertices[i + numVertices] = new Vector3(radiusBottom * angleCos, radiusBottom * angleSin, length);

                if (radiusTop == 0)
                    normals[i] = new Vector3(angleHalfCos * slopeCos, angleHalfSin * slopeCos, -slopeSin);
                else
                    normals[i] = new Vector3(angleCos * slopeCos, angleSin * slopeCos, -slopeSin);
                if (radiusBottom == 0)
                    normals[i + numVertices] = new Vector3(angleHalfCos * slopeCos, angleHalfSin * slopeCos, -slopeSin);
                else
                    normals[i + numVertices] = new Vector3(angleCos * slopeCos, angleSin * slopeCos, -slopeSin);

                uvs[i] = new Vector2(1.0f * i / numVertices, 1);
                uvs[i + numVertices] = new Vector2(1.0f * i / numVertices, 0);

                if (outside && inside)
                {
                    // vertices and uvs are identical on inside and outside, so just copy
                    vertices[i + 2 * numVertices] = vertices[i];
                    vertices[i + 3 * numVertices] = vertices[i + numVertices];
                    uvs[i + 2 * numVertices] = uvs[i];
                    uvs[i + 3 * numVertices] = uvs[i + numVertices];
                }

                if (inside)
                {
                    // invert normals
                    normals[i + offset] = -normals[i];
                    normals[i + numVertices + offset] = -normals[i + numVertices];
                }
            }            

            // create triangles
            // here we need to take care of point order, depending on inside and outside
            int cnt = 0;
            if (radiusTop == 0)
            {
                // top cone
                tris = new int[numVertices * 3 * multiplier];
                if (outside)
                {
                    for (i = 0; i < numVertices; i++)
                    {
                        tris[cnt++] = i + numVertices;
                        tris[cnt++] = i;
                        if (i == numVertices - 1)
                            tris[cnt++] = numVertices;
                        else
                            tris[cnt++] = i + 1 + numVertices;
                    }
                }

                if (inside)
                {
                    for (i = offset; i < numVertices + offset; i++)
                    {
                        tris[cnt++] = i;
                        tris[cnt++] = i + numVertices;
                        if (i == numVertices - 1 + offset)
                            tris[cnt++] = numVertices + offset;
                        else
                            tris[cnt++] = i + 1 + numVertices;
                    }
                }
            }
            else if (radiusBottom == 0)
            {
                // bottom cone
                tris = new int[numVertices * 3 * multiplier];
                if (outside)
                {
                    for (i = 0; i < numVertices; i++)
                    {
                        tris[cnt++] = i;
                        if (i == numVertices - 1)
                            tris[cnt++] = 0;
                        else
                            tris[cnt++] = i + 1;
                        tris[cnt++] = i + numVertices;
                    }
                }

                if (inside)
                {
                    for (i = offset; i < numVertices + offset; i++)
                    {
                        if (i == numVertices - 1 + offset)
                            tris[cnt++] = offset;
                        else
                            tris[cnt++] = i + 1;
                        tris[cnt++] = i;
                        tris[cnt++] = i + numVertices;
                    }
                }
            }
            else
            {
                // truncated cone
                tris = new int[numVertices * 6 * multiplier];
                if (outside)
                {
                    for (i = 0; i < numVertices; i++)
                    {
                        int ip1 = i + 1;
                        if (ip1 == numVertices)
                            ip1 = 0;

                        tris[cnt++] = i;
                        tris[cnt++] = ip1;
                        tris[cnt++] = i + numVertices;

                        tris[cnt++] = ip1 + numVertices;
                        tris[cnt++] = i + numVertices;
                        tris[cnt++] = ip1;
                    }
                }

                if (inside)
                {
                    for (i = offset; i < numVertices + offset; i++)
                    {
                        int ip1 = i + 1;
                        if (ip1 == numVertices + offset)
                            ip1 = offset;

                        tris[cnt++] = ip1;
                        tris[cnt++] = i;
                        tris[cnt++] = i + numVertices;

                        tris[cnt++] = i + numVertices;
                        tris[cnt++] = ip1 + numVertices;
                        tris[cnt++] = ip1;
                    }
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = tris;
            mesh.normals = normals;
            mesh.uv = uvs;

            return mesh;
        }
    }
}