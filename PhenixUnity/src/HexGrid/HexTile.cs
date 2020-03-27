using UnityEngine;

namespace Phenix.Unity.Grid
{    
    public class HexTile
    {
        public const int CornerCount = 6; // 六个顶点
        
        // tile的立方体坐标（Cube Coordinates）
        public HexCoordinates Coords { get; private set; }
        // tile中心点的笛卡尔坐标
        public Vector3 Center { get; private set; }
        // tile各个顶点的笛卡尔坐标偏移
        public Vector3[] Corners { get; private set; }

        public HexTile(HexCoordinates coordinates, float outerRadius,
            HexOrientation orientation, Vector3 center)
        {
            Coords = coordinates;
            Center = center;
            Corners = new Vector3[CornerCount];
            CalculateCorners(coordinates, orientation, outerRadius);
        }

        void CalculateCorners(HexCoordinates coordinates, 
            HexOrientation orientation, float outerRadius)
        {
            for (int i = 0; i < CornerCount; i++)
            {
                float angle = 60 * i;
                if (orientation == HexOrientation.POINTY)
                {
                    angle += 30;
                }
                angle *= Mathf.PI / 180;
                Corners[i].x = Center.x + outerRadius * Mathf.Cos(angle);
                Corners[i].y = Center.y;
                Corners[i].z = Center.z + outerRadius * Mathf.Sin(angle);
            }
        }

        public override bool Equals(object obj)
        {
            return Coords.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}