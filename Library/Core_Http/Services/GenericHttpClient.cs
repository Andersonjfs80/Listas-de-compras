using System.Net.Http.Json;
using Core_Http.Interfaces;
using Core_Logs.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Core_Http.Services;

public class GenericHttpClient(
    IHttpProxyService proxyService, 
    IHttpContextAccessor contextAccessor,
    IHttpLogService logService) : IGenericHttpClient
{
    public Task<TResponse?> GetAsync<TResponse>(string baseUrl, string path, IDictionary<string, string>? headers = null, bool generateLog = true, CancellationToken ct = default)
    {
        return SendRequest<object, TResponse>(HttpMethod.Get, baseUrl, path, null, headers, generateLog, ct);
    }

    public Task<TResponse?> PostAsync<TRequest, TResponse>(string baseUrl, string path, TRequest body, IDictionary<string, string>? headers = null, bool generateLog = true, CancellationToken ct = default)
    {
        return SendRequest<TRequest, TResponse>(HttpMethod.Post, baseUrl, path, body, headers, generateLog, ct);
    }

    public Task<TResponse?> PutAsync<TRequest, TResponse>(string baseUrl, string path, TRequest body, IDictionary<string, string>? headers = null, bool generateLog = true, CancellationToken ct = default)
    {
        return SendRequest<TRequest, TResponse>(HttpMethod.Put, baseUrl, path, body, headers, generateLog, ct);
    }

    public Task<TResponse?> DeleteAsync<TResponse>(string baseUrl, string path, IDictionary<string, string>? headers = null, bool generateLog = true, CancellationToken ct = default)
    {
        return SendRequest<object, TResponse>(HttpMethod.Delete, baseUrl, path, null, headers, generateLog, ct);
    }

    private async Task<TResponse?> SendRequest<TRequest, TResponse>(HttpMethod method, string baseUrl, string path, TRequest? body, IDictionary<string, string>? headers, bool generateLog, CancellationToken ct)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var fullUrl = $"{baseUrl}{path}";
        
        HttpResponseMessage response;
        try
        {
            response = await proxyService.SendAsync(baseUrl, path, method, body, headers, ct);
        }
        catch (Exception ex)
        {
            watch.Stop();
            if (generateLog)
            {
                logService.LogErroInterno(fullUrl, ex);
            }
            throw;
        }

        watch.Stop();

        if (generateLog)
        {
            var reqContent = body != null ? System.Text.Json.JsonSerializer.Serialize(body) : null;
            var responseContent = response.IsSuccessStatusCode 
                ? "Success (Body omitted for performance/proxy)" 
                : await response.Content.ReadAsStringAsync(ct);

            logService.LogChamadaInterna(fullUrl, reqContent, responseContent, (int)response.StatusCode, watch.ElapsedMilliseconds);
        }

        // Se o tipo esperado for IResult (Cenário de Proxy/Gateway)
        if (typeof(TResponse) == typeof(IResult))
        {
            return (TResponse?)(object)await MapToProxyResult(response, ct);
        }

        // Cenário Tipado (Microserviços)
        response.EnsureSuccessStatusCode();
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return default;
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct);
    }

    private async Task<IResult> MapToProxyResult(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent || 
           (response.Content.Headers.ContentLength.HasValue && response.Content.Headers.ContentLength == 0))
        {
            return Results.StatusCode((int)response.StatusCode);
        }

        var stream = await response.Content.ReadAsStreamAsync(ct);
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";

        var context = contextAccessor.HttpContext;
        if (context != null) context.Response.StatusCode = (int)response.StatusCode;

        return Results.Stream(stream, contentType);
    }
}
