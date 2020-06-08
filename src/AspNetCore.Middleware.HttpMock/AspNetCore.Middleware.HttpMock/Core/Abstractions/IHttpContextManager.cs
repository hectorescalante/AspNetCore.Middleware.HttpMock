using AspNetCore.Middleware.HttpMock.Core.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System.Threading.Tasks;

namespace AspNetCore.Middleware.HttpMock.Core.Abstractions
{
  public interface IHttpContextManager
  {
    //IHttpContextManager Create(HttpContext httpContext, IOptionsSnapshot<HttpMockOptions> mockOptions);
    Task<HttpMockRequest> GetHttpMockRequestAsync();
    Task WriteResponseAsync(HttpStatusCode statusCode, string contentType, string content);
    bool ContainsHeader(string headerName);
    string GetHeaderValue(string headerName);
  }
}
