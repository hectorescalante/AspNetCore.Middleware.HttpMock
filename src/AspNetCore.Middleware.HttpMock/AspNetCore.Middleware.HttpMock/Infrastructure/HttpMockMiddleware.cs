using AspNetCore.Middleware.HttpMock.Core;
using AspNetCore.Middleware.HttpMock.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
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

    private IHttpContextManager ContextManager { get; set; }
    private HttpMockOptions MockOptions { get; set; }
    private IMockService MockService { get; set; }

    public async Task InvokeAsync(HttpContext httpContext, IOptionsSnapshot<HttpMockOptions> mockOptions, IMockService mockService)
    {
      ContextManager = new HttpContextManager(httpContext, mockOptions);
      MockOptions = mockOptions.Value;
      MockService = mockService;

      var mockAction = ContextManager.GetHeaderValue(MockOptions.ActionHeader());

      if (mockAction == RequestHeaderActions.CreateKey)
      {
        await CreateKeyAsync();
      }
      else if (mockAction == RequestHeaderActions.CreateMock)
      {
        await CreateMockAsync();
      }
      else if (mockAction == RequestHeaderActions.DeleteMock)
      {
        await DeleteMockAsync();
      }
      else
      {
        await ReturnMockAsync();
      }

      return;
    }

    private async Task CreateKeyAsync()
    {
      _logger.LogInformation("Creating key...");
      var requestMock = await ContextManager.GetHttpMockRequestAsync();
      var requestKey = MockService.CreateKey(requestMock);
      await ContextManager.WriteResponseAsync(HttpStatusCode.OK, "text/plain", requestKey);
    }

    private async Task CreateMockAsync()
    {
      _logger.LogInformation("Creating mock...");
      var requestMock = await ContextManager.GetHttpMockRequestAsync();
      var requestKey = ContextManager.GetHeaderValue(MockOptions.RequestKeyHeader());
      var createdMock = await MockService.CreateMockAsync(requestKey.FromBase64String(), requestMock.ContentType, requestMock.BodyContent);
      await ContextManager.WriteResponseAsync(HttpStatusCode.Created, createdMock.Response.ContentType, createdMock.Response.BodyContent.FromBase64String());
    }

    private async Task DeleteMockAsync()
    {
      _logger.LogInformation("Deleting mock...");
      await MockService.DeleteMockAsync(ContextManager.GetHeaderValue(MockOptions.RequestKeyHeader()));
    }

    private async Task ReturnMockAsync()
    {
      _logger.LogInformation("Getting mock...");
      var requestMock = await ContextManager.GetHttpMockRequestAsync();
      var mock = await MockService.GetMockAsync(requestMock);
      if (mock == null)
      {
        _logger.LogInformation("Mock not found!");
        await ContextManager.WriteResponseAsync(HttpStatusCode.NotFound, requestMock.ContentType, null);
        return;
      }
      await ContextManager.WriteResponseAsync(HttpStatusCode.OK, mock.Response.ContentType, mock.Response.BodyContent.FromBase64String());
    }
  }
}
