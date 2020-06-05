namespace AspNetCore.Middleware.HttpMock.Core.DomainModels
{
  public class MockInstance
  {
    public MockInstance() { }

    public MockInstance(string requestKey, string contentType, string bodyContent)
    {
      Request = new HttpMockRequest(requestKey);
      Response = new HttpMockResponse(contentType, bodyContent);
    }

    public HttpMockRequest Request { get; set; }
    public HttpMockResponse Response { get; set; }

  }
}
