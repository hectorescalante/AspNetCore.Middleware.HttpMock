using AspNetCore.Middleware.HttpMock.Core.Abstractions;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Middleware.HttpMock.Infrastructure
{
  internal class CacheStorage : IMockStorage
  {
    public Task<T> SaveAsync<T>(string key, T value)
    {
      throw new NotImplementedException();
    }

    public Task<T> GetAsync<T>(string key)
    {
      throw new NotImplementedException();
    }

    public Task DeleteAsync(string key)
    {
      throw new NotImplementedException();
    }
  }
}
