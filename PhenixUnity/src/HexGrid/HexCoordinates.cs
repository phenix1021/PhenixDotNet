using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phenix.Unity.Grid
{

    /// <summary>
    /// 正六边形坐标系(立方体坐标系)
    /// 原理参见文章：
    /// 1.https://www.redblobgames.com/grids/hexagons/#conversions
    ///   https://www.redblobgames.com/grids/hexagons/implementation.html
    /// 2.https://catlikecoding.com/unity/tutorials/hex-map/
    /// 3.本代码使用cube coordinates(立方体坐标系)。
    ///   当Pointy形状时x轴↗,y轴↖，z轴↓；
    ///   当Flat形状时x轴→，y轴↖，z轴↙；
    /// </summary>
    [System.Serializable]
    public struct HexCoordinates
    {        
        [SerializeField]
        int _x, _y, _z;

        public int X { get { return _x; } }
        public int Y { get { return _y; } }
        public int Z { get { return _z; } }

        // 六个方向
        public static List<HexCoordinates> Directions = new List<HexCoordinates>()
        {
            new HexCoordinates(1, 0, -1),
            new HexCoordinates(1, -1, 0),
            new HexCoordinates(0, -1, 1),
            new HexCoordinates(-1, 0, 1),
            new HexCoordinates(-1, 1, 0),
            new HexCoordinates(0, 1, -1)
        };

        public HexCoordinates(int x, int y, int z)
        {
            if (x + y + z != 0)
            {
                throw new ArgumentException("x + y + z must be 0");
            }
            this._x = x;
            this._y = y;
            this._z = z;
        }

        /// <summary>
        /// 围绕（0，0，0）左旋
        /// </summary>    
        public HexCoordinates RotateLeft()
        {
            return new HexCoordinates(-Z, -X, -Y);
        }

        /// <summary>
        /// 围绕（0，0，0）右旋
        /// </summary>    
        public HexCoordinates RotateRight()
        {
            return new HexCoordinates(-Y, -Z, -X);
        }

        public int Length()
        {
            return (int)((Mathf.Abs(X) + Mathf.Abs(Y) + Mathf.Abs(Z)) / 2);
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            HexCoordinates o = (HexCoordinates)other;
            if ((System.Object)o == null)
                return false;
            return ((_x == o._x) && (_y == o._y));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("HexCoords({0},{1},{2})", X.ToString(), Y.ToString(), Z.ToString());
        }

        public static HexCoordinates HexRound(float x, float y, float z)
        {
            int xi = Mathf.RoundToInt(x);
            int yi = Mathf.RoundToInt(y);
            int zi = Mathf.RoundToInt(z);
            double x_diff = Mathf.Abs(xi - x);
            double y_diff = Mathf.Abs(yi - y);
            double z_diff = Mathf.Abs(zi - z);

            if (x_diff > y_diff && x_diff > z_diff)
            {
                xi = -yi - zi;
            }
            else if (y_diff > z_diff)
            {
                yi = -xi - zi;
            }
            else
            {
                zi = -xi - yi;
            }

            return new HexCoordinates(xi, yi, zi);
        }

        public static int Distance(HexCoordinates a, HexCoordinates b)
        {
            return (Mathf.Abs(a._x - b._x) + Mathf.Abs(a.Z - b.Z) + Mathf.Abs(a._y - b._y)) / 2;
            // 或者 return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.Y - b.Y), Mathf.Abs(a.z - b.z));
        }

        public static HexCoordinates operator + (HexCoordinates one, HexCoordinates two)
        {
            return new HexCoordinates(one._x + two._x, one._y + two._y, one._z + two._z);
        }

        public static HexCoordinates operator -(HexCoordinates one, HexCoordinates two)
        {
            return new HexCoordinates(one._x - two._x, one._y - two._y, one._z - two._z);
        }

        public static HexCoordinates operator *(HexCoordinates hexCoord, int scale)
        {
            return new HexCoordinates(hexCoord._x * scale, hexCoord._y * scale, hexCoord._z * scale);
        }

    }
}