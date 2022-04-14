using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JustAssets.Shared.UI
{
    public static class PooledUIListManager
    {
        private static Dictionary<string, IPooledUIList> _pools = new Dictionary<string, IPooledUIList>();

        public static PooledUIList<T> GetPool<T>(Func<T> initiator, Action<T> disabler = null, int defaultCacheSize = 5, Object template = null) where T : Component
        {
            var key = $"{typeof(T)}{template?.GetInstanceID()}";
            if (!_pools.TryGetValue(key, out var pool))
            {
                var instance = new PooledUIList<T>(initiator, disabler, defaultCacheSize);
                _pools[key] = instance;
                return instance;
            }

            return (PooledUIList<T>) pool;
        }

        /// <summary>
        /// Reuses objects which are no longer needed.
        /// </summary>
        /// <typeparam name="T">The type of the pooled MonoBehaviour.</typeparam>
        public class PooledUIList<T> : List<T>, IPooledUIList where T : Component
        {
            private readonly Func<T> _initiator;

            private readonly Action<T> _disabler;

            private readonly Stack<T> _cache = new Stack<T>();

            private readonly Transform _cacheContainer;

            private Transform _parent;

            /// <summary>
            /// Simple constructor.
            /// </summary>
            /// <param name="initiator">Creates a new instance.</param>
            /// <param name="disabler">Hides an existing instance. Can be overwritten. Do not destroy the instance but hide it.</param>
            /// <param name="defaultCacheSize">Amount of instances to create initially.</param>
            protected internal PooledUIList(Func<T> initiator, Action<T> disabler = null, int defaultCacheSize = 5)
            {
                _initiator = initiator ?? throw new ArgumentNullException(nameof(initiator));
                _disabler = disabler;

                string cacheName = "_Cache";
                string typeName = typeof(T).Name;

                var cacheFolder = GameObject.Find(cacheName)?.transform;
                if (cacheFolder is null)
                {
                    cacheFolder = new GameObject(cacheName).transform;
                    Object.DontDestroyOnLoad(cacheFolder);
                }

                _cacheContainer = cacheFolder.Find(typeName);
                if (_cacheContainer is null)
                {
                    var subFolder = new GameObject(typeName);
                    subFolder.transform.SetParent(cacheFolder);
                    _cacheContainer = subFolder.transform;
                }

                
                FillCache(defaultCacheSize);
            }

            /// <summary>
            /// Fills the cache with n instances.
            /// </summary>
            /// <param name="defaultCacheSize">The desired cache size.</param>
            private void FillCache(int defaultCacheSize)
            {
                for (int i = 0; i < defaultCacheSize; i++)
                {
                    T item = _initiator.Invoke();

                    if (_parent == null)
                        _parent = item.transform.parent;

                    item.gameObject.SetActive(false);
                    AddToCache(item, false);
                }
            }

            private void AddToCache(T item, bool runDisabler = true)
            {
                if (_cache.Contains(item))
                {
                    Debug.LogWarning("Removing item which was already removed.", item);
                    return;
                }

                if (runDisabler)
                    _disabler?.Invoke(item);

                item.gameObject.SetActive(false);

                _cache.Push(item);
                item.transform.SetParent(_cacheContainer, true);

                Remove(item);
            }

            /// <summary>
            /// Sets the amount of items in the list to show.
            /// </summary>
            /// <param name="count"></param>
            public void SetCount(int count)
            {
                while (count > Count)
                    Create();

                for (int i = Count - 1; i >= count; i--)
                {
                    T entry = this[i];

                    Destroy(entry);
                }
            }

            public T Create(Transform parent = null)
            {
                if (_cache.Count == 0)
                    FillCache(2);

                T item = PopFromCache();
                Add(item);

                GameObject itemGameObject = item.gameObject;
                Transform transform = itemGameObject.transform;
                transform.SetParent(parent ?? _parent, false);
                transform.localScale = Vector3.one;
                itemGameObject.SetActive(true);

                return item;
            }

            private T PopFromCache()
            {
                T item = _cache.Pop();

                item.transform.SetParent(null, true);

                return item;
            }

            public void Destroy(T item)
            {
                AddToCache(item);
            }

            public new void Clear()
            {
                SetCount(0);
            }
        }

        internal interface IPooledUIList 
        {
        }
    }
}