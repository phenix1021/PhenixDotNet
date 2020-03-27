using System;
using System.Collections.Generic;
using UnityEngine;
using Phenix.Unity.Pattern;

namespace Phenix.Unity.Collection
{
    /// <summary>
    /// GameObject的组件查询缓存。注意不适用于查询动态添加/删除的组件。
    /// </summary>
    public class ComponentCache : StandAloneSingleton<ComponentCache>
    {
        Dictionary<GameObject, Dictionary<Type, Component>> _gameObjectComponentMap = new Dictionary<GameObject, Dictionary<Type, Component>>();
        Dictionary<GameObject, Dictionary<Type, Component[]>> _gameObjectComponentsMap = new Dictionary<GameObject, Dictionary<Type, Component[]>>();
        Dictionary<GameObject, Dictionary<Type, Component>> _gameObjectParentComponentMap = new Dictionary<GameObject, Dictionary<Type, Component>>();
        Dictionary<GameObject, Dictionary<Type, Component[]>> _gameObjectParentComponentsMap = new Dictionary<GameObject, Dictionary<Type, Component[]>>();
        Dictionary<GameObject, Dictionary<Type, Component>> _gameObjectChildrenComponentMap = new Dictionary<GameObject, Dictionary<Type, Component>>();
        Dictionary<GameObject, Dictionary<Type, Component[]>> _gameObjectChildrenComponentsMap = new Dictionary<GameObject, Dictionary<Type, Component[]>>();

        List<GameObject> _toDel = new List<GameObject>();

        // 使用“查表法”从对象上查找指定Component，相较每次都从对象结构遍历要节省许多开销
        public T GetComponent<T>(GameObject target) where T : Component
        {
            Dictionary<Type, Component> typeComponentMap;
            Component targetComponent;
            // Return the cached component if it exists.
            if (_gameObjectComponentMap.TryGetValue(target, out typeComponentMap))
            {
                if (typeComponentMap.TryGetValue(typeof(T), out targetComponent))
                {
                    return targetComponent as T;
                }
            }
            else
            {
                // The cached component doesn't exist for the specified type.
                typeComponentMap = new Dictionary<Type, Component>();
                _gameObjectComponentMap.Add(target, typeComponentMap);
            }

            // Find the component reference and cache the results.
            targetComponent = target.GetComponent<T>();
            typeComponentMap.Add(typeof(T), targetComponent);
            return targetComponent as T;
        }        

        // 使用“查表法”从对象上查找指定Components，相较每次都从对象结构遍历要节省许多开销
        public T[] GetComponents<T>(GameObject target) where T : Component
        {
            Dictionary<Type, Component[]> typeComponentsMap;
            Component[] targetComponents;
            // Return the cached component if it exists.
            if (_gameObjectComponentsMap.TryGetValue(target, out typeComponentsMap))
            {
                if (typeComponentsMap.TryGetValue(typeof(T), out targetComponents))
                {
                    return targetComponents as T[];
                }
            }
            else
            {
                // The cached components doesn't exist for the specified type.
                typeComponentsMap = new Dictionary<Type, Component[]>();
                _gameObjectComponentsMap.Add(target, typeComponentsMap);
            }

            // Find the component reference and cache the results.
            targetComponents = target.GetComponents<T>();
            typeComponentsMap.Add(typeof(T), targetComponents);
            return targetComponents as T[];
        }        

        // 使用“查表法”从对象及其父对象上查找指定Component，相较每次都从对象结构遍历要节省许多开销
        public T GetParentComponent<T>(GameObject target) where T : Component
        {
            Dictionary<Type, Component> typeComponentMap;
            Component targetComponent;
            // Return the cached component if it exists.
            if (_gameObjectParentComponentMap.TryGetValue(target, out typeComponentMap))
            {
                if (typeComponentMap.TryGetValue(typeof(T), out targetComponent))
                {
                    return targetComponent as T;
                }
            }
            else
            {
                // The cached component doesn't exist for the specified type.
                typeComponentMap = new Dictionary<Type, Component>();
                _gameObjectParentComponentMap.Add(target, typeComponentMap);
            }

            // Find the component reference and cache the results.
            targetComponent = target.GetComponentInParent<T>();
            typeComponentMap.Add(typeof(T), targetComponent);
            return targetComponent as T;
        }        

        // 使用“查表法”从对象及其父对象上查找指定Components，相较每次都从对象结构遍历要节省许多开销
        public T[] GetParentComponents<T>(GameObject target) where T : Component
        {
            Dictionary<Type, Component[]> typeComponentsMap;
            Component[] targetComponents;
            // Return the cached component if it exists.
            if (_gameObjectParentComponentsMap.TryGetValue(target, out typeComponentsMap))
            {
                if (typeComponentsMap.TryGetValue(typeof(T), out targetComponents))
                {
                    return targetComponents as T[];
                }
            }
            else
            {
                // The cached components doesn't exist for the specified type.
                typeComponentsMap = new Dictionary<Type, Component[]>();
                _gameObjectParentComponentsMap.Add(target, typeComponentsMap);
            }

            // Find the component reference and cache the results.
            targetComponents = target.GetComponents<T>();
            typeComponentsMap.Add(typeof(T), targetComponents);
            return targetComponents as T[];
        }        

