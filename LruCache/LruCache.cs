using System;
using System.Collections.Generic;

namespace LruCacheLib
{
    /// <summary>
    /// Thread-safe, fixed-capacity Least Recently Used (LRU) cache.
    /// Evicts least recently accessed items when capacity is reached.
    /// </summary>
    public class LruCache<K, V>
    {
        private readonly int _capacity;
        private readonly Dictionary<K, LinkedListNode<CacheItem>> _cacheMap;
        private readonly LinkedList<CacheItem> _usageOrder;
        private readonly object _syncRoot = new();

        /// <summary>
        /// Optional callback invoked when an item is evicted from the cache.
        /// </summary>
        public Action<K, V>? OnEviction { get; set; }

        public LruCache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero.");

            _capacity = capacity;

            // Dictionary for fast O(1) lookups by key
            // LinkedList maintains usage order: most recently used at front, least at end
            _cacheMap = new Dictionary<K, LinkedListNode<CacheItem>>(capacity);
            _usageOrder = new LinkedList<CacheItem>();
        }

        public void Add(K key, V value)
        {
            lock (_syncRoot)  // Ensure thread-safe access
            {
                if (_cacheMap.TryGetValue(key, out var node))
                {
                    // Key exists: update value and move node to front (most recently used)
                    node.Value.Value = value;
                    _usageOrder.Remove(node);
                    _usageOrder.AddFirst(node);
                    return;
                }

                // Cache is full: remove least recently used item (at tail of linked list)
                if (_cacheMap.Count >= _capacity)
                {
                    var lru = _usageOrder.Last!;
                    _usageOrder.RemoveLast();
                    _cacheMap.Remove(lru.Value.Key);

                    // Invoke eviction callback if set
                    OnEviction?.Invoke(lru.Value.Key, lru.Value.Value);
                }

                // Add new item to front (most recently used)
                var newItem = new CacheItem(key, value);
                var newNode = new LinkedListNode<CacheItem>(newItem);
                _usageOrder.AddFirst(newNode);
                _cacheMap[key] = newNode;
            }
        }

        public V Get(K key)
        {
            lock (_syncRoot)
            {
                if (!_cacheMap.TryGetValue(key, out var node))
                    throw new KeyNotFoundException($"Key '{key}' not found.");

                // Move accessed node to front (mark as most recently used)
                _usageOrder.Remove(node);
                _usageOrder.AddFirst(node);

                return node.Value.Value;
            }
        }

        public bool TryGet(K key, out V? value)
        {
            lock (_syncRoot)
            {
                if (_cacheMap.TryGetValue(key, out var node))
                {
                    // Move accessed node to front (most recently used)
                    _usageOrder.Remove(node);
                    _usageOrder.AddFirst(node);
                    value = node.Value.Value;
                    return true;
                }

                value = default;
                return false;
            }
        }

        public bool ContainsKey(K key)
        {
            lock (_syncRoot)
            {
                return _cacheMap.ContainsKey(key);
            }
        }

        // Represents a cache entry pairing key and value
        private class CacheItem
        {
            public K Key { get; }
            public V Value { get; set; }
            public CacheItem(K key, V value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}