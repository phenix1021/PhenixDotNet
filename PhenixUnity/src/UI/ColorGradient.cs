using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Phenix.Unity.UI
{
    /// <summary>
    /// 颜色渐变
    /// </summary>
    /// <mark>
    /// 对于Image或者单个字符的Text，如果设置了多层渐变，只有首尾两种颜色会起作用，因为
    /// 该情况下横竖两个方向只有两个数值的顶点。
    /// </mark>>
    [ExecuteInEditMode]
    [AddComponentMenu("Phenix/UI/ColorGradient")]
    public class ColorGradient : BaseMeshEffect
    {
        public enum GradientDirection
        {
            VERTICAL = 0,   // 从上到下
            HORIZONTAL,     // 从左往右
        }

        public List<Color32> colors = new List<Color32>();
        public GradientDirection gradientDirection;

        List<UIVertex> _vertexList = new List<UIVertex>();

        protected override void Start()
        {
            colors.Add(Color.black);
            colors.Add(Color.white);
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }

            int vertCount = vh.currentVertCount;
            if (vertCount < 2)
            {
                return;
            }

            if (colors.Count == 0)
            {
                return;
            }

            _vertexList.Clear();
            for (var i = 0; i < vertCount; i++)
            {
                var vertex = new UIVertex();
                vh.PopulateUIVertex(ref vertex, i);
                _vertexList.Add(vertex);
            }

            float start = Mathf.Infinity;
            float end = Mathf.NegativeInfinity;

            // 遍历所有顶点，获取首尾顶点
            for (int i = 0; i < vertCount; i++)
            {
                float val = 0;
                if (gradientDirection == GradientDirection.VERTICAL)
                {
                    val = _vertexList[i].position.y;
                }
                else
                {
                    val = _vertexList[i].position.x;
                }

                if (val > end)
                {
                    end = val;
                }
                else if (val < start)
                {
                    start = val;
                }
            }

            // 首尾距离
            float totalDist = end - start;
            if (totalDist == 0)
            {
                return;
            }            

            // 遍历所有顶点上色
            for (int i = 0; i < vertCount; i++)
            {
                UIVertex uiVertex = _vertexList[i];

                if (colors.Count == 1)
                {
                    uiVertex.color = colors[0];
                }
                else
                {
                    Color32 startColor = Color.white;
                    Color32 endColor = Color.white;

                    float step = totalDist / (colors.Count - 1);
                    float passed = 0;

                    if (gradientDirection == GradientDirection.VERTICAL)
                    {
                        passed = uiVertex.position.y - start;                        
                    }
                    else
                    {
                        passed = uiVertex.position.x - start;                        
                    }

                    int stepIdx = (int)(passed / step);
                    if (stepIdx >= colors.Count - 1)
                    {
                        startColor = endColor = colors[colors.Count - 1];
                    }
                    else
                    {
                        startColor = colors[stepIdx];
                        endColor = colors[stepIdx + 1];
                    }

                    float remain = passed - stepIdx * step;
                    uiVertex.color = Color32.Lerp(startColor, endColor, remain / step);
                }
                
                vh.SetUIVertex(uiVertex, i);
            }
        }
    }
}