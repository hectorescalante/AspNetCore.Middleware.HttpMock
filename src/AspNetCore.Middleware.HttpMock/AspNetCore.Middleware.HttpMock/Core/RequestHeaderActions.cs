using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Middleware.HttpMock.Core
{
  public static class RequestHeaderActions
  {
    public static bool CreateKey(this IHeaderDictionary headers, string prefix) => headers.ContainsKey($"{prefix}-createkey");
    public static bool CreateMock(this IHeaderDictionary headers, string prefix) => headers.ContainsKey($"{prefix}-createmock");
    public static bool DeleteMock(this IHeaderDictionary headers, string prefix) => headers.ContainsKey($"{prefix}-deletemock");
  }
}
