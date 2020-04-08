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
        /* 注意：T为引用类型时，hashset的哈希值只和T地址有关，和是否重载==、Equals无关
        T为值类型时，hashset的哈希值由Equals决定*/
        HashSet<T> _checkRepeat = new HashSet<T>();
        Comparison<T> _comparison;
        bool _ascentOrder;

        public BinaryHeap(Comparison<T> comparison, bool ascentOrder/*最小堆还是最大堆*/)
        {
            _comparison = comparison;
            /* 用一个无意义数据占据_values[0]，二叉堆从数组下标1开始递增，
            这样左子节点下标 = 父节点下标*2， 右子节点下标 = 父节点*2+1。
            而父节点下标 = 子节点下标/2。*/
            _values.Add(default(T));
            _ascentOrder = ascentOrder;
        }

        public void Add(List<T> valList)
        {
            foreach (var item in valList)
            {
                Add(item);
            }
        }

        public bool Add(T t)
        {
            if (_checkRepeat.Contains(t))
            {
                // 已存在t
                return false;
            }
            
            _values.Add(t);
            _checkRepeat.Add(t);
            AdjustUp(_values.Count - 1);
            return true;
        }
        
        void Swap(int idx1, int idx2)
        {
            T tmp = _values[idx1];
            _values[idx1] = _values[idx2];
            _values[idx2] = tmp;
        }

        public bool Empty { get { return Count == 0; } }
        public int Count { get { return _values.Count - 1/*扣去1个_values[0]占位符*/; } }

        public void Pop()
        {
            if (Empty)
            {
                return;
            }

            // 尾位提前
            _checkRepeat.Remove(_values[1]);
            _values[1] = _values[_values.Count-1];
            // 移除尾位
            _values.RemoveAt(_values.Count-1);

            AdjustDown(1);            
        }

        public bool Peek(ref T ret)
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
            /* 用一个无意义数据占据_values[0]，二叉堆从数组下标1开始递增，
            这样左子节点下标 = 父节点下标*2， 右子节点下标 = 父节点*2+1。
            而父节点下标 = 子节点下标/2。*/
            _values.Add(default(T));
            _checkRepeat.Clear();
        }

        int GetChildIdx(int childIdx1, int childIdx2)
        {
            if (childIdx2 < _values.Count)
            {
                // 如果childrenIdx1, childrenIdx2都还没有到达_values末尾，则和二者较小(大)值比较
                if ((_ascentOrder && _comparison(_values[childIdx1], _values[childIdx2]) < 0) ||
                    (_ascentOrder == false && _comparison(_values[childIdx1], _values[childIdx2]) > 0))
                {
                    return childIdx1;
                }
                else
                {
                    return childIdx2;
                }
            }
            else if (childIdx1 < _values.Count)
            {
                // 如果childrenIdx1未到达, childrenIdx2已到达_values末尾
                return childIdx1;
            }
            else
            {
                // childrenIdx1, childrenIdx1都已到达_values末尾
                return -1;
            }
        }

        void Adjust(int idx)
        {
            if (AdjustUp(idx) == false)
            {
                AdjustDown(idx);
            }
        }

        bool AdjustUp(int idx)
        {
            if (idx <= 1 || idx > _values.Count - 1)
            {
                return false;
            }

            bool ret = false;
            int parentIdx = idx / 2;
            while (parentIdx > 0)
            {
                if ((_ascentOrder && _comparison(_values[idx], _values[parentIdx]) < 0) ||
                    (_ascentOrder == false && _comparison(_values[idx], _values[parentIdx]) > 0))
                {
                    // 如果最小堆且子节点 < 父节点，或者最大堆且子节点 > 父节点，则交换
                    Swap(idx, parentIdx);
                    idx = parentIdx;
                    parentIdx = parentIdx / 2;
                    ret = true;
                }
                else
                {
                    break;
                }
            }
            return ret;
        }

        bool AdjustDown(int idx)
        {
            if (idx < 1 || idx >= _values.Count - 1)
            {
                return false;
            }

            bool ret = false;
            int parentIdx = idx;
            int childIdx1 = parentIdx * 2;
            int childIdx2 = childIdx1 + 1;
            while (true)
            {
                int childIdx = GetChildIdx(childIdx1, childIdx2);
                if (childIdx == -1)
                {
                    break;
                } 

                if ((_ascentOrder && _comparison(_values[childIdx], _values[parentIdx]) < 0) ||
                    (_ascentOrder == false && _comparison(_values[childIdx], _values[parentIdx]) > 0))
                {
                    // 如果最小堆且子节点 < 父节点，或最大堆且子节点 > 父节点， 则交换
                    Swap(parentIdx, childIdx);

                    parentIdx = childIdx;
                    childIdx1 = parentIdx * 2;
                    childIdx2 = childIdx1 + 1;
                    ret = true;
                }
                else
                {
                    break;
                }
            }
            return ret;
        }


        int Find(T val)
        {
            for (int i = 1; i < _values.Count; i++)
            {
                if (_values[i].Equals(val))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool Replace(T oldVal, T newVal)
        {
            if (oldVal.Equals(newVal))
            {
                // 新旧相等
                return false;
            }

            if (_checkRepeat.Contains(oldVal) == false)
            {
                // 不存在旧值
                return false;
            }

            if (_checkRepeat.Contains(newVal))
            {
                // 已存在新值
                return false;
            }

            int idx = Find(oldVal);
            _values[idx] = newVal;
            Adjust(idx);

            _checkRepeat.Remove(oldVal);
            _checkRepeat.Add(newVal);
            return true;
        }

        public void OnChanged(T t)
        {
            if (_checkRepeat.Contains(t) == false)
            {
                // 不存在t
                return;
            }

            Adjust(Find(t));
        }

        public bool Remove(T t)
        {
            if (_checkRepeat.Contains(t) == false)
            {
                // 不存在t
                return false;
            }

            int idx = Find(t);
            _values[idx] = _values[_values.Count - 1];
            _values.RemoveAt(_values.Count - 1);
            Adjust(idx);
            _checkRepeat.Remove(t);
            return true;
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