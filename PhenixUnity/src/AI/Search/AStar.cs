using System.Collections.Generic;
using Phenix.Unity.Collection;

namespace Phenix.Unity.AI.SEARCH
{
    /*
     * 原理参考 https://www.jianshu.com/p/e52d856e7d48 （《我见过的最容易读懂的 a*算法(A*寻路初探)》）
     * 优化点：
     *   1.open列表采用最小堆而非普通list
     *   2.不使用close列表而是采用状态字段。寻路单元（T）必须实现AStarNode接口，因为AStar需要访问接口
     * 中的数据。这样的方式虽然对单元本身有一定侵入性，但能大大提高获取Parent、Status和G的效率。
     * 如果使用原始的寻路单元（T），就只能在AStar里添加map等结构保存Parent、Status和G，效率降低。
     * 无法使用内建结构（如Node）包含T的方法，因为遍历T的邻居时即便动态创建Node，也无法方便做到同
     * 一个T实例对应同一个Node
     *   3.支持寻路缓存（可选）。在考虑节点数消耗内存的情况下，使用二维数组保存任意两点间的路径。
     */
    public interface AStarNode<T> where T : class
    {
        T Parent { get; set; }                  // 父节点（由此构成最终路径）
        AStarNodeStatus Status { get; set; }    // 节点状态
        float G { get; set; }                   // 路径累计消耗，对应启发函数 F = G + H 中的G     
        int PathCacheIdx { get; set; }          // 路径缓存中的下标
    }

    // A*节点状态
    public enum AStarNodeStatus
    {
        NONE = 0,   // 未访问
        OPENED,     // 待确认（还需要对其邻居进行访问）
        CLOSED,     // 已关闭（已对其邻居进行访问）
    }

    // A*寻路结果
    public enum AStarResultCode
    {
        NONE = 0,
        SUCCESS,
        FAIL_NO_WAY,        // 未发现可达路径
        FAIL_OVERLOAD,      // 搜寻节点数量超过上限
    }

    public abstract class AStar<T> where T : class, AStarNode<T>
    {
        protected T Start { get; private set; }
        protected T Finish { get; private set; }
        
        MinHeap<T> _openedNodes;
        List<T> _allNodes = new List<T>();

        // 路径缓存
        T[,] _pathCache = null;
        
        public AStar()
        {
            _openedNodes = new MinHeap<T>((T a, T b) => { return GetF(a).CompareTo(GetF(b)); });
        }
        
        public void ResetPathCache(List<T> nodes/*所有可缓存的节点*/)
        {
            // 创建路径缓存二维数组
            _pathCache = new T[nodes.Count, nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                // 设置各缓存节点的下标
                nodes[i].PathCacheIdx = i;
                _pathCache[i, i] = nodes[i];

                for (int j = 0; j < nodes.Count; j++)
                {
                    if (i != j)
                    {
                        _pathCache[i, j] = null;
                    }
                }
            }
        }
        
        bool GetPathFromCache(T fromNode, T toNode, ref List<T> path)
        {
            path.Clear();
            if (InPathCache(fromNode, toNode) == false)
            {
                return false;
            }
            
            T node = toNode;
            while (true)
            {
                path.Insert(0, node);
                T tmp = _pathCache[fromNode.PathCacheIdx, node.PathCacheIdx];
                if (tmp == node)
                {
                    break;
                }
                node = tmp;
            }

            return true;
        }

        bool InPathCache(T node1, T node2)
        {
            return (_pathCache != null && 
                node1 != node2 &&
                _pathCache[node1.PathCacheIdx, node2.PathCacheIdx] != null);
        }

        // 当前节点的开销（可依据地形、位置等要素确定）。一般来说G = Cost+父节点G
        protected virtual float GetCost(T node)
        {
            return 1;
        }

        // 当前节点相对终点的H值，即启发函数 F = G + H 中的H，可以选择欧拉距离、曼哈顿距离等等
        protected abstract float GetH(T node, T finish);
        // 是否到达终点
        protected abstract bool Arrived(T node, T finish);
        // 获得相邻节点
        protected abstract void Neighbors(T node, ref List<T> neighbors);

