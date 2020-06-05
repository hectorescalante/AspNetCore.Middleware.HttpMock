using AspNetCore.Middleware.HttpMock.Core;
using AspNetCore.Middleware.HttpMock.Core.Abstractions;
using AspNetCore.Middleware.HttpMock.Core.DomainModels;
using AspNetCore.Middleware.HttpMock.Infrastructure;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace HttpMock.Tests
{
  public class HttpMockMiddlewareUnitTest
  {
    private readonly IFixture _autoFixture;
    public HttpMockMiddlewareUnitTest()
    {
      _autoFixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public async Task TestCreateKey_WithNoBody_ShouldSuccess()
    {
      //Arrange
      var mockOptions = _autoFixture.Freeze<Mock<IOptionsSnapshot<HttpMockOptions>>>();
      var httpContext = _autoFixture.Freeze<HttpContext>();
      var mockHttpContextManager = _autoFixture.Freeze<Mock<IHttpContextManager>>();
      mockHttpContextManager
        .Setup(mock => mock.ContainsHeader(It.Is<string>(v => v == mockOptions.Object.Value.CreateKeyHeader())))
        .Returns(true);
      var mockService = _autoFixture.Freeze<Mock<IMockService>>();
      mockService
        .Setup(mock => mock.CreateKey(It.IsAny<HttpMockRequest>()))
        .Returns("Ok");

      //Act
      var sut = _autoFixture.Create<HttpMockMiddleware>();
      await sut.InvokeAsync(httpContext, mockHttpContextManager.Object, mockOptions.Object, mockService.Object);

      //Assert
      mockService.Verify(mock => mock.CreateKey(It.IsAny<HttpMockRequest>()), Times.Once);
      mockHttpContextManager.Verify(mock => mock.WriteResponseAsync(It.IsAny<HttpStatusCode>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task TestCreateMock_WithNoBody_ShouldSuccess()
    {
      //Arrange
      var mockOptions = _autoFixture.Freeze<Mock<IOptionsSnapshot<HttpMockOptions>>>();
      var httpContext = _autoFixture.Freeze<HttpContext>();
      var mockHttpContextManager = _autoFixture.Freeze<Mock<IHttpContextManager>>();
      mockHttpContextManager
        .Setup(mock => mock.ContainsHeader(It.Is<string>(v => v == mockOptions.Object.Value.CreateMockHeader())))
        .Returns(true);
      mockHttpContextManager
        .Setup(mock => mock.GetHeaderValue(It.IsAny<string>()))
        .Returns("RequestKey".ToBase64String());
      var mockService = _autoFixture.Freeze<Mock<IMockService>>();
      mockService
        .Setup(mock => mock.CreateMockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(_autoFixture.Create<MockInstance>());

      //Act
      var sut = _autoFixture.Create<HttpMockMiddleware>();
      await sut.InvokeAsync(httpContext, mockHttpContextManager.Object, mockOptions.Object, mockService.Object);

      //Assert
      mockService.Verify(mock => mock.CreateMockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
      mockHttpContextManager.Verify(mock => mock.WriteResponseAsync(It.IsAny<HttpStatusCode>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

  }
}
