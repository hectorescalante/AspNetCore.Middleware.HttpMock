using AspNetCore.Middleware.HttpMock.Core;
using AspNetCore.Middleware.HttpMock.Core.Abstractions;
using AspNetCore.Middleware.HttpMock.Core.DomainModels;
using AspNetCore.Middleware.HttpMock.Infrastructure;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
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
      var httpContext = _autoFixture.Create<DefaultHttpContext>();
      httpContext.Request.Headers.Add(mockOptions.Object.Value.ActionHeader(), new StringValues(RequestHeaderActions.CreateKey));
      var mockService = _autoFixture.Freeze<Mock<IMockService>>();
      mockService
        .Setup(mock => mock.CreateKey(It.IsAny<HttpMockRequest>()))
        .Returns("Ok");

      //Act
      var sut = _autoFixture.Create<HttpMockMiddleware>();
      await sut.InvokeAsync(httpContext, mockOptions.Object, mockService.Object);

      //Assert
      Assert.Equal((int)HttpStatusCode.OK, httpContext.Response.StatusCode);
      mockService.Verify(mock => mock.CreateKey(It.IsAny<HttpMockRequest>()), Times.Once);
    }

    [Fact]
    public async Task TestCreateMock_WithNoBody_ShouldSuccess()
    {
      //Arrange
      var mockOptions = _autoFixture.Freeze<Mock<IOptionsSnapshot<HttpMockOptions>>>();
      var httpContext = _autoFixture.Create<DefaultHttpContext>();
      httpContext.Request.Headers.Add(mockOptions.Object.Value.ActionHeader(), new StringValues(RequestHeaderActions.CreateMock));
      httpContext.Request.Headers.Add(mockOptions.Object.Value.RequestKeyHeader(), new StringValues("RequestKey".ToBase64String()));
      var mockService = _autoFixture.Freeze<Mock<IMockService>>();
      mockService
        .Setup(mock => mock.CreateMockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(_autoFixture.Create<MockInstance>());

      //Act
      var sut = _autoFixture.Create<HttpMockMiddleware>();
      await sut.InvokeAsync(httpContext, mockOptions.Object, mockService.Object);

      //Assert
      Assert.Equal((int)HttpStatusCode.Created, httpContext.Response.StatusCode);
      mockService.Verify(mock => mock.CreateMockAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task TestDeleteMock_WithNoBody_ShouldSuccess()
    {
      //Arrange
      var mockOptions = _autoFixture.Freeze<Mock<IOptionsSnapshot<HttpMockOptions>>>();
      var httpContext = _autoFixture.Create<DefaultHttpContext>();
      httpContext.Request.Headers.Add(mockOptions.Object.Value.ActionHeader(), new StringValues(RequestHeaderActions.DeleteMock));
      var mockService = _autoFixture.Freeze<Mock<IMockService>>();

      //Act
      var sut = _autoFixture.Create<HttpMockMiddleware>();
      await sut.InvokeAsync(httpContext, mockOptions.Object, mockService.Object);

      //Assert
      Assert.Equal((int)HttpStatusCode.OK, httpContext.Response.StatusCode);
      mockService.Verify(mock => mock.DeleteMockAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task TestGetMock_WithNoBody_ShouldSuccess()
    {
      //Arrange
      var mockOptions = _autoFixture.Freeze<Mock<IOptionsSnapshot<HttpMockOptions>>>();
      var httpContext = _autoFixture.Create<DefaultHttpContext>();
      var mockService = _autoFixture.Freeze<Mock<IMockService>>();
      mockService
        .Setup(mock => mock.GetMockAsync(It.IsAny<HttpMockRequest>()))
        .ReturnsAsync(_autoFixture.Create<MockInstance>());

      //Act
      var sut = _autoFixture.Create<HttpMockMiddleware>();
      await sut.InvokeAsync(httpContext, mockOptions.Object, mockService.Object);

      //Assert
      Assert.Equal((int)HttpStatusCode.OK, httpContext.Response.StatusCode);
      mockService.Verify(mock => mock.GetMockAsync(It.IsAny<HttpMockRequest>()), Times.Once);
    }

  }
}
