using System;
using System.Collections.Generic;

namespace Phenix.Unity.Extend
{
    public static class ListExtend
    {        
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count - i; ++i)
            {
                Random rand = new Random();
                int randIdx = rand.Next(list.Count - i);
                T tmp = list[randIdx];
                list[randIdx] = list[list.Count - i - 1];
                list[list.Count - i - 1] = tmp;
            }
        }
    }
}