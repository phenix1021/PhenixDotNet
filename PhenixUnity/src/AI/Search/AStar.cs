using System.Collections.Generic;
using Phenix.Unity.Collection;

namespace Phenix.Unity.AI.SEARCH
{
    /* 寻路单元（T）必须实现AStarNode接口，因为AStar需要访问接口中的数据。
     这样的方式虽然对单元本身有一定侵入性，但能大大提高获取Parent、Status和G的效率。
     如果使用原始的寻路单元（T），就只能在AStar里添加map等结构保存Parent、Status和G，效率降低。
     无法使用内建结构（如Node）包含T的方法，因为遍历T的邻居时即便动态创建Node，也无法
     方便做到同一个T实例对应同一个Node*/
    public interface AStarNode<T> where T : class
    {
        T Parent { get; set; }                  // 父节点（由此构成最终路径）
        AStarNodeStatus Status { get; set; }    // 节点状态
        float G { get; set; }                   // 路径累计消耗，对应启发函数 F = G + H 中的G        
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

        public AStar()
        {
            _openedNodes = new MinHeap<T>((T a, T b) => { return GetF(a).CompareTo(GetF(b)); });
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

        public AStarResultCode FindPath(T start, T finish, ref List<T> path, int maxOverload = 0)
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

            List<T> neighbors = new List<T>();            
            T curNode = Start;            
            _openedNodes.Add(curNode);
            _allNodes.Add(curNode);
                        
            while (true)
            {
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
                        if (neighbor.G > GetCost(neighbor) + curNode.G)
                        {
                            // 如果新路径的G值较小
                            neighbor.Parent = curNode;
                            neighbor.G = GetCost(neighbor) + curNode.G;
                        }
                    }
                    else
                    {
                        // 如果是NONE状态
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
                            ResetAll();
                            return AStarResultCode.FAIL_OVERLOAD;
                        }
                    }
                }

                // 遍历周边各节点之后，当前节点可以close
                curNode.Status = AStarNodeStatus.CLOSED;
                // 移出open列表
                _openedNodes.Pop();
                if (_openedNodes.Count == 0)
                {
                    // 无可到达路径
                    ResetAll();
                    return AStarResultCode.FAIL_NO_WAY;                    
                }

                // 取最小F值node作为新的cur节点
                T ret;
                _openedNodes.Peek(out ret);
                curNode = ret;
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

        // 逆向导出最终路径
        void FillPath(T node, ref List<T> path)
        {
            while (node.Parent != null)
            {
                path.Insert(0, node);
                node = node.Parent;
            }
        }        
    }
}