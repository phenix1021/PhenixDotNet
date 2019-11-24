using System.Collections.Generic;
using UnityEngine;

namespace Phenix.Unity.Collection
{
    public class Pool<T> where T : new()
    {
        Queue<T> _cache = new Queue<T>();
        int _capacity = 0;

        public delegate void ResetDelegate(T obj);
        event ResetDelegate _resetEvent;

        float _expire = 0;
        float _expireTimer = 0;

        public Pool(int capacity = 50, ResetDelegate reset = null, float expire = 0/*0表示持久*/)
        {
            _capacity = capacity;
            _resetEvent += reset;
            _expire = expire;
            DelayExpire();
        }       

        void DelayExpire()
        {
            if (_expire == 0)
            {
                return;
            }
            _expireTimer = Time.timeSinceLevelLoad + _expire;
        }

        public void Collect(T t)
        {
            if (t == null)
            {
                return;
            }

            if (_capacity == 0 || _cache.Count < _capacity)
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
            DelayExpire();
            return inst;
        }

        bool IsExpired()
        {
            return _expireTimer > 0 && Time.timeSinceLevelLoad >= _expireTimer;
        }

        void HandleExpire()
        {
            if (IsExpired())
            {
                int releaseCount = _cache.Count / 2;
                while (_cache.Count > releaseCount)
                {
                    _cache.Dequeue();
                }
            }
        }

        public void OnUpdate()
        {
            HandleExpire();
        }
    }
}