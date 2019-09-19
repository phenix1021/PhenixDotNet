using System.Collections.Generic;
using Phenix.Unity.Extend;

namespace Phenix.Unity.AI
{   
    public abstract class AStar<NODE>
    {
        public enum ResultCode
        {
            NONE = 0,
            SUCCESS,
            FAIL_NO_WAY,        // 未发现可达路径
            FAIL_OVERLOAD,      // 搜寻节点数量超过上限
        }

        protected NODE Start { get; private set; }
        protected NODE Finish { get; private set; }

        Stack<NODE> _opened = new Stack<NODE>();
        List<NODE> _closed = new List<NODE>();

        protected virtual bool IsSame(NODE a, NODE b)
        {
            return a.Equals(b);
        }

        bool InOpened(NODE node)
        {
            foreach (var item in _opened)
            {
                if (IsSame(node, item))
                {
                    return true;
                }
            }
            return false;            
        }

        bool InClosed(NODE node)
        {
            /*foreach (var item in _closed)
            {
                if (IsSame(node, item))
                {
                    return true;
                }
            }
            return false;*/
            return InNodes(node, ref _closed);
        }

        bool InNodes(NODE node, ref List<NODE> nodes)
        {
            foreach (var item in nodes)
            {
                if (IsSame(node, item))
                {
                    return true;
                }
            }
            return false;
        }

        void Clear()
        {
            while (_opened.Count > 0)
            {
                Release(_opened.Pop());
            }
            foreach (var item in _closed)
            {
                Release(item);
            }
            _closed.Clear();
        }

        public ResultCode FindPath(NODE start, NODE finish, out List<NODE> path, int maxCount = 0)
        {
            Clear();

            Start = start;
            Finish = finish;
            
            path = new List<NODE>();
            List<NODE> neighbors = new List<NODE>();
            List<NODE> nextSteps = new List<NODE>();
            path.Add(start);
            NODE cur;
            do
            {
                if (maxCount > 0 && path.Count > maxCount)
                {
                    return ResultCode.FAIL_OVERLOAD;
                }
                cur = path[path.Count - 1];
                if (Arrived(cur))
                {
                    return ResultCode.SUCCESS;
                }
                Neighbors(cur, ref neighbors);                
                foreach (NODE t in neighbors)
                {
                    if (InOpened(t) || InClosed(t) || InNodes(t, ref path))
                    {
                        continue;
                    }
                    nextSteps.Add(t);
                }

                if (nextSteps.Count == 0)
                {
                    path.Remove(cur);
                    _closed.Add(cur);
                    if (_opened.Count == 0)
                    {
                        return ResultCode.FAIL_NO_WAY;
                    }
                    path.Add(_opened.Pop());
                    continue;
                }

                nextSteps.Sort((NODE a, NODE b) => { return (Cost(a) + Precedence(a)).CompareTo(Cost(b) + Precedence(b)); });
                UnityEngine.Debug.Log("nextSteps count: " + nextSteps.Count);                
                if (_opened.Count == 4)
                {
                    UnityEngine.Debug.Log("_opened count: " + _opened.Count);
                }
                for (int i = 0; i < nextSteps.Count; i++)
                {
                    if (i == 0)
                    {
                        path.Add(nextSteps[0]);
                        if (path.Count == 3)
                        {
                            UnityEngine.Debug.Log("path count: " + path.Count);
                        }
                        continue;
                    }
                    _opened.Push(nextSteps[i]);
                    if (maxCount > 0 && _opened.Count > maxCount)
                    {
                        return ResultCode.FAIL_OVERLOAD;
                    }
                }
                nextSteps.Clear();
            } while (true);            
        }
        
        protected virtual void Release(NODE node) { }
        protected abstract void Neighbors(NODE node, ref List<NODE> neighbors);
        protected abstract bool Arrived(NODE node);
        protected abstract int Cost(NODE node);

        protected abstract int Precedence(NODE node);
    }
}