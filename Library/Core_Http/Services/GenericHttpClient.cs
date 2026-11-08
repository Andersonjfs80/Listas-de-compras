using System.Net.Http.Json;
using Core_Http.Interfaces;
using Core_Logs.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Core_Logs.Security.Models;
using System.Text;
using System.Text.Json;
using Jose;
using Core_Http.Configuration;

namespace Core_Http.Services;

public class GenericHttpClient(
    IHttpProxyService proxyService, 
    IHttpContextAccessor contextAccessor,
    IHttpLogService logService,
    IOptions<SecuritySettings> settings,
    CoreHttpMode mode,
    Microsoft.AspNetCore.Hosting.IWebHostEnvironment webHostEnvironment) : IGenericHttpClient
{

    private readonly byte[] _encryptionKey = Encoding.UTF8.GetBytes((string.IsNullOrEmpty(settings.Value.SecretKey) ? "SecretKey_Deve_Vir_Do_Appsettings_Com_Pelo_Menos_32_Chars" : settings.Value.SecretKey).PadRight(32).Substring(0, 32));

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
        var requestContent = body != null ? System.Text.Json.JsonSerializer.Serialize(body) : "No Body";
        
        HttpResponseMessage response;
        try
        {
            if (generateLog)
            {                
                logService.LogChamadaInterna(fullUrl, requestContent, "", 0, 0);
            }

            response = await proxyService.SendAsync(baseUrl, path, method, body, headers, ct);

            watch.Stop();

            // Sempre carrega o buffer para permitir múltiplas leituras (Log + Descriptografia)
            await response.Content.LoadIntoBufferAsync();
            var responseContent = await response.Content.ReadAsStringAsync(ct);

            if (generateLog)
            {
                logService.LogChamadaInterna(fullUrl, requestContent, responseContent, (int)response.StatusCode, watch.ElapsedMilliseconds);
            }

            // --- REMOVER CRIPTOGRAFIA (Apenas se for BFF Gateway e a criptografia estiver ATIVA) ---
            var isNonProd = webHostEnvironment.EnvironmentName.Equals("DEV", StringComparison.OrdinalIgnoreCase) || 
                            webHostEnvironment.EnvironmentName.Equals("HML", StringComparison.OrdinalIgnoreCase) ||
                            webHostEnvironment.EnvironmentName.Equals("Development", StringComparison.OrdinalIgnoreCase) ||
                            webHostEnvironment.EnvironmentName.Equals("Homologation", StringComparison.OrdinalIgnoreCase);

            var isEncryptionEnabled = isNonProd ? settings.Value.EnableBodyEncryption : true;

            if (mode.IsBffGateway && isEncryptionEnabled && response.IsSuccessStatusCode && response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                if (!string.IsNullOrEmpty(responseContent) && responseContent.TrimStart().StartsWith("{\"data\":", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        using var jsonDoc = JsonDocument.Parse(responseContent);
                        if (jsonDoc.RootElement.TryGetProperty("data", out var dataProp))
                        {
                            var jweToken = dataProp.GetString();
                            if (!string.IsNullOrEmpty(jweToken))
                            {
                                logService.LogTraceCriptografia("BFF_CLIENT", "ANTES_DESCRIPTOGRAFAR", jweToken);
                                
                                var decryptedBody = Jose.JWT.Decode(jweToken, _encryptionKey);
                                
                                logService.LogTraceCriptografia("BFF_CLIENT", "DEPOIS_DESCRIPTOGRAFAR", decryptedBody);
                                
                                response.Content = new StringContent(decryptedBody, Encoding.UTF8, "application/json");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logService.LogErroInterno(fullUrl, new Exception("BFF: Erro ao remover criptografia do backend para manipulação", ex));
                    }
                }
            }
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

        // PROPAGAÇÃO DE HEADERS DE SEGURANÇA
        var context = contextAccessor.HttpContext;


        // Se o tipo esperado for IResult (Cenário de Proxy/Gateway)
        if (typeof(TResponse) == typeof(IResult))
        {
            return (TResponse?)(object)await MapToProxyResult(response, ct);
        }

        // Cenário Tipado (Microserviços ou Gateway com Transformação)
        if (!response.IsSuccessStatusCode)
        {
            if (context != null) context.Response.StatusCode = (int)response.StatusCode;
            
            try {
                if (typeof(TResponse) == typeof(string))
                {
                    var content = await response.Content.ReadAsStringAsync(ct);
                    return (TResponse?)(object)content;
                }
                if (typeof(TResponse) == typeof(byte[]))
                {
                    var content = await response.Content.ReadAsByteArrayAsync(ct);
                    return (TResponse?)(object)content;
                }
                return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct);
            } catch {
                return default;
            }
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return default;

        if (typeof(TResponse) == typeof(string))
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            return (TResponse?)(object)content;
        }
        if (typeof(TResponse) == typeof(byte[]))
        {
            var content = await response.Content.ReadAsByteArrayAsync(ct);
            return (TResponse?)(object)content;
        }

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
