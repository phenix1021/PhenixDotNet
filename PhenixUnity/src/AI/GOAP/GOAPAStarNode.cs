using Phenix.Unity.AI.SEARCH;

namespace Phenix.Unity.AI.GOAP
{
    public class GOAPAStarNode : AStarNode<GOAPAStarNode>
    {
        public WorldState nodeWS;
        //public int goalType;
        public GOAPAction goapAction;

        public GOAPAStarNode Parent { get; set; }
        public AStarNodeStatus Status { get; set; }
        public float G { get; set; }
        public int PathCacheIdx { get; set; }
    }
}