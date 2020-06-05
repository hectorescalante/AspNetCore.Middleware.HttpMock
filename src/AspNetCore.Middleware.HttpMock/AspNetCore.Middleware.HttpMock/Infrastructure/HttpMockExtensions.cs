using AspNetCore.Middleware.HttpMock.Core;
using AspNetCore.Middleware.HttpMock.Core.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Middleware.HttpMock.Infrastructure
{
  public static class HttpMockExtensions
  {
    public static IApplicationBuilder UseHttpMockMiddleware(this IApplicationBuilder applicationBuilder)
    {
      applicationBuilder.UseMiddleware<HttpMockMiddleware>();
      return applicationBuilder;
    }

    public static IServiceCollection AddHttpMockMiddleware(this IServiceCollection serviceCollection, IConfiguration configuration, IMockStorage customStorage = null)
    {
      serviceCollection.Configure<HttpMockOptions>(configuration);
      serviceCollection.AddScoped<IMockStorage, CacheStorage>();
      if (customStorage != null)  
        serviceCollection.AddScoped(storage => customStorage);
      serviceCollection.AddScoped<IMockRepository, HttpMockRepository>();
      serviceCollection.AddScoped<IMockService, HttpMockService>();
      return serviceCollection;
    }
  }
}
