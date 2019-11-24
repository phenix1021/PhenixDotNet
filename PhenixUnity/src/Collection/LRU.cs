using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phenix.Unity.Collection
{
    /// <summary>
    ///  LRU（最近最少使用）缓存，如可以用来管理prefab，超过一定时间没有使用则unload
    /// </summary>    
    public class LRU<K, E> where E : new()
    {
        class Data
        {
            public E entity;
            public float expireTimer = 0;

            public Data(E entity, float expireTimer)
            {
                this.entity = entity;
                this.expireTimer = expireTimer;
            }
        }

        // 数据表
        Dictionary<K, Data> _data = new Dictionary<K, Data>();
        List<K> _expiredList = new List<K>();

        // 过期时长
        float _expire;

        System.Action<E> _releaseAction;

        public LRU(float expire = 0 /*0表示持久*/, System.Action<E> releaseAction = null)
        {
            _expire = expire;
            _releaseAction = releaseAction;
        }

        public E Get(K key)
        {
            if (_data.ContainsKey(key) == false)
            {
                return default(E);
            }

            Data data = _data[key];

            if (_expire > 0)
            {
                data.expireTimer = Time.timeSinceLevelLoad + _expire;
            }            

            return data.entity;
        }

        public E Add(K key, E entity)
        {
            Data data = new Data(entity, _expire > 0 ? Time.timeSinceLevelLoad + _expire : 0);
            _data.Add(key, data);
            return data.entity;
        }

        bool IsExpired(Data data)
        {
            return data.expireTimer > 0 && Time.timeSinceLevelLoad > data.expireTimer;
        }

        public void OnUpdate()
        {
            _expiredList.Clear();

            foreach (var item in _data)
            {
                if (IsExpired(item.Value))
                {
                    _expiredList.Add(item.Key);
                }
            }

            foreach (var item in _expiredList)
            {
                if (_releaseAction != null)
                {
                    _releaseAction.Invoke(_data[item].entity);
                }                

                _data.Remove(item);                
            }
        }
    }
}
