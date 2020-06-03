using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Middleware.HttpMock.Core
{
  class RequestHeaderValues
  {
    public static string RequestKey(string prefix) => $"{prefix}-requestkey";
    public static string Host(string prefix) => $"{prefix}-host";
  }
}
