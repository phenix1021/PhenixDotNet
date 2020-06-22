using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.AI.SEARCH;
using Phenix.Unity.Extend;

namespace Phenix.Unity.Grid
{
    public abstract class HexGridComponent<T> : MonoBehaviour 
        where T : HexTileComponent, AStarNode<T>
    {        
        HexGrid _hexGrid;   // 网格数据
        
        public HexGridParams gridParams;    // 创建grid需要的各种参数       
        public string tilesParentTag = "TilesParent";

        GameObject _tilesParent;

        // tile组件列表        
        Dictionary<HexCoordinates, T> _tileComps = 
            new Dictionary<HexCoordinates, T>();

        public IEnumerable<T> Tiles { get { return _tileComps.Values; } }

        // 是否使用寻路缓存（注意：如果tile的walkable会动态改变，如tile上有无hero，则不支持寻路缓存）
        [SerializeField]
        protected bool useFindPathCache = false;

        HexGridAStar<T> _astar;
        
        /// <summary>
        /// 清除网格 
        /// </summary>      
        public void CleraGrid()
        {
            // 清除tile对象
            foreach (var item in _tileComps.Values)
            {
                DestroyImmediate(item.gameObject);
            }

            _tileComps.Clear();
            if (_hexGrid != null)
            {
                _hexGrid.ClearGrid();
            }            
        }

        /// <summary>
        /// 创建网格
        /// </summary>
        public void CreateGrid()
        {            
            CleraGrid();
            _tilesParent = GameObject.FindGameObjectWithTag(tilesParentTag);
            if (_tilesParent)
            {
                gridParams.basePos = _tilesParent.transform.position;
            }
            else
            {
                gridParams.basePos = Vector3.zero;
            }
            
            _hexGrid = new HexGrid(gridParams);
            _hexGrid.CreateGrid();            
            foreach (var tile in _hexGrid.Tiles)
            {
                CreateTileObj(tile);
            }

            CreatePathFinder();
        }

        void CreatePathFinder()
        {
            if (_astar == null)
            {
                _astar = new HexGridAStar<T>(this);
            }

            if (useFindPathCache)
            {
                // 支持寻路缓存
                List<T> walkableTiles = new List<T>();
                foreach (var item in _tileComps)
                {
                    if (IsWalkable(item.Value))
                    {
                        walkableTiles.Add(item.Value);
                    }
                }
                _astar.ResetPathCache(walkableTiles);
            }
        }

        /// <summary>
        /// 创建tile对象
        /// </summary>        
        void CreateTileObj(HexTile tile)
        {
            // 创建tile对象
            GameObject tileObj = null;
            if (gridParams.tilePrefab)
            {                
                tileObj = Instantiate(gridParams.tilePrefab);
                tileObj.name = "Tile" + tile.Coords.ToString();
                tileObj.layer = LayerMask.NameToLayer(gridParams.tileLayerName);
            }

            tileObj.AddComponent<MeshCollider>();
            T tileComp = tileObj.AddComponent<T>();
            tileComp.tile = tile;                   
            tileObj.transform.position = tile.Center;
            tileObj.transform.SetParent(_tilesParent.transform);
            _tileComps.Add(tile.Coords, tileComp);

            OnTileCreated(tileComp);
        }

        public HexTile TileFromPosition(Vector3 position)
        {
            return _hexGrid.TileFromPosition(position);
        }

        public T TileCompFromCoords(HexCoordinates coords)
        {
            return _tileComps[coords];
        }

        public T TileCompFromPosition(Vector3 position)
        {
            HexTile hexTile = _hexGrid.TileFromPosition(position);
            if (_tileComps.ContainsKey(hexTile.Coords))
            {
                return _tileComps[hexTile.Coords];
            }
            return null;
        }

        public void Neighbors(T tileComp, ref List<T> ret)
        {
            ret.Clear();
            var hexTiles = _hexGrid.Neighbours(tileComp.tile);
            foreach (var item in hexTiles)
            {
                T tile = TileCompFromCoords(item.Coords);                
                ret.Add(tile);
            }            
        }

        public void WalkableNeighbors(T tileComp, ref List<T> ret)
        {
            ret.Clear();
            var hexTiles = _hexGrid.Neighbours(tileComp.tile);
            foreach (var item in hexTiles)
            {
                T tile = TileCompFromCoords(item.Coords);
                if (IsWalkable(tile) == false)
                {
                    continue;
                }
                ret.Add(tile);
            }
        }

        public void UnblockNeighbors(T tileComp, ref List<T> ret)
        {
            ret.Clear();
            var hexTiles = _hexGrid.Neighbours(tileComp.tile);
            foreach (var item in hexTiles)
            {
                T tile = TileCompFromCoords(item.Coords);
                if (IsBlock(tile))
                {
                    continue;
                }
                ret.Add(tile);
            }
        }

        public int Distance(T t1, T t2)
        {            
            return _hexGrid.Distance(t1.tile.Coords, t2.tile.Coords);
        }

        public void TilesInRange(T t, int range, ref List<T> ret)
        {
            ret.Clear();
            if (range <= 0)
            {
                return;
            }
            var tiles = _hexGrid.TilesInRange(t.tile, range);
            foreach (var item in tiles)
            {                
                ret.Add(TileCompFromCoords(item.Coords));
            }
        }

        public void TilesInRange(T t, int minRange, int maxRange, ref List<T> ret)
        {
            ret.Clear();
            if (minRange > maxRange || maxRange <= 0)
            {
                return;
            }
            var tiles = _hexGrid.TilesInRange(t.tile, minRange, maxRange);
            foreach (var item in tiles)
            {
                ret.Add(TileCompFromCoords(item.Coords));
            }
        }

        public void TilesInLine(T t1, T t2, ref List<T> ret)
        {
            ret.Clear();
            var tiles = _hexGrid.TilesInLine(t1.tile.Coords, t2.tile.Coords);
            foreach (var item in tiles)
            {
                ret.Add(TileCompFromCoords(item.Coords));
            }
        }

        public void ReachableTilesInLine(T t1, T t2, ref List<T> ret)
        {
            ret.Clear();
            var tiles = _hexGrid.TilesInLine(t1.tile.Coords, t2.tile.Coords);
            foreach (var item in tiles)
            {
                T tileComp = TileCompFromCoords(item.Coords);
                if (IsWalkable(tileComp) == false)
                {
                    continue;
                }
                ret.Add(TileCompFromCoords(item.Coords));
            }
        }

        protected virtual bool IsBlock(T hexTileComponent)
        {
            return false;
        }

        protected virtual bool IsWalkable(T hexTileComponent)
        {
            return IsBlock(hexTileComponent) == false;
        }

        protected virtual void OnTileCreated(T hexTileComponent)
        {
            // 派生类可以在此处理hexTileComponent.prop的赋值、添加MeshRender其它组件，以及相关操作
        }

        public void ReachableTilesInRange(T t, int range, ref List<T> ret)
        {
            ret.Clear();
            if (range <= 0)
            {
                return;
            }
            
            if (range == 1)
            {
                WalkableNeighbors(t, ref ret);
                return;
            }

            // range > 1
            t.Status = AStarNodeStatus.CLOSED;

            int[] loops = new int[range-1];
            List<T> neighbors = new List<T>();
            WalkableNeighbors(t, ref neighbors);
            loops[0] = neighbors.Count;
            foreach (var item in neighbors)
            {
                ret.Add(item);
                item.Status = AStarNodeStatus.CLOSED;
            }

            int count = 0;
            int idx = 0;
            for (int i = 0; i < loops.Length; i++)
            {                
                count += loops[i];
                while (idx < count)
                {
                    WalkableNeighbors(ret[idx], ref neighbors);
                    foreach (var item in neighbors)
                    {
                        if (item.Status != AStarNodeStatus.CLOSED)
                        {
                            ret.Add(item);
                            item.Status = AStarNodeStatus.CLOSED;
                            if (i + 1 < loops.Length)
                            {
                                loops[i + 1] += 1;
                            }
                        }
                    }
                    ++idx;
                }
            }

            foreach (var item in ret)
            {
                item.Status = AStarNodeStatus.NONE;                
            }
            t.Status = AStarNodeStatus.NONE;
        }

        /* 递归可行，但效率太低
        public void ReachableTilesInRange(T t, int range, ref List<T> ret)
        {            
            ret.Clear();
            if (range <= 0)
            {
                return;
            }
            HashSet<T> _exists = new HashSet<T>();
            _exists.Add(t);
            DSF(t, 1, range, ref ret, ref _exists);            
        }

        void DSF(T t, int depth, int maxDepth, ref List<T> ret, ref HashSet<T> exists)
        {
            List<T> neighbors = new List<T>();
            Neighbors(t, ref neighbors, true);
            foreach (var item in neighbors)
            {                
                if (depth < maxDepth)
                {                    
                    // 递归调用
                    DSF(item, depth+1, maxDepth, ref ret, ref exists);
                }

                if (exists.Contains(item))
                {
                    continue;
                }

                ret.Add(item);
                exists.Add(item);
            }
        }*/

        public bool FindPath(T src, T dst, ref List<T> path)
        {            
            AStarResultCode ret = _astar.FindPath(src, dst, ref path, 0);
            Debug.Log(string.Format("寻路结果：{0}", ret));
            return ret == AStarResultCode.SUCCESS;
        }

        /// <summary>
        /// 获得随机一个tile
        /// </summary>
        public T RandomTile()
        {            
            int idx = Random.Range(0, _hexGrid.Tiles.Count);
            return _tileComps[_hexGrid.Tiles[idx].Coords];
        }

        /// <summary>
        /// 获得随机一个非阻挡tile
        /// </summary>
        public T RandomUnblockTile()
        {
            List<T> unblockTiles = new List<T>();
            foreach (var item in _tileComps)
            {
                if (IsBlock(item.Value) == false)
                {
                    unblockTiles.Add(item.Value);
                }
            }

            if (unblockTiles.Count == 0)
            {
                return null;
            }

            return unblockTiles[Random.Range(0, unblockTiles.Count)];
        }

        /// <summary>
        /// 获得随机N个tile
        /// </summary>
        public void RandomTiles(ref List<T> ret, int count)
        {
            ret.Clear();
            List<T> tiles = new List<T>();
            foreach (var item in _tileComps)
            {
                tiles.Add(item.Value);
            }

            if (tiles.Count < count)
            {
                return;
            }

            tiles.Shuffle();
            for (int i = 0; i < count; i++)
            {
                ret.Add(tiles[i]);
            }
        }

        /// <summary>
        /// 获得随机N个非阻挡tile
        /// </summary>
        public void RandomUnblockTiles(ref List<T> ret, int count)
        {
            ret.Clear();
            List<T> unblockTiles = new List<T>();
            foreach (var item in _tileComps)
            {
                if (IsBlock(item.Value) == false)
                {
                    unblockTiles.Add(item.Value);
                }
            }

            if (unblockTiles.Count < count)
            {
                return;
            }

            unblockTiles.Shuffle();
            for (int i = 0; i < count; i++)
            {
                ret.Add(unblockTiles[i]);
            }
        }

        /// <summary>
        /// 检测是否存在“孤岛”（非阻挡却无法到达）
        /// </summary>
        public bool CheckUnreachableButNonblockTiles()
        {
            int blockCount = 0; // 阻挡的数量
            T unblockTile = null;
            foreach (var item in _tileComps)
            {
                if (IsBlock(item.Value))
                {                    
                    ++blockCount;
                }
                else
                {
                    if (unblockTile == null)
                    {
                        unblockTile = item.Value; // 任何一个非阻挡的tile
                    }
                }
            }

            List<T> connected = new List<T>();
            // 获得walkableTile能到达的所有tile
            ConnectedTiles(unblockTile, ref connected);

            return connected.Count + blockCount + 1/*即unblockTile*/ < _tileComps.Count;
        }

        /// <summary>
        /// 获得和指定tile相连通的所有tile（只需要考虑是否block）
        /// </summary>
        public void ConnectedTiles(T t, ref List<T> ret)
        {            
            ret.Clear();
            if (IsBlock(t))
            {
                return;
            }

            Queue<T> openTiles = new Queue<T>();
            openTiles.Enqueue(t);
            t.Status = AStarNodeStatus.CLOSED;
            List<T> neighbors = new List<T>();
            while (openTiles.Count > 0)
            {
                T cur = openTiles.Dequeue();
                if (cur != t)
                {
                    ret.Add(cur);
                }
                UnblockNeighbors(cur, ref neighbors);
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.Status == AStarNodeStatus.CLOSED)
                    {
                        continue;
                    }
                    openTiles.Enqueue(neighbor);
                    neighbor.Status = AStarNodeStatus.CLOSED;
                }
            }

            foreach (var item in ret)
            {
                item.Status = AStarNodeStatus.NONE;
            }
            t.Status = AStarNodeStatus.NONE;
        }
    }    
}