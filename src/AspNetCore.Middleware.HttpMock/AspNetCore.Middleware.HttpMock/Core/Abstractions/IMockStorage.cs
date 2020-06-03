using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Middleware.HttpMock.Core.Abstractions
{
  public interface IMockStorage
  {
    Task<T> SaveAsync<T>(string key, T value);
    Task<T> GetAsync<T>(string key);
    Task DeleteAsync(string key);
  }
}