        public AStarResultCode FindPath(T start, T finish, ref List<T> path, 
            int maxOverload = 0)
        {
            path.Clear();
            _openedNodes.Clear();
            _allNodes.Clear();

            Start = start;
            Finish = finish;
            if (Arrived(Start, Finish))
            {
                // 如果起点==终点
                path.Add(Start);
                return AStarResultCode.SUCCESS;
            }
            else if (InPathCache(Start, Finish))
            {
                // 获得从neighbor到Finish的路径
                GetPathFromCache(Start, Finish, ref path);                
                return AStarResultCode.SUCCESS;
            }

            List<T> neighbors = new List<T>();            
            T curNode = Start;            
            _openedNodes.Add(curNode);
            _allNodes.Add(curNode);
                        
            while (true)
            {
                // 获得curNode邻接的所有“可达”节点
                Neighbors(curNode, ref neighbors);
                // 遍历cur节点的周边节点
                foreach (T neighbor in neighbors)
                {
                    if (neighbor.Status == AStarNodeStatus.CLOSED)
                    {
                        continue;
                    }
                    else if (neighbor.Status == AStarNodeStatus.OPENED)
                    {
                        // 如果neighbor已访问过
                        if (neighbor.G > GetCost(neighbor) + curNode.G)
                        {
                            // 如果neighbor之前的G值比当前新路径更大, 重置其Parent和G（即将其归入新路径）
                            neighbor.Parent = curNode;
                            neighbor.G = GetCost(neighbor) + curNode.G;
                            // binary heep mod
                            _openedNodes.OnChanged(neighbor);
                        }
                    }
                    else
                    {
                        // 如果neighbor尚未访问（即NONE状态）
                        neighbor.Status = AStarNodeStatus.OPENED;
                        neighbor.Parent = curNode;
                        neighbor.G = GetCost(neighbor) + curNode.G;
                        // 加入open列表
                        _openedNodes.Add(neighbor);
                        _allNodes.Add(neighbor);

                        if (Arrived(neighbor, Finish))
                        {
                            // 如果到达终点
                            FillPath(neighbor, ref path);
                            ResetAll();
                            return AStarResultCode.SUCCESS;
                        }
                        else if (maxOverload > 0 && _openedNodes.Count >= maxOverload)
                        {
                            // 如果寻路过载
                            ResetAll();
                            return AStarResultCode.FAIL_OVERLOAD;
                        }
                    }
                }

                // 遍历周边各节点之后，当前节点可以close
                curNode.Status = AStarNodeStatus.CLOSED;
                // 移出open列表
                _openedNodes.Pop();

                if (_openedNodes.Empty)
                {
                    // 无可到达路径
                    ResetAll();
                    return AStarResultCode.FAIL_NO_WAY;                    
                }
                else
                {
                    // 取最小F值node作为下一个cur节点                
                    _openedNodes.Peek(ref curNode);

                    // -----------如果有路径缓存--------------
                    if (InPathCache(curNode, Finish))
                    {
                        // 获得从curNode(不含)到Finish（含）的路径
                        GetPathFromCache(curNode, Finish, ref path);
                        int countFromCache = path.Count;
                        // 填入从Start（不含）到curNode（含）的路径
                        FillPath(curNode, ref path);                        
                        ResetAll();

                        // 为path中find到的部分节点修改缓存
                        path.Insert(0, Start);// 把起点添加进来作添加缓存处理
                        int countToAddInCache = path.Count - countFromCache - 1/*即curNode,因为curNode到Finish已经记录缓存了*/;
                        for (int i = 0; i < countToAddInCache; i++)
                        {
                            if (i+1 < countFromCache)
                            {
                                _pathCache[path[i].PathCacheIdx, path[i+1].PathCacheIdx] = path[i+1];
                                _pathCache[path[i+1].PathCacheIdx, path[i].PathCacheIdx] = path[i];
                            }

                            for (int j = i+2; j < path.Count; j++)
                            {
                                _pathCache[path[i].PathCacheIdx, path[j].PathCacheIdx] = path[j-1];
                                _pathCache[path[j].PathCacheIdx, path[i].PathCacheIdx] = path[j-1];
                            }
                        }

                        return AStarResultCode.SUCCESS;
                    }
                }                
            }
        }

        void ResetAll()
        {
            // 还原所有参与搜索的节点数据
            foreach (var node in _allNodes)
            {
                node.Status = AStarNodeStatus.NONE;
                node.Parent = null;
            }
        }

        // F = G + H
        float GetF(T node)
        {
            return node.G + GetH(node, Finish);
        }

        // 逆向导出最终路径。注意：得到的path=(Start,Finsih]，但添加的路径缓存=[Start,Finish]
        void FillPath(T node, ref List<T> path)
        {
            while (node.Parent != null)
            {
                if (_pathCache != null)
                {
                    // 如果使用缓存，则双向存入
                    T preNode = node.Parent;
                    _pathCache[preNode.PathCacheIdx, node.PathCacheIdx] = node;
                    _pathCache[node.PathCacheIdx, preNode.PathCacheIdx] = preNode;
                    
                    while (preNode.Parent != null)
                    {
                        _pathCache[preNode.Parent.PathCacheIdx, node.PathCacheIdx] = preNode;
                        _pathCache[node.PathCacheIdx, preNode.Parent.PathCacheIdx] = preNode;
                        preNode = preNode.Parent;
                    }
                }

                path.Insert(0, node);
                node = node.Parent;
            }
        }        
    }
}