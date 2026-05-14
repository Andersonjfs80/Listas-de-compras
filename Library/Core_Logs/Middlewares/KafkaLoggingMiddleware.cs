using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Core_Logs.Configuration;
using Core_Logs.Interfaces;
using System.Diagnostics;
using Core_Logs.Log;
using Core_Logs.Security.Models;
using Core_Logs.Security.Interfaces;
using System.Text.Json;
using System.Text;

namespace Core_Logs.Middlewares;

public class KafkaLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly KafkaSettings _settings;
    private readonly ILogQueue _queue;

    public KafkaLoggingMiddleware(RequestDelegate next, IOptions<KafkaSettings> options, ILogQueue queue)
    {
        _next = next;
        _settings = options.Value;
        _queue = queue;
    }

    public async Task InvokeAsync(HttpContext context, ILogCustom logCustom, ISecurityService securityService, IOptions<SecuritySettings> securitySettings, IUserContext userContext)
    {
        Console.WriteLine($"[DEBUG-LOG] Iniciando processamento de log para: {context.Request.Path}");
        if (!_settings.Enabled)
        {
            await _next(context);
            return;
        }

        var watch = Stopwatch.StartNew();
        
        // 1. GERAR/CAPTURAR TRACE ID
        var traceId = context.Request.Headers["X-Trace-Id"].FirstOrDefault() 
                      ?? context.TraceIdentifier 
                      ?? Guid.NewGuid().ToString("n").Substring(0, 16);
        
        var userId = userContext.UserId;
        
        if (!context.Response.Headers.ContainsKey("X-Trace-Id"))
            context.Response.Headers.Add("X-Trace-Id", traceId);

        // PROPAGAR SESSAO E MESSAGE ID PARA O RESPONSE (para aparecer no log de resposta)
        PropagarHeadersParaResponse(context);

        var requestBodyText = await ObterRequestBody(context.Request, securityService, securitySettings.Value);
        
        // 1. GERAR LOG DE REQUEST (ENTRADA)
        PreencherDadosRequisicao(logCustom.Log, context, traceId, userId, 0, requestBodyText, "request");
        logCustom.Log.RequestHeaders = LimparHeaders(context.Request.Headers);
        await _queue.EnqueueAsync(logCustom.Log);

        // 2. INTERCEPTAR RESPOSTA
        var (originalBodyStream, responseBody) = InterceptarResposta(context.Response);

        try
        {
            await _next(context);

            watch.Stop();
            
            // 3. GERAR LOG DE RESPONSE (SAÍDA)
            // Criamos um novo objeto para a resposta para não sobrescrever o da request que pode estar na fila
            var logRes = new Models.LogCustomModel();
            PreencherDadosRequisicao(logRes, context, traceId, userId, watch.ElapsedMilliseconds, requestBodyText, "response");
            
            logRes.RequestHeaders = logCustom.Log.RequestHeaders;
            logRes.ResponseHeaders = LimparHeaders(context.Response.Headers);
            logRes.StatusCode = context.Response.StatusCode;

            var responseContentText = await ObterResponseBody(context.Response, securityService, securitySettings.Value);
            logRes.Response = ConvertToLogObject(responseContentText);

            await _queue.EnqueueAsync(logRes);

            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            watch.Stop();
            
            // 4. Registrar ERRO em log de Response
            var logErr = new Models.LogCustomModel();
            PreencherDadosRequisicao(logErr, context, traceId, userId, watch.ElapsedMilliseconds, requestBodyText, "error");
            
            logErr.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            logErr.StackTrace = ex.StackTrace;
            logErr.Logs.Add($"[EXCEPTION] {ex.Message}");
            
            await _queue.EnqueueAsync(logErr);
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
            await responseBody.DisposeAsync();
        }
    }

    private (Stream originalStream, MemoryStream tempStream) InterceptarResposta(HttpResponse response)
    {
        var originalBodyStream = response.Body;
        var responseBody = new MemoryStream();
        response.Body = responseBody;
        return (originalBodyStream, responseBody);
    }

    private async Task<string?> ObterRequestBody(HttpRequest request, ISecurityService securityService, SecuritySettings securitySettings)
    {
        request.EnableBuffering();
        var body = await ReadStreamAsync(request.Body);
        request.Body.Position = 0;

        // Se o body estiver criptografado no envelope { data: "..." }, descriptografamos apenas para o LOG
        body = TentarDescriptografar(body, securityService, securitySettings);
        
        return JsonSanitizer.Sanitize(body, _settings.OfuscarCampos.Request);
    }

    private async Task<string?> ObterResponseBody(HttpResponse response, ISecurityService securityService, SecuritySettings securitySettings)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        // Se a resposta estiver criptografada no envelope { data: "..." }, descriptografamos apenas para o LOG
        body = TentarDescriptografar(body, securityService, securitySettings);
        
        return JsonSanitizer.Sanitize(body, _settings.OfuscarCampos.Response);
    }

    private string? TentarDescriptografar(string? body, ISecurityService securityService, SecuritySettings securitySettings)
    {
        if (string.IsNullOrWhiteSpace(body) || !body.TrimStart().StartsWith("{")) return body;

        try
        {
            using var jsonDoc = JsonDocument.Parse(body);
            if (jsonDoc.RootElement.TryGetProperty("data", out var dataProp))
            {
                var jweToken = dataProp.GetString();
                if (!string.IsNullOrEmpty(jweToken) && jweToken.Contains("."))
                {
                    // Chave simétrica conforme padrão do middleware
                    var secretKey = string.IsNullOrEmpty(securitySettings.SecretKey) ? "SecretKey_Deve_Vir_Do_Appsettings_Com_Pelo_Menos_32_Chars" : securitySettings.SecretKey;
                    var encryptionKey = Encoding.ASCII.GetBytes(secretKey.PadRight(32).Substring(0, 32));

                    return Jose.JWT.Decode(jweToken, encryptionKey);
                }
            }
        }
        catch { /* Se falhar, mantém o body original para o log */ }

        return body;
    }

    private void PropagarHeadersParaResponse(HttpContext context)
    {
        string[] headersParaPropagar = ["SESSAO-ID", "MESSAGE-ID", "MESSAGE-ID-MODULO", "HARDWARE-ID"];
        foreach (var header in headersParaPropagar)
        {
            if (context.Request.Headers.TryGetValue(header, out var value) && !context.Response.Headers.ContainsKey(header))
            {
                context.Response.Headers.Add(header, value);
            }
        }
    }

    private void PreencherDadosRequisicao(Models.LogCustomModel log, HttpContext context, string traceId, string? userId, long durationMs, string? bodyText, string tipo)
    {
        log.Tipo = tipo;
        log.TraceId = traceId;
        log.UserId = userId;
        log.Method = context.Request.Method ?? "UNKNOWN";
        log.Scheme = context.Request.Scheme;
        log.Host = context.Request.Host.Host;
        log.Port = context.Request.Host.Port;
        log.Url = context.Request.GetDisplayUrl();
        log.FullUrl = context.Request.GetDisplayUrl();
        log.Path = context.Request.Path.Value ?? "/";
        log.RelativePath = context.Request.PathBase + context.Request.Path;
        log.Query = context.Request.QueryString.ToString();
        log.DurationMs = durationMs;
        log.Timestamp = DateTime.UtcNow;
        log.Body = ConvertToLogObject(bodyText);
    }

    private Dictionary<string, string> LimparHeaders(IHeaderDictionary headers)
    {
        var cleanHeaders = new Dictionary<string, string>();
        foreach (var header in headers)
        {
            // Pega valores distintos para evitar o problema de duplicação (ex: "ID, ID")
            var values = header.Value.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct();
            cleanHeaders.Add(header.Key, string.Join(", ", values));
        }
        return cleanHeaders;
    }

    private object? ConvertToLogObject(string? content)
    {
        if (string.IsNullOrWhiteSpace(content)) return null;

        try
        {
            // Tenta fazer o parse para ver se é um JSON válido (evita double-escaping)
            return JsonSerializer.Deserialize<JsonElement>(content);
        }
        catch
        {
            // Se não for um JSON válido, retorna a string original
            return content;
        }
    }

    private async Task<string> ReadStreamAsync(Stream stream)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        var content = await reader.ReadToEndAsync();
        return content;
    }
}
