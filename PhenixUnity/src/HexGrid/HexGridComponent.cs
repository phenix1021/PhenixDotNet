using UnityEngine;
using System.Collections.Generic;
using Phenix.Unity.AI.SEARCH;

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

        public T GetTile(HexCoordinates coords)
        {
            return _tileComps[coords];
        }

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
            tileObj.transform.parent = _tilesParent.transform;
            _tileComps.Add(tile.Coords, tileComp);

            OnTileCreated(tileComp);
        }

        public HexTile TileFromPosition(Vector3 position)
        {
            return _hexGrid.TileFromPosition(position);
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

        public void Neighbors(T tileComp, ref List<T> ret, bool onlyWalkable = false)
        {
            ret.Clear();
            var hexTiles = _hexGrid.Neighbours(tileComp.tile);
            foreach (var item in hexTiles)
            {
                T tile = GetTile(item.Coords);
                if (onlyWalkable && IsWalkable(tile) == false)
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
                ret.Add(GetTile(item.Coords));
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
                ret.Add(GetTile(item.Coords));
            }
        }

        public void TilesInLine(T t1, T t2, ref List<T> ret)
        {
            ret.Clear();
            var tiles = _hexGrid.TilesInLine(t1.tile.Coords, t2.tile.Coords);
            foreach (var item in tiles)
            {
                ret.Add(GetTile(item.Coords));
            }
        }

        protected virtual bool IsWalkable(T hexTileComponent)
        {
            return true;
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
        }

        public void FindPath(T src, T dst, ref List<T> path)
        {
            HexGridAStar<T> _astar = new HexGridAStar<T>(this);
            AStarResultCode ret = _astar.FindPath(src, dst, ref path, 0);
            Debug.Log(string.Format("寻路结果：{0}", ret));
        }
    }    
}