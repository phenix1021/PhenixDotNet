using System.Collections.Generic;
using UnityEngine;

namespace Phenix.Unity.Extend
{
    public static class ListExtend
    {        
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int randIdx1 = Random.Range(0, list.Count);
                int randIdx2 = Random.Range(0, list.Count);
                T tmp = list[randIdx1];
                list[randIdx1] = list[randIdx2];
                list[randIdx2] = tmp;
            }
        }
    }
}