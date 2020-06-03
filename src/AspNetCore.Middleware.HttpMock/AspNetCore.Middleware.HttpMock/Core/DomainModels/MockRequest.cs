using AspNetCore.Middleware.HttpMock.Core;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace AspNetCore.Middleware.HttpMock.Core.DomainModels
{
  public class MockRequest
  {
    public MockRequest() { }

    public MockRequest(string requestKey)
    {
      var request = requestKey.Split('|');

      IsHttps = request[0].Contains("https");
      Method = request[1];
      Host = request[2];
      if (request.Length > 3)
        Path = request[3];
      if (request.Length > 4)
        QueryString = request[4];
      if (request.Length > 5)
        BodyContent = request[5];
    }

    public ICollection<KeyValuePair<string, StringValues>> Headers { get; set; }
    public bool IsHttps { get; set; }
    public string Method { get; set; }
    public string Host { get; set; }
    public string Path { get; set; }
    public string QueryString { get; set; }
    public string ContentType { get; set; }
    public string BodyContent { get; set; }

    public string Http
    {
      get
      {
        return IsHttps ? "https://" : "http://";
      }
    }
    public string GetRequestKey() => $"{Http}|{Method}|{Host}|{Path}|{QueryString}|{BodyContent}".Minify();
    public string GetUri() => $"{Http}{Host}{Path}{QueryString}";
    public string GetPathWithQueryString() => $"{Path}{QueryString}";

  }
}
