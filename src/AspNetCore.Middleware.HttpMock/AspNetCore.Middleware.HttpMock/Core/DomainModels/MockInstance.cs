namespace AspNetCore.Middleware.HttpMock.Core.DomainModels
{
  public class MockInstance
  {
    public MockInstance() { }

    public MockInstance(string requestKey, string contentType, string bodyContent)
    {
      Request = new MockRequest(requestKey);
      Response = new MockResponse(contentType, bodyContent);
    }

    public MockRequest Request { get; set; }
    public MockResponse Response { get; set; }

  }
}
