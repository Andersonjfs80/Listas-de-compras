using System.Net.Http.Headers;
using System.Net.Http.Json;
using Core_Http.Interfaces;
using Core_Http.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;

namespace Core_Http.Services;

public class HttpProxyService : IHttpProxyService
{
    private readonly HttpClient _client;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ResiliencePipeline<HttpResponseMessage> _pipeline;

    public HttpProxyService(
        IHttpClientFactory factory, 
        IHttpContextAccessor contextAccessor,
        IOptions<HttpConfig> options) // Note: user requested "HttpConfig" but then "mudar o nome tira policy" and I renamed it to HttpConfig. Wait, let me check the file name.
    {
        _contextAccessor = contextAccessor;
        
        // REUSO: Cria o cliente uma única vez para este serviço Singleton
        _client = factory.CreateClient("Core_Http_Proxy");

        var config = options.Value;
        
        if (config == null) throw new ArgumentNullException(nameof(options), "Configurações HTTP não encontradas.");

        // PIPELINE COMPARTILHADO: Essencial para o Circuit Breaker funcionar entre requisições
        _pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = config.RetryCount,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<Exception>()
                    .HandleResult(r => (int)r.StatusCode >= 500)
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                FailureRatio = 0.5,
                MinimumThroughput = config.FailureThreshold,
                BreakDuration = TimeSpan.FromSeconds(config.DurationOfBreakSeconds),
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<Exception>()
                    .HandleResult(r => (int)r.StatusCode >= 500)
            })
            .Build();
    }

    public async Task<HttpResponseMessage> SendAsync(
        string baseUrl, 
        string targetPath, 
        HttpMethod method, 
        object? body = null, 
        IDictionary<string, string>? headers = null, 
        CancellationToken ct = default)
    {
        var context = _contextAccessor.HttpContext;

        // Constrói URI absoluta para ser thread-safe (sem BaseAddress)
        var baseUri = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/");
        var fullUri = new Uri(baseUri, targetPath);

        return await _pipeline.ExecuteAsync(async token =>
        {
            var pathWithQuery = fullUri.ToString();
            if (context != null && context.Request.QueryString.HasValue)
            {
                pathWithQuery += context.Request.QueryString.Value;
            }

            var request = new HttpRequestMessage(method, pathWithQuery);

            // PROPAGAÇÃO DE HEADERS PADRONIZADOS: Captura do contexto inbound e replica no outbound
            if (context != null)
            {
                foreach (var headerName in Core_Logs.Constants.StandardHeaderNames.MandatoryHeaders)
                {
                    if (context.Request.Headers.TryGetValue(headerName, out var headerValue))
                    {
                        request.Headers.TryAddWithoutValidation(headerName, headerValue.ToArray());
                    }
                }
            }

            // Copia headers manuais (se houver)
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Adiciona body
            if (body != null)
            {
                request.Content = JsonContent.Create(body);
            }

            return await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
        }, ct);
    }
}
