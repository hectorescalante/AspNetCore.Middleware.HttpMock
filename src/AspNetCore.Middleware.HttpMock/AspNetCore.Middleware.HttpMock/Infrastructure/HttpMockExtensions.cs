using AspNetCore.Middleware.HttpMock.Core;
using AspNetCore.Middleware.HttpMock.Core.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore.Middleware.HttpMock.Infrastructure
{
  public static class HttpMockExtensions
  {
    public static IServiceCollection AddHttpMockMiddleware(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
      serviceCollection.AddMemoryCache();
      return serviceCollection.AddHttpMockMiddleware(configuration, typeof(CacheStorage));
    }

    public static IServiceCollection AddHttpMockMiddleware(this IServiceCollection serviceCollection, IConfiguration configuration, Type customStorageImplementation)
    {
      serviceCollection.Configure<HttpMockOptions>(configuration);
      serviceCollection.AddScoped<IMockRepository, HttpMockRepository>();
      serviceCollection.AddScoped<IMockService, HttpMockService>();
      serviceCollection.AddScoped(typeof(IMockStorage), customStorageImplementation);

      return serviceCollection;
    }

    public static IApplicationBuilder UseHttpMockMiddleware(this IApplicationBuilder applicationBuilder)
    {
      applicationBuilder.UseMiddleware<HttpMockMiddleware>();
      return applicationBuilder;
    }
  }
}
