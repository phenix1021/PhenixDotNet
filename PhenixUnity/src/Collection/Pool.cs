using System.Collections.Generic;

namespace Phenix.Unity.Collection
{
    public class Pool<T> where T : new()
    {
        Queue<T> _cache = new Queue<T>();
        int _capacity = 0;

        public delegate void ResetDelegate(T obj);
        event ResetDelegate _resetEvent;

        public Pool(int capacity = 50, ResetDelegate reset = null)
        {
            _capacity = capacity;
            _resetEvent += reset;
        }       

        public void Collect(T t)
        {            
            if (_capacity > 0 && _cache.Count < _capacity)
            {
                _cache.Enqueue(t);
            }
        }

        public T Get()
        {
            T inst;
            if (_cache.Count > 0)
            {
                inst = _cache.Dequeue();
            }
            else
            {
                inst = new T();
            }
            if (_resetEvent != null)
            {
                _resetEvent.Invoke(inst);
            }
            return inst;
        }
    }
}