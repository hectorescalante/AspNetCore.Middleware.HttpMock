using AspNetCore.Middleware.HttpMock.Core.DomainModels;
using System.Threading.Tasks;

namespace AspNetCore.Middleware.HttpMock.Core.Abstractions
{
  public interface IMockRepository
  {
    Task<MockInstance> SaveAsync(string key, MockInstance mockInstance);
    Task<MockInstance> GetAsync(string key);
    Task DeleteAsync(string key);

  }
}
