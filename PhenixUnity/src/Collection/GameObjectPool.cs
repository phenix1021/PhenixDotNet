﻿using UnityEngine;
using System.Collections.Generic;

namespace Phenix.Unity.Collection
{
    public class GameObjectPool
    {
        Queue<GameObject> _cache = new Queue<GameObject>();
        int _capacity;
        GameObject _prefab;

        public GameObjectPool(int capacity, GameObject prefab)
        {
            _capacity = capacity;
            _prefab = prefab;
        }

        public void Collect(GameObject go)
        {
            _cache.Enqueue(go);
            if (_cache.Count >= _capacity)
            {
                GameObject.DestroyImmediate(_cache.Dequeue());
            }
        }

        public GameObject Get()
        {
            if (_cache.Count > 0)
            {
                return _cache.Dequeue();
            }
            return GameObject.Instantiate(_prefab);
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
    }
}