namespace AspNetCore.Middleware.HttpMock.Core
{
  public static class RequestHeaderActions
  {
    public static string ActionHeader(this HttpMockOptions mockOptions) => $"{mockOptions.RequestHeaderPrefix}-action";
    public static string CreateKey = "createkey";
    public static string CreateMock = "createmock";
    public static string DeleteMock = "deletemock";
  }
}
