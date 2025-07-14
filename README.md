# ThreadSafeLruCache

A simple, thread-safe Least Recently Used (LRU) cache implementation in C#.

This cache keeps a fixed number of items, and when it’s full, it automatically removes the least recently used item to make space for new ones. It’s safe to use across multiple threads and includes an optional callback to notify you when items are evicted.

---

## Features

- Thread-safe for concurrent access  
- Fixed capacity with automatic eviction of least recently used entries  
- Supports updating existing keys and marks them as recently used  
- Optional eviction callback for custom handling when items are removed  
- Unit tests included using NUnit

---

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download) or newer  
- NUnit (included as a test dependency)

### Clone and Build

bash
git clone https://github.com/yourusername/ThreadSafeLruCache.git
cd ThreadSafeLruCache
dotnet build

---

## Usage Example

```csharp
var cache = new LruCache<string, int>(2);
cache.Add("a", 1);
cache.Add("b", 2);
cache.Get("a");
cache.Add("c", 3); // "b" will be evicted

Console.WriteLine(cache.ContainsKey("b")); // false
```

---

## Contributing

Feel free to open an issue or submit a pull request if you find something to improve.

---

## Development Time

Total time spent: **~3 hours**

This includes design, implementation, writing unit tests, documentation, and Git setup.

