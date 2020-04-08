using System.Collections.Generic;
using UnityEngine;

namespace Phenix.Unity.Extend
{
    public static class ListExtend
    {
        // 基于著名的Knuth-Shuffle算法
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count-1; i >= 0; --i)
            {                
                int randIdx = Random.Range(0, i+1);
                T tmp = list[i];
                list[i] = list[randIdx];
                list[randIdx] = tmp;
            }
        }
    }
}