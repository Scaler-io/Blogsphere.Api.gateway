namespace Blogsphere.Api.Gateway.Extensions;

public static class HttpRequestExtensions
{
    public static string GetRequestHeaderOrdefault(this HttpRequest request, string key, string defaultValue = null)
    {
        var headers = request?.Headers?.FirstOrDefault(h => h.Key.Equals(key)).Value.FirstOrDefault();
        return headers ?? defaultValue;
    }
}
