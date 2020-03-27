using System;
using System.Collections.Generic;

namespace Phenix.Unity.Collection
{
    /// <summary>
    /// 二叉堆
    /// </summary>
    public abstract class BinaryHeap<T>
    {
        List<T> _values = new List<T>();
        Comparison<T> _comparison;
        bool _minHeap;

        public BinaryHeap(Comparison<T> comparison, bool minHeap/*最小堆还是最大堆*/)
        {
            _comparison = comparison;
            _values.Add(default(T)); // 用一个无意义数据占据_values[0]，此位对二叉堆来说无用
            _minHeap = minHeap;
        }

        public void Add(List<T> valList)
        {
            foreach (var item in valList)
            {
                Add(item);
            }
        }

        public void Add(T t)
        {
            _values.Add(t);
            int idx = _values.Count - 1;
            int parentIdx = _values.Count / 2;
            while (parentIdx > 0)
            {                
                if ((_minHeap && _comparison(_values[idx], _values[parentIdx]) < 0) ||
                    (_minHeap == false && _comparison(_values[idx], _values[parentIdx]) > 0))
                {
                    // 如果最小堆且子节点 < 父节点，或者最大堆且子节点 > 父节点，则交换
                    Swap(idx, parentIdx);
                    idx = parentIdx;
                    parentIdx = parentIdx / 2;
                }
                else
                {
                    break;
                }
            }
        }
        
        void Swap(int idx1, int idx2)
        {
            T tmp = _values[idx1];
            _values[idx1] = _values[idx2];
            _values[idx2] = tmp;
        }

        public bool Empty { get { return Count == 0; } }
        public int Count { get { return _values.Count - 1; } }

        public void Pop()
        {
            if (Empty)
            {
                return;
            }

            // 尾位提前
            _values[1] = _values[_values.Count-1];
            // 移除尾位
            _values.RemoveAt(_values.Count-1);

            int parentIdx = 1;
            int childrenIdx1 = parentIdx * 2;
            int childrenIdx2 = childrenIdx1 + 1;
            while (true)
            {
                int childrenIdx = 0;
                if (childrenIdx2 < _values.Count)
                {
                    // 如果childrenIdx1, childrenIdx2都还没有到达_values末尾，则和二者较小(大)值比较
                    if ((_minHeap && _comparison(_values[childrenIdx1], _values[childrenIdx2]) < 0) ||
                        (_minHeap == false && _comparison(_values[childrenIdx1], _values[childrenIdx2]) > 0))
                    {
                        childrenIdx = childrenIdx1;
                    }
                    else
                    {
                        childrenIdx = childrenIdx2;
                    }                    
                }
                else if (childrenIdx1 < _values.Count)
                {
                    // 如果childrenIdx1未到达, childrenIdx2已到达_values末尾
                    childrenIdx = childrenIdx1;
                }
                else
                {
                    // childrenIdx1, childrenIdx1都已到达_values末尾
                    break;
                }

                if ((_minHeap && _comparison(_values[childrenIdx], _values[parentIdx]) < 0) ||
                    (_minHeap == false && _comparison(_values[childrenIdx], _values[parentIdx]) > 0))
                {
                    // 如果最小堆且子节点 < 父节点，或最大堆且子节点 > 父节点， 则交换
                    Swap(parentIdx, childrenIdx);

                    parentIdx = childrenIdx;
                    childrenIdx1 = parentIdx * 2;
                    childrenIdx2 = childrenIdx1 + 1;
                }          
                else
                {
                    break;
                }
            }
        }

        public bool Peek(out T ret)
        {
            if (Empty)
            {
                ret = default(T);
                return false;
            }
            ret = _values[1];
            return true;
        }

        public void Clear()
        {
            _values.Clear();
        }
    }

    /// <summary>
    /// 最小堆
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MinHeap<T> : BinaryHeap<T>
    {
        public MinHeap(Comparison<T> comparison):base(comparison, true) { }
    }

    /// <summary>
    /// 最大堆
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MaxHeap<T> : BinaryHeap<T>
    {
        public MaxHeap(Comparison<T> comparison) : base(comparison, false) { }
    }
}