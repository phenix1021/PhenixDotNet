using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Phenix.Unity.Grid
{
    /// <summary>
    /// 网格形状
    /// </summary>
    [System.Serializable]
    public enum HexGridLayout
    {
        RECTANGLE = 0,      // 矩形
        HEXAGON,            // 正六边形
        TRIANGLE,           // 正三角形
        PARRALLELOGRAM,     // 平行四边形
    }

    [System.Serializable]
    public enum HexOrientation
    {
        POINTY = 0,
        FLAT = 1,
    }

    [System.Serializable]
    public class HexGridParams
    {
        // grid基准位置
        [HideInInspector]
        public Vector3 basePos;
        // 水平方向格子数量
        public int horizontalTilesCount;
        // 垂直方向格子数量        
        public int verticalTilesCount;
        // 格子朝向
        public HexOrientation tileOrientation;
        // 网格形状
        public HexGridLayout gridShape;
        // 边长（外径长度）
        public float sideLength;
        // hexTile的prefab
        public GameObject tilePrefab;
        public string tileLayerName;
    }    

    /// <summary>
    /// 六边形网格地图
    /// </summary>    
    public class HexGrid
    {
        HexGridParams _params;
        
        // tile列表        
        Dictionary<HexCoordinates, HexTile> _tiles = new Dictionary<HexCoordinates, HexTile>();
        public List<HexTile> Tiles { get { return _tiles.Values.ToList(); } }

        // HexTile外径
        [SerializeField]
        public float OuterRadius { get { return _params.sideLength; } }
        // HexTile内径
        public float InnerRadius { get { return OuterRadius * 0.866025404f; } }

        public HexGrid(HexGridParams param)
        {
            _params = param;      
        }

        public void AddTile(HexTile tile)
        {            
            _tiles.Add(tile.Coords, tile);
        }

        /// <summary>
        /// 清空网格
        /// </summary>
        public void ClearGrid()
        {            
            _tiles.Clear();
        }

        // 创建网格
        public void CreateGrid()
        {
            ClearGrid();
            //Generate the grid shape
            switch (_params.gridShape)
            {
                case HexGridLayout.HEXAGON:
                    GenHexShape();
                    break;
                case HexGridLayout.RECTANGLE:
                    GenRectShape();
                    break;
                case HexGridLayout.PARRALLELOGRAM:
                    GenParrallShape();
                    break;
                case HexGridLayout.TRIANGLE:
                    GenTriangleShape();
                    break;
                default:
                    break;
            }
        }

        void GenHexShape()
        {
            HexTile tile;
            Vector3 relPos = Vector3.zero;
            int mapSize = Mathf.Max(_params.horizontalTilesCount, _params.verticalTilesCount);

            for (int i = -mapSize; i <= mapSize; i++)
            {
                int r1 = Mathf.Max(-mapSize, -i - mapSize);
                int r2 = Mathf.Min(mapSize, -i + mapSize);
                for (int j = r1; j <= r2; j++)
                {
                    switch (_params.tileOrientation)
                    {
                        case HexOrientation.FLAT:
                            relPos.x = OuterRadius * 3.0f / 2.0f * i;
                            relPos.z = -OuterRadius * Mathf.Sqrt(3.0f) * (j + i / 2.0f);
                            break;

                        case HexOrientation.POINTY:
                            relPos.x = OuterRadius * Mathf.Sqrt(3.0f) * (i + j / 2.0f);
                            relPos.z = -OuterRadius * 3.0f / 2.0f * j;
                            break;
                    }

                    tile = new HexTile(new HexCoordinates(i, -i - j, j), OuterRadius,
                        _params.tileOrientation, _params.basePos + relPos);
                    AddTile(tile);
                }
            }
        }

        void GenRectShape()
        {
            HexTile tile;
            Vector3 relPos = Vector3.zero;
            switch (_params.tileOrientation)
            {
                case HexOrientation.FLAT:
                    for (int i = 0; i < _params.horizontalTilesCount; i++)
                    {
                        int qOff = i >> 1;
                        for (int j = -qOff; j < _params.verticalTilesCount - qOff; j++)
                        {
                            relPos.x = OuterRadius * 3.0f / 2.0f * i;
                            relPos.z = -OuterRadius * Mathf.Sqrt(3.0f) * (j + i / 2.0f);
                            tile = new HexTile(new HexCoordinates(i, -i - j, j), OuterRadius, 
                                _params.tileOrientation, _params.basePos + relPos);
                            AddTile(tile);
                        }
                    }
                    break;

                case HexOrientation.POINTY:
                    for (int i = 0; i < _params.verticalTilesCount; i++)
                    {
                        int rOff = i >> 1;
                        for (int j = -rOff; j < _params.horizontalTilesCount - rOff; j++)
                        {
                            relPos.x = OuterRadius * Mathf.Sqrt(3.0f) * (j + i / 2.0f);
                            relPos.z = -OuterRadius * 3.0f / 2.0f * i;
                            tile = new HexTile(new HexCoordinates(j, -j - i, i), OuterRadius, 
                                _params.tileOrientation, _params.basePos + relPos);
                            AddTile(tile);
                        }
                    }
                    break;
            }
        }

        void GenParrallShape()
        {
            HexTile tile;
            Vector3 relPos = Vector3.zero;
            for (int i = 0; i <= _params.horizontalTilesCount; i++)
            {
                for (int j = 0; j <= _params.verticalTilesCount; j++)
                {
                    switch (_params.tileOrientation)
                    {
                        case HexOrientation.FLAT:
                            relPos.x = OuterRadius * 3.0f / 2.0f * i;
                            relPos.z = -OuterRadius * Mathf.Sqrt(3.0f) * (j + i / 2.0f);
                            break;

                        case HexOrientation.POINTY:
                            relPos.x = OuterRadius * Mathf.Sqrt(3.0f) * (i + j / 2.0f);
                            relPos.z = -OuterRadius * 3.0f / 2.0f * j;
                            break;
                    }

                    tile = new HexTile(new HexCoordinates(i, -i - j, j), OuterRadius, 
                        _params.tileOrientation, _params.basePos + relPos);
                    AddTile(tile);
                }
            }
        }

        void GenTriangleShape()
        {
            HexTile tile;
            Vector3 relPos = Vector3.zero;
            int mapSize = Mathf.Max(_params.horizontalTilesCount, _params.verticalTilesCount);
            for (int i = 0; i <= mapSize; i++)
            {
                for (int j = 0; j <= mapSize - i; j++)
                {
                    switch (_params.tileOrientation)
                    {
                        case HexOrientation.FLAT:
                            relPos.x = OuterRadius * 3.0f / 2.0f * i;
                            relPos.z = -OuterRadius * Mathf.Sqrt(3.0f) * (j + i / 2.0f);
                            break;

                        case HexOrientation.POINTY:
                            relPos.x = OuterRadius * Mathf.Sqrt(3.0f) * (i + j / 2.0f);
                            relPos.z = -OuterRadius * 3.0f / 2.0f * j;
                            break;
                    }

                    tile = new HexTile(new HexCoordinates(i, -i - j, j), OuterRadius, 
                        _params.tileOrientation, _params.basePos + relPos);
                    AddTile(tile);
                }
            }
        }

        public HexTile TileFromPosition(Vector3 position)
        {
            HexCoordinates coords = CoordsFromPosition(position);
            if (_tiles.ContainsKey(coords) == false)
            {
                return null;
            }
            return _tiles[coords];
        }

        public HexCoordinates CoordsFromPosition(Vector3 position)
        {
            position -= _params.basePos;
            if (_params.tileOrientation == HexOrientation.POINTY)
            {
                float x = position.x / (InnerRadius * 2f);
                float y = -x;

                float offset = position.z / (OuterRadius * 3f);
                x += offset;
                y += offset;

                int iX = Mathf.RoundToInt(x);
                int iY = Mathf.RoundToInt(y);
                int iZ = Mathf.RoundToInt(-x - y);

                if (iX + iY + iZ != 0)
                {
                    float dX = Mathf.Abs(x - iX);
                    float dY = Mathf.Abs(y - iY);
                    float dZ = Mathf.Abs(-x - y - iZ);

                    if (dX > dY && dX > dZ)
                    {
                        iX = -iY - iZ;
                    }
                    else if (dZ > dY)
                    {
                        iZ = -iX - iY;
                    }
                }

                return new HexCoordinates(iX, -iX-iZ, iZ);
            }
            else
            {
                float y = position.z / (InnerRadius * 2f);
                float z = -y;

                float offset = -position.x / (OuterRadius * 3f);
                y += offset;
                z += offset;

                int iY = Mathf.RoundToInt(y);
                int iZ = Mathf.RoundToInt(z);
                int iX = Mathf.RoundToInt(-z - y);

                if (iX + iY + iZ != 0)
                {
                    float dY = Mathf.Abs(y - iY);
                    float dZ = Mathf.Abs(z - iZ);
                    float dX = Mathf.Abs(-z - y - iX);

                    if (dX > dY && dX > dZ)
                    {
                        iX = -iY - iZ;
                    }
                    else if (dZ > dY)
                    {
                        iZ = -iX - iY;
                    }
                }

                return new HexCoordinates(iX, -iX-iZ, iZ);
            }

        }

        public HexTile TileAt(HexCoordinates coords)
        {
            if (_tiles.ContainsKey(coords))
                return _tiles[coords];
            return null;
        }

        public HexTile TileAt(int x, int y, int z)
        {
            return TileAt(new HexCoordinates(x, y, z));
        }

        public List<HexTile> Neighbours(HexTile tile)
        {
            List<HexTile> ret = new List<HexTile>();

            if (tile == null)
                return ret;

            HexCoordinates o;
            for (int i = 0; i < 6; i++)
            {
                o = tile.Coords + HexCoordinates.Directions[i];
                if (_tiles.ContainsKey(o))
                    ret.Add(_tiles[o]);
            }
            return ret;
        }

        public List<HexTile> Neighbours(HexCoordinates index)
        {
            return Neighbours(TileAt(index));
        }

        public List<HexTile> Neighbours(int x, int y, int z)
        {
            return Neighbours(TileAt(x, y, z));
        }

        public List<HexTile> TilesInRange(HexTile center, int range)
        {
            List<HexTile> ret = new List<HexTile>();
            HexCoordinates o;

            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = Mathf.Max(-range, -range - dx);
                    dy <= Mathf.Min(range, range - dx);
                    dy++)
                {
                    o = new HexCoordinates(dx, dy, -dx - dy) + center.Coords;
                    if (_tiles.ContainsKey(o))
                        ret.Add(_tiles[o]);
                }
            }
            return ret;
        }

        public List<HexTile> TilesInRange(HexTile center, int minRange, int maxRange)
        {
            List<HexTile> min = TilesInRange(center, minRange);
            List<HexTile> max = TilesInRange(center, maxRange);
            return new List<HexTile>(min.Except(max));
        }


        public List<HexTile> TilesInRange(HexCoordinates index, int range)
        {
            return TilesInRange(TileAt(index), range);
        }

        public List<HexTile> TilesInRange(int x, int y, int z, int range)
        {
            return TilesInRange(TileAt(x, y, z), range);
        }

        public int Distance(HexCoordinates a, HexCoordinates b)
        {
            return (Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y) +
                Mathf.Abs(a.Z - b.Z)) / 2;
        }

        public int Distance(HexTile a, HexTile b)
        {
            return Distance(a.Coords, b.Coords);
        }

        public List<HexTile> TilesInLine(HexCoordinates start, HexCoordinates end)
        {
            List<HexTile> ret = new List<HexTile>();
            int dist = Distance(start, end);
            if (dist == 0)
            {
                if (_tiles.ContainsKey(start))
                {
                    ret.Add(_tiles[start]);
                }
            }
            else
            {
                // 插值
                for (int i = 0; i <= dist; i++)
                {
                    float progress = i / dist;
                    HexCoordinates coords = HexCoordinates.HexRound(
                        Mathf.Lerp(start.X, end.X, progress),
                        Mathf.Lerp(start.Y, end.Y, progress),
                        Mathf.Lerp(start.Z, end.Z, progress));
                    if (_tiles.ContainsKey(coords))
                    {
                        ret.Add(_tiles[coords]);
                    }
                }
            }
            return ret;
        }
    }
}