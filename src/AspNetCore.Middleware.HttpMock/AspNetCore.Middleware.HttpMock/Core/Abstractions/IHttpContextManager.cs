using AspNetCore.Middleware.HttpMock.Core.DomainModels;
using System.Net;
using System.Threading.Tasks;

namespace AspNetCore.Middleware.HttpMock.Core.Abstractions
{
  public interface IHttpContextManager
  {
    Task<HttpMockRequest> GetHttpMockRequestAsync();
    Task WriteResponseAsync(HttpStatusCode statusCode, string contentType, string content);
    bool ContainsHeader(string headerName);
    string GetHeaderValue(string headerName);
  }
}
