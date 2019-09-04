using UnityEngine;
using System.Collections.Generic;

namespace Phenix.Unity.AI
{
    [System.Serializable]
    public class GlobalShareVariablesAsset : ScriptableObject
    {
        public List<string> shareVariableNames = new List<string>();
    }
}