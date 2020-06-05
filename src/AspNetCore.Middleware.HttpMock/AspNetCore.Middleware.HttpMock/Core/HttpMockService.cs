using AspNetCore.Middleware.HttpMock.Core.Abstractions;
using AspNetCore.Middleware.HttpMock.Core.DomainModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Middleware.HttpMock.Core
{
  public class HttpMockService : IMockService
  {
    private readonly ILogger _logger;
    private readonly HttpMockOptions _mockOptions;
    private readonly IMockRepository _mockRepository;

    public HttpMockService(ILogger<HttpMockService> logger, IOptionsSnapshot<HttpMockOptions> mockOptions, IMockRepository mockRepository)
    {
      _logger = logger;
      _mockOptions = mockOptions.Value;
      _mockRepository = mockRepository;
    }

    public string CreateKey(HttpMockRequest mockRequest)
    {
      var requestKey = mockRequest.GetRequestKey().ToBase64String();
      _logger.LogInformation($"Created RequestKey: {requestKey}");
      return requestKey;
    }

    public async Task<MockInstance> CreateMockAsync(string requestKey, string contentType, string bodyContent)
    {
      if (requestKey == null) throw new ArgumentException("RequestKey is required");
      return await _mockRepository.SaveAsync(requestKey.ToBase64String(), new MockInstance(requestKey, contentType, bodyContent));
    }

    public async Task DeleteMockAsync(string requestKey)
    {
      if (requestKey == null) throw new ArgumentException("RequestKey is required");
      await _mockRepository.DeleteAsync(requestKey);
      _logger.LogInformation($"Deleted Mock with RequestKey: {requestKey}");
    }

    public async Task<MockInstance> GetMockAsync(HttpMockRequest mockRequest)
    {
      var requestKey = mockRequest.GetRequestKey().ToBase64String();
      _logger.LogInformation($"Searching Mock with RequestKey: {requestKey}");
      return await _mockRepository.GetAsync(requestKey);
    }

  }
}
