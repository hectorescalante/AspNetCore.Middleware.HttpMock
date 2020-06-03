using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Middleware.HttpMock.Core.DomainModels
{
  public class MockResponse
  {
    public MockResponse() { }

    public MockResponse(string contentType, string content)
    {
      ContentType = contentType;
      BodyContent = content;
    }

    public string ContentType { get; set; }
    public string BodyContent { get; set; }

  }
}
