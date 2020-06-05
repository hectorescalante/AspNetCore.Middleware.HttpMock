using AspNetCore.Middleware.HttpMock.Core;
using AspNetCore.Middleware.HttpMock.Core.Abstractions;
using AspNetCore.Middleware.HttpMock.Core.DomainModels;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AspNetCore.Middleware.HttpMock.Infrastructure
{
  internal class HttpMockRepository : IMockRepository
  {
    private readonly ILogger _logger;
    private readonly IMockStorage _mockStorage;

    public HttpMockRepository(ILogger<HttpMockRepository> logger, IMockStorage mockStorage)
    {
      _logger = logger;
      _mockStorage = mockStorage;
    }

    public async Task<MockInstance> GetAsync(string key)
    {
      _logger.LogInformation($"Retrieving Mock with RequestKey: {key}");
      return await _mockStorage.GetAsync<MockInstance>(key);
    }

    public async Task<MockInstance> SaveAsync(string key, MockInstance mockInstance)
    {
      var mockBodyContent = mockInstance.Response.BodyContent;
      mockInstance.Response.BodyContent = mockBodyContent.ToBase64String();
      _logger.LogInformation($"Created Mock with RequestKey: {key}");
      return await _mockStorage.SaveAsync(key, mockInstance);
    }

    public async Task DeleteAsync(string key)
    {
      _logger.LogInformation($"Deleting Mock with RequestKey: {key}");
      await _mockStorage.DeleteAsync(key);
    }

  }
}
