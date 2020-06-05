namespace AspNetCore.Middleware.HttpMock.Core
{
  public static class RequestHeaderActions
  {
    public static string CreateKeyHeader(this HttpMockOptions mockOptions) => $"{mockOptions.RequestHeaderPrefix}-createkey";
    public static string CreateMockHeader(this HttpMockOptions mockOptions) => $"{mockOptions.RequestHeaderPrefix}-createmock";
    public static string DeleteMockHeader(this HttpMockOptions mockOptions) => $"{mockOptions.RequestHeaderPrefix}-deletemock";
  }
}
