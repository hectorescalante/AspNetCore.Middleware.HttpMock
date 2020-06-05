using AspNetCore.Middleware.HttpMock.Core;
using AspNetCore.Middleware.HttpMock.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Middleware.HttpMock.Infrastructure
{
  public class HttpMockMiddleware
  {
    private readonly RequestDelegate _requestDelegate;
    private readonly ILogger _logger;

    public HttpMockMiddleware(RequestDelegate requestDelegate, ILogger<HttpMockMiddleware> logger)
    {
      _requestDelegate = requestDelegate;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext, IHttpContextManager httpContextManager, IOptionsSnapshot<HttpMockOptions> mockOptions, IMockService mockService)
    {
      if (httpContextManager.ContainsHeader(mockOptions.Value.CreateKeyHeader()))
      {
        _logger.LogInformation("Creating key...");
        var requestKey = mockService.CreateKey(await httpContextManager.GetHttpMockRequestAsync());
        await httpContextManager.WriteResponseAsync(HttpStatusCode.OK, RequestHeaderValues.ContentTypeHeader(), requestKey);
        return;
      }
      else if (httpContextManager.ContainsHeader(mockOptions.Value.CreateMockHeader()))
      {
        _logger.LogInformation("Creating mock...");
        var requestMock = await httpContextManager.GetHttpMockRequestAsync();
        var requestKey = httpContextManager.GetHeaderValue(mockOptions.Value.RequestKeyHeader());
        var createdMock = await mockService.CreateMockAsync(requestKey.FromBase64String(), requestMock.ContentType, requestMock.BodyContent);

        await httpContextManager.WriteResponseAsync(HttpStatusCode.Created, createdMock.Response.ContentType, createdMock.Response.BodyContent.FromBase64String());
        return;
      }
      else if (httpContextManager.ContainsHeader(mockOptions.Value.DeleteMockHeader()))
      {
        _logger.LogInformation("Deleting mock...");
        await mockService.DeleteMockAsync(httpContextManager.GetHeaderValue(mockOptions.Value.RequestKeyHeader()));
        return;
      }
      else
      {
        _logger.LogInformation("Getting mock...");
        var requestMock = await httpContextManager.GetHttpMockRequestAsync();
        var mock = await mockService.GetMockAsync(requestMock);
        if (mock == null)
        {
          httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
          await mockService.CreateMockAsync(requestMock.GetRequestKey(), requestMock.ContentType, null);
          return;
        }

        await httpContextManager.WriteResponseAsync(HttpStatusCode.OK, mock.Response.ContentType, mock.Response.BodyContent.FromBase64String());

        return;
      }
    }
  }
}
