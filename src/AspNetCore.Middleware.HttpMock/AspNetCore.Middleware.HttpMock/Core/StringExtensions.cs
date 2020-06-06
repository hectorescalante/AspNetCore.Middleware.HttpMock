using System;
using System.Text;
using System.Text.Json;

namespace AspNetCore.Middleware.HttpMock.Core
{
  public static class StringExtensions
  {
    private readonly static JsonSerializerOptions _jsonSerializerOptions =
      new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, IgnoreNullValues = true };

    public static string ToJsonString<T>(this T entity) =>
      JsonSerializer.Serialize(entity, _jsonSerializerOptions);

    public static T FromJsonString<T>(this string jsonString)
    {
      try
      {
        return JsonSerializer.Deserialize<T>(jsonString, _jsonSerializerOptions);
      }
      catch (Exception)
      {
        return default;
      }
    }

    public static string FromBase64String(this string base64String)
    {
      try
      {
      return Encoding.UTF8.GetString(Convert.FromBase64String(base64String ?? ""));
      }
      catch (Exception)
      {
        return default;
      }
    }

    public static string ToBase64String(this string plainText) =>
      Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText ?? ""));

    public static string Minify(this string text) =>
      text.Replace("\n", "").Replace("\r", "").Replace(" ", "").ToLower();

  }
}
