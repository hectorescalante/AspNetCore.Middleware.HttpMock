namespace AspNetCore.Middleware.HttpMock.Core
{
  public static class RequestHeaderValues
  {
    public static string RequestKeyHeader(this HttpMockOptions mockOptions) => $"{mockOptions.RequestHeaderPrefix}-requestkey";
    public static string HostHeader(this HttpMockOptions mockOptions) => $"{mockOptions.RequestHeaderPrefix}-host";
    public static string ContentTypeHeader() => "content-type";

  }
}
