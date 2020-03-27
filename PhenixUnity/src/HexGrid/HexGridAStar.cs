using Phenix.Unity.AI.SEARCH;
using System.Collections.Generic;

namespace Phenix.Unity.Grid
{
    /// <summary>
    /// A* for HexGrid
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HexGridAStar<T> : AStar<T> where T : HexTileComponent, AStarNode<T>
    {
        HexGridComponent<T> _grid;

        public HexGridAStar(HexGridComponent<T> grid)
        {
            _grid = grid;
        }

        protected override bool Arrived(T node, T finish)
        {
            return node == finish;
        }

        protected override float GetCost(T node)
        {
            return 1;
        }

        protected override void Neighbors(T node, ref List<T> neighbors)
        {
            _grid.Neighbors(node, ref neighbors, true);
        }

        protected override float GetH(T node, T finish)
        {
            return _grid.Distance(node, finish);
        }
    }
}