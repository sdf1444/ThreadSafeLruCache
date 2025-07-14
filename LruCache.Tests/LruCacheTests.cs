using NUnit.Framework;
using LruCacheLib;

namespace LruCache.Tests
{
    public class LruCacheTests
    {
        [Test]
        public void AddAndGet_ShouldReturnValue()
        {
            var cache = new LruCache<string, int>(2);
            cache.Add("a", 1);

            // Verify that added item can be retrieved correctly
            Assert.AreEqual(1, cache.Get("a"));
        }

        [Test]
        public void Add_WhenFull_ShouldEvictLRU()
        {
            var cache = new LruCache<string, int>(2);
            cache.Add("a", 1);
            cache.Add("b", 2);

            // Access "a" to mark it as recently used
            cache.Get("a");

            // Adding "c" should evict the least recently used key "b"
            cache.Add("c", 3);

            Assert.IsTrue(cache.ContainsKey("a"));  // "a" should still be present
            Assert.IsFalse(cache.ContainsKey("b")); // "b" should be evicted
            Assert.IsTrue(cache.ContainsKey("c"));  // "c" should be added
        }

        [Test]
        public void EvictionCallback_ShouldBeTriggered()
        {
            string? evictedKey = null;
            int evictedValue = -1;

            var cache = new LruCache<string, int>(1);

            // Set eviction callback to capture evicted key/value
            cache.OnEviction = (k, v) =>
            {
                evictedKey = k;
                evictedValue = v;
            };

            cache.Add("x", 10);
            cache.Add("y", 20);  // Should evict "x"

            Assert.AreEqual("x", evictedKey);
            Assert.AreEqual(10, evictedValue);
        }

        [Test]
        public void Add_ExistingKey_ShouldUpdateValue_And_MarkAsRecentlyUsed()
        {
            var cache = new LruCache<string, int>(2);
            cache.Add("a", 1);
            cache.Add("b", 2);

            // Update "a" with a new value and mark as recently used
            cache.Add("a", 100);

            // Adding "c" should evict "b" since "a" was recently updated/accessed
            cache.Add("c", 3);

            Assert.IsTrue(cache.ContainsKey("a"));  // "a" should remain
            Assert.IsFalse(cache.ContainsKey("b")); // "b" should be evicted
            Assert.IsTrue(cache.ContainsKey("c"));  // "c" should be added

            // Verify updated value for "a"
            Assert.AreEqual(100, cache.Get("a"));
        }
    }
}