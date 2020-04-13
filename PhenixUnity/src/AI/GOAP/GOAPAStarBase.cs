using System.Collections.Generic;
using Phenix.Unity.AI.SEARCH;
using Phenix.Unity.Extend;

namespace Phenix.Unity.AI.GOAP
{
    public abstract class GOAPAStarBase : AStar<GOAPAStarNode>
    {
        GOAPGoal _goal;                 // 寻路目标
        GOAPAStarNode _start;

        List<GOAPAStarNode> _nodes = new List<GOAPAStarNode>(); // 寻路节点
        List<GOAPAStarNode> _nodePath = new List<GOAPAStarNode>();

        public GOAPAStarBase(List<GOAPAction> actions)
        {
            _start = new GOAPAStarNode();            
            _start.goapAction = null;

            foreach (var goapAction in actions)
            {
                GOAPAStarNode node = new GOAPAStarNode();
                node.goapAction = goapAction;
                _nodes.Add(node);
            }
        }

        public AStarResultCode FindPath(GOAPGoal goal, WorldState curWorldState,
            ref List<GOAPAction> path, int maxOverload = 0)
        {
            path.Clear();
            _goal = goal;
            _nodes.Shuffle();
            curWorldState.CopyTo(ref _start.nodeWS);

            AStarResultCode ret = FindPath(_start, null, ref _nodePath, maxOverload);
            if (AStarResultCode.SUCCESS == ret)
            {
                foreach (var node in _nodePath)
                {
                    path.Add(node.goapAction);
                }
            }

            return ret;
        }

        protected override void OnNodeReset(GOAPAStarNode node)
        {
            base.OnNodeReset(node);
        }

        protected override void OnOpenedNodeChanged(GOAPAStarNode node, GOAPAStarNode cur)
        {
            OnOpenedNodeAdded(node, cur);
        }

        protected override void OnOpenedNodeAdded(GOAPAStarNode node, GOAPAStarNode cur)
        {
            cur.nodeWS.CopyTo(ref node.nodeWS);
            if (node.goapAction != null)
            {
                node.goapAction.ApplyWorldStateEffect(node.nodeWS);
            }            
        }

        protected sealed override void Neighbors(GOAPAStarNode curNode, ref List<GOAPAStarNode> neighbors)
        {
            neighbors.Clear();
            foreach (var node in _nodes)
            {
                if (node == curNode)
                {
                    continue;
                }

                // 检测当前node是否满足action的前置条件
                if (node.goapAction.CheckWorldStatePrecondition(curNode.nodeWS))
                {
                    neighbors.Add(node);
                }
            }
        }

        protected sealed override float GetCost(GOAPAStarNode node)
        {
            return GetCost(node.goapAction, _goal);
        }

        protected sealed override float GetH(GOAPAStarNode node, GOAPAStarNode finish)
        {
            return GetH(node.goapAction, _goal);
        }

        protected sealed override bool Arrived(GOAPAStarNode node, GOAPAStarNode finish)
        {
            if (node == null)
            {
                return false;
            }
            return _goal.IsFinished(node.nodeWS);
        }

        protected virtual float GetCost(GOAPAction action, GOAPGoal goal)
        {
            return 1;
        }

        protected virtual float GetH(GOAPAction action, GOAPGoal goal)
        {
            return 0;
        }
    }
}