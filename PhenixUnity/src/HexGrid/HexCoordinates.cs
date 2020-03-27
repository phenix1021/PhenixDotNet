using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phenix.Unity.Grid
{

    /// <summary>
    /// 正六边形坐标系(立方体坐标系)
    /// 原理参见文章：
    /// 1.https://www.redblobgames.com/grids/hexagons/#conversions
    /// 2.https://catlikecoding.com/unity/tutorials/hex-map/
    /// 3.本代码使用的cube coordinates(立方体坐标系)和上述文章有所不同。
    ///   当Pointy形状时q轴↗,r轴↖，s轴↓；
    ///   当Flat形状时q轴→，r轴↖，s轴↙；
    /// </summary>
    [System.Serializable]
    public struct HexCoordinates
    {        
        [SerializeField]
        int _q, _r, _s;

        public int Q { get { return _q; } }
        public int R { get { return _r; } }
        public int S { get { return _s; } }

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

        public HexCoordinates(int q, int r, int s)
        {
            if (q + r + s != 0)
            {
                throw new ArgumentException("q + r + s must be 0");
            }
            this._q = q;
            this._r = r;
            this._s = s;
        }

        /// <summary>
        /// 围绕（0，0，0）左旋
        /// </summary>    
        public HexCoordinates RotateLeft()
        {
            return new HexCoordinates(-S, -Q, -R);
        }

        /// <summary>
        /// 围绕（0，0，0）右旋
        /// </summary>    
        public HexCoordinates RotateRight()
        {
            return new HexCoordinates(-R, -S, -Q);
        }

        public int Length()
        {
            return (int)((Mathf.Abs(Q) + Mathf.Abs(R) + Mathf.Abs(S)) / 2);
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            HexCoordinates o = (HexCoordinates)other;
            if ((System.Object)o == null)
                return false;
            return ((_q == o._q) && (_r == o._r));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("HexCoords({0},{1},{2})", Q.ToString(), R.ToString(), S.ToString());
        }

        public static HexCoordinates HexRound(float q, float r, float s)
        {
            int qi = Mathf.RoundToInt(q);
            int ri = Mathf.RoundToInt(r);
            int si = Mathf.RoundToInt(s);
            double q_diff = Mathf.Abs(qi - q);
            double r_diff = Mathf.Abs(ri - r);
            double s_diff = Mathf.Abs(si - s);

            if (q_diff > r_diff && q_diff > s_diff)
            {
                qi = -ri - si;
            }
            else if (r_diff > s_diff)
            {
                ri = -qi - si;
            }
            else
            {
                si = -qi - ri;
            }

            return new HexCoordinates(qi, ri, si);
        }

        public static int Distance(HexCoordinates a, HexCoordinates b)
        {
            return (Mathf.Abs(a._q - b._q) + Mathf.Abs(a.S - b.S) + Mathf.Abs(a._r - b._r)) / 2;
            // 或者 return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.Y - b.Y), Mathf.Abs(a.z - b.z));
        }

        public static HexCoordinates operator + (HexCoordinates one, HexCoordinates two)
        {
            return new HexCoordinates(one._q + two._q, one._r + two._r, one._s + two._s);
        }

        public static HexCoordinates operator -(HexCoordinates one, HexCoordinates two)
        {
            return new HexCoordinates(one._q - two._q, one._r - two._r, one._s - two._s);
        }

        public static HexCoordinates operator *(HexCoordinates hexCoord, int scale)
        {
            return new HexCoordinates(hexCoord._q * scale, hexCoord._r * scale, hexCoord._s * scale);
        }

    }
}