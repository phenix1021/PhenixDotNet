using UnityEngine;
using System.Collections.Generic;

namespace Phenix.Unity.Collection
{
    public class GameObjectPool
    {
        Queue<GameObject> _cache = new Queue<GameObject>();
        int _capacity;
        GameObject _prefab;

        float _expire = 0;
        float _expireTimer = 0;

        public GameObjectPool(int capacity, GameObject prefab, float expire = 0/*0表示持久*/)
        {
            _capacity = capacity;
            _prefab = prefab;
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

        public void Collect(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            if (_capacity == 0 || _cache.Count < _capacity)
            {
                _cache.Enqueue(go);
            }
            else
            {
                GameObject.DestroyImmediate(go);
            }            
        }

        public GameObject Get()
        {
            DelayExpire();
            if (_cache.Count > 0)
            {
                return _cache.Dequeue();
            }            
            return GameObject.Instantiate(_prefab);
        }

        public GameObject Get(out bool isNewInst/*是否新创建的实例*/)
        {
            isNewInst = (_cache.Count == 0);
            return Get();
        }

        public void Destroy()
        {
            while (_cache.Count > 0)
            {
                GameObject.DestroyImmediate(_cache.Dequeue());
            }
            if (_prefab != null)
            {
                GameObject.DestroyImmediate(_prefab, true);
            }
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
                    GameObject.DestroyImmediate(_cache.Dequeue());
                }
            }
        }

        public void OnUpdate()
        {
            HandleExpire();
        }
    }
}