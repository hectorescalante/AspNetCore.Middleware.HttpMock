using AspNetCore.Middleware.HttpMock.Core;
using AspNetCore.Middleware.HttpMock.Core.DomainModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

    public async Task InvokeAsync(HttpContext httpContext, IOptionsSnapshot<HttpMockOptions> mockOptions, HttpMockService mockService)
    {
      var requestHeaders = httpContext.Request.Headers;
      var headerPrefix = mockOptions.Value.RequestHeaderPrefix;
      httpContext.Request.EnableBuffering();

      if (requestHeaders.CreateKey(headerPrefix))
      {
        _logger.LogInformation("Creating key...");
        var requestKey = mockService.CreateKey(await GetRequestInfoAsync(httpContext, GetHeaderValue(requestHeaders, RequestHeaderValues.Host(mockOptions.Value.RequestHeaderPrefix))));
        await httpContext.Response.WriteAsync(requestKey, Encoding.UTF8);
        return;
      }
      else if (requestHeaders.CreateMock(headerPrefix))
      {
        _logger.LogInformation("Creating mock...");
        var request = await GetRequestInfoAsync(httpContext);
        var requestKey = GetHeaderValue(requestHeaders, RequestHeaderValues.RequestKey(headerPrefix));
        var createdMock = await mockService.CreateMock(requestKey.FromBase64String(), request.ContentType, request.BodyContent);

        httpContext.Response.StatusCode = (int)HttpStatusCode.Created;
        httpContext.Response.ContentType = createdMock.Response.ContentType;
        await httpContext.Response.WriteAsync(createdMock.Response.BodyContent.FromBase64String(), Encoding.UTF8);
        return;
      }
      else if (requestHeaders.DeleteMock(headerPrefix))
      {
        _logger.LogInformation("Deleting mock...");
        await mockService.DeleteMock(GetHeaderValue(requestHeaders, RequestHeaderValues.RequestKey(headerPrefix)));
        return;
      }
      else
      {
        _logger.LogInformation("Getting mock...");
        var request = await GetRequestInfoAsync(httpContext, GetHeaderValue(requestHeaders, RequestHeaderValues.Host(headerPrefix)));
        var mock = await mockService.GetMock(request);
        if (mock == null)
        {
          httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
          await mockService.CreateMock(request.GetRequestKey(), request.ContentType, null);
          return;
        }

        httpContext.Response.ContentType = mock.Response.ContentType;
        await httpContext.Response.WriteAsync(mock.Response.BodyContent.FromBase64String(), Encoding.UTF8);

        return;
      }
    }

    private string GetHeaderValue(ICollection<KeyValuePair<string, StringValues>> headers, string headerName)
    {
      var header = headers?.FirstOrDefault(h => h.Key == headerName);
      return header?.Value.ToString();
    }
    private async Task<MockRequest> GetRequestInfoAsync(HttpContext httpContext, string hostOverride = null)
    {
      return new MockRequest()
      {
        Headers = httpContext.Request.Headers,
        IsHttps = httpContext.Request.IsHttps,
        Method = httpContext.Request.Method,
        Host = hostOverride ?? httpContext.Request.Host.ToString(),
        Path = httpContext.Request.Path,
        QueryString = httpContext.Request.QueryString.ToString(),
        ContentType = httpContext.Request.ContentType,
        BodyContent = await GetRequestBodyAsync(httpContext.Request, httpContext.RequestAborted)
      };
    }
    private async Task<string> GetRequestBodyAsync(HttpRequest httpRequest, CancellationToken cancellationToken)
    {
      var bodyReader = httpRequest.BodyReader;
      var bodyString = "";

      while (!cancellationToken.IsCancellationRequested)
      {
        var bodyReadResult = await bodyReader.ReadAsync();
        var buffer = bodyReadResult.Buffer;

        bodyString = StringCreate(buffer);

        bodyReader.AdvanceTo(buffer.Start, buffer.End);

        if (bodyReadResult.IsCompleted) break;
      }

      return bodyString;
    }
    private static string StringCreate(in ReadOnlySequence<byte> readOnlySequence)
    {
      // Separate method because Span/ReadOnlySpan cannot be used in async methods
      ReadOnlySpan<byte> span = readOnlySequence.IsSingleSegment ? readOnlySequence.First.Span : readOnlySequence.ToArray().AsSpan();
      return (Encoding.UTF8.GetString(span));
    }


  }
}
