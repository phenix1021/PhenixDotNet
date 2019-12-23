using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
/*
namespace Phenix.Unity.UI
{
    /// <summary>
    /// 颜色渐变
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Phenix/UI/ColorGradient1")]
    public class ColorGradient1 : BaseMeshEffect
    {
        public enum GradientDirection
        {
            TOP_BOTTOM = 0,
            LEFT_RIGHT,
        }

        public Color32 startColor = Color.white;
        public Color32 endColor = Color.white;        

        public GradientDirection gradientDirection;

        List<UIVertex> _vertexList = new List<UIVertex>();

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
                if (gradientDirection == GradientDirection.TOP_BOTTOM)
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

                if (gradientDirection == GradientDirection.TOP_BOTTOM)
                {
                    uiVertex.color = Color32.Lerp(endColor, startColor, (uiVertex.position.y - start) / totalDist);
                }
                else
                {
                    uiVertex.color = Color32.Lerp(startColor, endColor, (uiVertex.position.x - start) / totalDist);
                }
                
                vh.SetUIVertex(uiVertex, i);
            }
        }
    }
}*/