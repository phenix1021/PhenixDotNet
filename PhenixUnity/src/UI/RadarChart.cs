using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Phenix.Unity.UI
{
    /// <summary>
    /// 雷达图
    /// </summary>
    [AddComponentMenu("Phenix/UI/RadarChart")]
    [ExecuteInEditMode]
    public class RadarChart : MaskableGraphic
    {
        [Range(0.05f, 1)]
        public float fillPercent = 1f;

        [Range(0f, 1f)]
        [SerializeField]
        List<float> _propValues = new List<float>();

        [Range(0f, 360f)]
        public float angleOffset = 0;

        public Color propAxisColor = Color.yellow;
        public float propAxisWidth = 1f;

        public Color edgeColor = Color.blue;
        public float edgeWidth = 1f;

        public float radius = 40;

        public bool showAxis = false;
        public Color axisColor = Color.black;
        public float axisWidth = 1f;
        float _axisLength = 0;
        
        public void SetPropValue(int idx, float val)
        {
            if (idx < 0 || idx >= _propValues.Count)
            {
                return;
            }
            _propValues[idx] = Mathf.Clamp01(val);
            SetAllDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            Vector2 size = GetComponent<RectTransform>().rect.size / 2f;
            _axisLength = Mathf.Min(size.x, size.y);

            vh.Clear();

            if (showAxis)
            {
                DrawAxis(vh);   // 绘制坐标轴
            }

            int propCount = _propValues.Count;

            if (propCount == 1)
            {                
                // 绘制属性区域
                DrawPropArea(vh, GetQuadFromLine(Vector2.zero, GetPoint(0) * _propValues[0], 2, color));
                // 绘制属性轴
                DrawPropAxis(vh, 0);
            }
            else if (propCount == 2)
            {                
                // 绘制属性区域
                DrawPropArea(vh, GetQuadFromLine(GetPoint(0) * _propValues[0], GetPoint(1) * _propValues[1], 2, color));
                // 绘制属性轴
                DrawPropAxis(vh, 0);
                DrawPropAxis(vh, 1);
            }
            else
            {
                for (int i = 0; i < propCount; i++)
                {
                    Vector2 pos1 = GetPoint(i) * _propValues[i];
                    Vector2 pos2 = pos1 * (1 - fillPercent);
                    Vector2 pos4 = (i + 1 >= propCount) ? (GetPoint(0) * _propValues[0]) : (GetPoint(i + 1) * _propValues[i + 1]);
                    Vector2 pos3 = pos4 * (1 - fillPercent);
                    
                    // 绘制属性区域
                    DrawPropArea(vh, GetQuadBetweenProps(pos1, pos2, pos3, pos4));
                    // 绘制属性轴
                    DrawPropAxis(vh, i);
                    // 绘制边线
                    DrawEdge(vh, i);
                }
            }            
        }

        void DrawPropArea(VertexHelper vh, UIVertex[] vertexes)
        {
            vh.AddUIVertexQuad(vertexes);
        }

        void DrawEdge(VertexHelper vh, int curPropIdx)
        {
            Vector2 lineStartPos = GetPoint(curPropIdx);
            Vector2 lineNextPos = GetPoint((curPropIdx + 1) % _propValues.Count);
            vh.AddUIVertexQuad(GetQuadFromLine(lineStartPos, lineNextPos, edgeWidth, edgeColor));
        }

        void DrawPropAxis(VertexHelper vh, int propIdx)
        {
            Vector2 lineEndPos = GetPoint(propIdx);
            vh.AddUIVertexQuad(GetQuadFromLine(Vector2.zero, lineEndPos, propAxisWidth, propAxisColor));
        }

        Vector2 GetPoint(int propIdx)
        {
            int propCount = _propValues.Count;
            float angle = 360f / propCount * propIdx + angleOffset;
            float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
            float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
            return new Vector2(radius * cos, radius * sin);
        }

        UIVertex[] GetQuadFromLine(Vector2 start, Vector2 end, float width, Color lineColor)
        {
            UIVertex[] vs = new UIVertex[4];

            Vector2 v1 = end - start;
            Vector2 v2 = (v1.y == 0f) ? new Vector2(0f, 1f) : new Vector2(1f, -v1.x / v1.y);
            v2.Normalize();
            v2 *= width / 2f;

            Vector2[] pos = new Vector2[4];
            pos[0] = start + v2;
            pos[1] = end + v2;
            pos[2] = end - v2;
            pos[3] = start - v2;
            for (int i = 0; i < 4; i++)
            {
                vs[i].color = lineColor;
                vs[i].position = pos[i];
                vs[i].uv0 = Vector2.zero;
            }
            return vs;
        }

        UIVertex[] GetQuadBetweenProps(params Vector2[] vertPos)
        {
            UIVertex[] vs = new UIVertex[vertPos.Length];
            for (int i = 0; i < vertPos.Length; i++)
            {
                vs[i].color = color;
                vs[i].position = vertPos[i];
                vs[i].uv0 = Vector2.zero;
            }
            return vs;
        }

        void DrawAxis(VertexHelper vh)
        {
            Vector2 startPosX = Vector2.zero - new Vector2(_axisLength, 0);
            Vector2 endPosX = Vector2.zero + new Vector2(_axisLength, 0);

            Vector2 startPosY = Vector2.zero - new Vector2(0, _axisLength);
            Vector2 endPosY = Vector2.zero + new Vector2(0, _axisLength);

            vh.AddUIVertexQuad(GetQuadFromLine(startPosX, endPosX, axisWidth, axisColor));
            vh.AddUIVertexQuad(GetQuadFromLine(startPosY, endPosY, axisWidth, axisColor));

            // 绘制箭头
            float arrowSize = axisWidth * 2;
            var xFirst = endPosX + new Vector2(0, arrowSize);
            var xSecond = endPosX + new Vector2(1.73f * arrowSize, 0);
            var xThird = endPosX + new Vector2(0, -arrowSize);
            vh.AddUIVertexQuad(new UIVertex[]
            {
                 new UIVertex{position = xFirst, color = axisColor },
                 new UIVertex{position = xSecond, color = axisColor },
                 new UIVertex{position = xThird, color = axisColor },
                 new UIVertex{position = endPosX, color = axisColor },
            });

            var yFirst = endPosY + new Vector2(-arrowSize, 0);
            var ySecond = endPosY + new Vector2(0, 1.73f * arrowSize);
            var yThird = endPosY + new Vector2(arrowSize, 0);
            vh.AddUIVertexQuad(new UIVertex[]
            {
                 new UIVertex{position = yFirst, color = axisColor },
                 new UIVertex{position = ySecond, color = axisColor },
                 new UIVertex{position = yThird, color = axisColor },
                 new UIVertex{position = endPosY, color = axisColor },
            });
        }
    }
}