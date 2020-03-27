using UnityEngine;
using Phenix.Unity.AI.SEARCH;
using System.Collections.Generic;

namespace Phenix.Unity.Grid
{
    public abstract class HexTileComponent : MonoBehaviour//, AStarNode<HexTileComponent>
    {        
        public HexTile tile;
        /*
        public HexTileComponent Parent { get; set; }
        public AStarNodeStatus Status { get; set; }
        public float G { get; set; }*/
    }
}