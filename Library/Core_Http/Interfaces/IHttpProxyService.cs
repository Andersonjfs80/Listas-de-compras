namespace Core_Http.Interfaces;

public interface IHttpProxyService
{
    Task<HttpResponseMessage> SendAsync(
        string baseUrl, 
        string targetPath, 
        HttpMethod method, 
        object? body = null, 
        IDictionary<string, string>? headers = null, 
        CancellationToken ct = default);
}