        // 使用“查表法”从对象及其子对象上查找指定Component，相较每次都从对象结构遍历要节省许多开销
        public T GetChildrenComponent<T>(GameObject target) where T : Component
        {
            Dictionary<Type, Component> typeComponentMap;
            Component targetComponent;
            // Return the cached component if it exists.
            if (_gameObjectChildrenComponentMap.TryGetValue(target, out typeComponentMap))
            {
                if (typeComponentMap.TryGetValue(typeof(T), out targetComponent))
                {
                    return targetComponent as T;
                }
            }
            else
            {
                // The cached component doesn't exist for the specified type.
                typeComponentMap = new Dictionary<Type, Component>();
                _gameObjectChildrenComponentMap.Add(target, typeComponentMap);
            }

            // Find the component reference and cache the results.
            targetComponent = target.GetComponentInChildren<T>();
            typeComponentMap.Add(typeof(T), targetComponent);
            return targetComponent as T;
        }        

        // 使用“查表法”从对象及其子对象上查找指定Components，相较每次都从对象结构遍历要节省许多开销
        public T[] GetChildrenComponents<T>(GameObject target) where T : Component
        {
            Dictionary<Type, Component[]> typeComponentsMap;
            Component[] targetComponents;
            // Return the cached component if it exists.
            if (_gameObjectChildrenComponentsMap.TryGetValue(target, out typeComponentsMap))
            {
                if (typeComponentsMap.TryGetValue(typeof(T), out targetComponents))
                {
                    return targetComponents as T[];
                }
            }
            else
            {
                // The cached components doesn't exist for the specified type.
                typeComponentsMap = new Dictionary<Type, Component[]>();
                _gameObjectChildrenComponentsMap.Add(target, typeComponentsMap);
            }

            // Find the component reference and cache the results.
            targetComponents = target.GetComponents<T>();
            typeComponentsMap.Add(typeof(T), targetComponents);
            return targetComponents as T[];
        }

        private void Update()
        {
            ClearNullCaches();
        }

        void ClearNullCaches()
        {
            /*
             GameObject被Destroy时，剔除其对应的cache
             */
            foreach (var item in _gameObjectComponentMap)
            {
                if (item.Key == null)
                {
                    _toDel.Add(item.Key);
                }
            }
            foreach (var del in _toDel)
            {
                _gameObjectComponentMap.Remove(del);
            }
            _toDel.Clear();

            foreach (var item in _gameObjectComponentsMap)
            {
                if (item.Key == null)
                {
                    _toDel.Add(item.Key);
                }
            }
            foreach (var del in _toDel)
            {
                _gameObjectComponentsMap.Remove(del);
            }
            _toDel.Clear();

            foreach (var item in _gameObjectParentComponentMap)
            {
                if (item.Key == null)
                {
                    _toDel.Add(item.Key);
                }
            }
            foreach (var del in _toDel)
            {
                _gameObjectParentComponentMap.Remove(del);
            }
            _toDel.Clear();

            foreach (var item in _gameObjectParentComponentsMap)
            {
                if (item.Key == null)
                {
                    _toDel.Add(item.Key);
                }
            }
            foreach (var del in _toDel)
            {
                _gameObjectParentComponentsMap.Remove(del);
            }
            _toDel.Clear();

            foreach (var item in _gameObjectChildrenComponentMap)
            {
                if (item.Key == null)
                {
                    _toDel.Add(item.Key);
                }
            }
            foreach (var del in _toDel)
            {
                _gameObjectChildrenComponentMap.Remove(del);
            }
            _toDel.Clear();

            foreach (var item in _gameObjectChildrenComponentsMap)
            {
                if (item.Key == null)
                {
                    _toDel.Add(item.Key);
                }
            }
            foreach (var del in _toDel)
            {
                _gameObjectChildrenComponentsMap.Remove(del);
            }
            _toDel.Clear();
        }

        public void ClearCaches()
        {
            _gameObjectComponentMap.Clear();
            _gameObjectComponentsMap.Clear();
            _gameObjectParentComponentMap.Clear();
            _gameObjectParentComponentsMap.Clear();
            _gameObjectChildrenComponentMap.Clear();
            _gameObjectChildrenComponentsMap.Clear();
        }

        public void ClearCaches(GameObject obj)
        {
            _gameObjectComponentMap.Remove(obj);
            _gameObjectComponentsMap.Remove(obj);
            _gameObjectParentComponentMap.Remove(obj);
            _gameObjectParentComponentsMap.Remove(obj);
            _gameObjectChildrenComponentMap.Remove(obj);
            _gameObjectChildrenComponentsMap.Remove(obj);
        }
    }
}