using UnityEngine;
using System.Collections.Generic;

namespace Phenix.Unity.AI.BT
{
    [System.Serializable]
    public class GlobalShareVariablesAsset : ScriptableObject
    {
        public List<string> shareVariableNames = new List<string>();
    }
}