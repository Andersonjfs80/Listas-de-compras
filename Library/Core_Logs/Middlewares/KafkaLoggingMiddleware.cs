using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Core_Logs.Configuration;
using Core_Logs.Interfaces;
using System.Diagnostics;
using Core_Logs.Log;

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

    public async Task InvokeAsync(HttpContext context, LogCustom logCustom)
    {
        if (!_settings.Enabled)
        {
            await _next(context);
            return;
        }

        var watch = Stopwatch.StartNew();
        var log = logCustom.Log; 

        // 1. Setup inicial
        log.Method = context.Request.Method;
        log.Url = context.Request.GetDisplayUrl();
        log.Headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

        // Captura Request Body
        var requestBody = await ObterRequestBody(context.Request);
        log.Body = requestBody; 

        // 2. Interceptar Resposta
        var (originalBodyStream, responseBody) = InterceptarResposta(context.Response);

        try
        {
            await _next(context);

            watch.Stop();
            
            // 3. Registrar SAÍDA
            log.StatusCode = context.Response.StatusCode;
            
            // Opcional: Se quiser registrar o Response Body, teria que ser na lista de Logs ou mudar o CustomLogCore
            var responseContent = await ObterResponseBody(context.Response);
            if (!string.IsNullOrEmpty(responseContent))
            {
               log.Logs.Add($"[RESPONSE_BODY] {responseContent}");
            }

            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            watch.Stop();
            // 4. Registrar ERRO
            log.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            log.Logs.Add($"[EXCEPTION] {ex.Message} | StackTrace: {ex.StackTrace}");
            
            throw;
        }
        finally
        {
            log.DurationMs = watch.ElapsedMilliseconds;
            
            // Consolida itens de outras libs
            logCustom.MapearItensExternos(context);

            // Envia o log
            await _queue.EnqueueAsync(log);

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

    private async Task<string?> ObterRequestBody(HttpRequest request)
    {
        request.EnableBuffering();
        var body = await ReadStreamAsync(request.Body);
        request.Body.Position = 0;
        
        return JsonSanitizer.Sanitize(body, _settings.OfuscarCampos.Request);
    }

    private async Task<string?> ObterResponseBody(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        
        return JsonSanitizer.Sanitize(body, _settings.OfuscarCampos.Response);
    }

    private async Task<string> ReadStreamAsync(Stream stream)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        var content = await reader.ReadToEndAsync();
        return content;
    }
}
