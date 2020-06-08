using AspNetCore.Middleware.HttpMock.Core;
using AspNetCore.Middleware.HttpMock.Core.Abstractions;
using AspNetCore.Middleware.HttpMock.Core.DomainModels;
using Microsoft.AspNetCore.Http;
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
  public class HttpContextManager : IHttpContextManager
  {
    private readonly HttpContext _httpContext;
    private readonly HttpMockOptions _mockOptions;

    public HttpContextManager(HttpContext httpContext, IOptionsSnapshot<HttpMockOptions> mockOptions)
    {
      _httpContext = httpContext;
      _mockOptions = mockOptions.Value;
    }

    //public static IHttpContextManager Create(HttpContext httpContext, IOptionsSnapshot<HttpMockOptions> mockOptions)
    //{
    //  return new HttpContextManager(httpContext, mockOptions);
    //}

    public async Task<HttpMockRequest> GetHttpMockRequestAsync()
    {
      var hostOverride = GetHeaderValue(_mockOptions.HostHeader());

      return new HttpMockRequest()
      {
        Headers = _httpContext.Request.Headers,
        IsHttps = _httpContext.Request.IsHttps,
        Method = _httpContext.Request.Method,
        Host = hostOverride ?? _httpContext.Request.Host.ToString(),
        Path = _httpContext.Request.Path,
        QueryString = _httpContext.Request.QueryString.ToString(),
        ContentType = _httpContext.Request.ContentType,
        BodyContent = await GetRequestBodyAsync(_httpContext.Request, _httpContext.RequestAborted)
      };
    }
    private async Task<string> GetRequestBodyAsync(HttpRequest httpRequest, CancellationToken cancellationToken)
    {
      httpRequest.EnableBuffering();
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

    public async Task WriteResponseAsync(HttpStatusCode statusCode, string contentType, string content)
    {
      _httpContext.Response.StatusCode = (int)statusCode;
      _httpContext.Response.ContentType = contentType;
      await _httpContext.Response.WriteAsync(content ?? string.Empty, Encoding.UTF8);
    }

    public bool ContainsHeader(string headerName) => _httpContext.Request.Headers.ContainsKey(headerName);
    public string GetHeaderValue(string headerName)
    {
      var headers = _httpContext.Request.Headers as IEnumerable<KeyValuePair<string, StringValues>>;
      var header = headers?.FirstOrDefault(h => h.Key == headerName);
      return header?.Value.ToString();
    }

  }
}
