using AspNetCore.Middleware.HttpMock.Core.DomainModels;
using System.Threading.Tasks;

namespace AspNetCore.Middleware.HttpMock.Core.Abstractions
{
  public interface IMockService
  {
    string CreateKey(HttpMockRequest mockRequest);
    Task<MockInstance> CreateMockAsync(string requestKey, string contentType, string bodyContent);
    Task DeleteMockAsync(string requestKey);
    Task<MockInstance> GetMockAsync(HttpMockRequest mockRequest);

  }
}
