using AspNetCore.Middleware.HttpMock.Core.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Middleware.HttpMock.Infrastructure
{
  internal class CacheStorage : IMockStorage
  {
    private readonly IMemoryCache _memoryCache;

    public CacheStorage(IMemoryCache memoryCache) => _memoryCache = memoryCache;

    public Task<T> SaveAsync<T>(string key, T value)
    {
      return Task.FromResult(_memoryCache.Set(key, value));
    }

    public Task<T> GetAsync<T>(string key)
    {
      return Task.FromResult(_memoryCache.Get<T>(key));
    }

    public async Task DeleteAsync(string key)
    {
      await Task.Run(() => _memoryCache.Remove(key));
    }
  }
}
