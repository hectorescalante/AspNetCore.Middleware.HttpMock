using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Middleware.HttpMock.Core.DomainModels
{
  public class HttpMockResponse
  {
    public HttpMockResponse() { }

    public HttpMockResponse(string contentType, string content)
    {
      ContentType = contentType;
      BodyContent = content;
    }

    public string ContentType { get; set; }
    public string BodyContent { get; set; }

  }
}
